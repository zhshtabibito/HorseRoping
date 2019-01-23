using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static int FREE = 0;
    private static int CHARGING = 1;
    private static int ROPING = 2;
    private static int DIZZY = 3;
    private static int THROWING = 4;

    public int id;
    public int score = 0;
    public int state = 0;

    public float spd0;
    private float spd;
    public float lenRope;

    private Rigidbody2D rb;

    public GameObject AimerObj;
    private Aimer aimer;

    private bool canDash;
    private float cdDash = 8f;
    private float rateDash = 3f;
    private float timeDash = 1f;

    private bool canPull;
    private float cdPull = 8f;
    // private float ratePull = 1 / 3;

    // Start is called before the first frame update
    void Start()
    {
        spd = spd0;
        aimer = AimerObj.GetComponent<Aimer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spd > spd0)
            spd -= spd0 * (rateDash - 1) * Time.deltaTime / timeDash;
        if (spd < spd0)
            spd = spd0;

        // for test on PC
        //***********************************************
        Vector2 v = Vector2.zero;
        if (id == 1 && state != DIZZY)
        {
            float LR = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            float UD = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            rb.MovePosition(rb.position + new Vector2(LR, UD) * spd * Time.deltaTime);

            if (Input.GetKey(KeyCode.Space) && (state == FREE || state == CHARGING))
            {
                state = CHARGING;
                aimer.AddR();
            }
            else if (Input.GetKeyUp(KeyCode.Space) && state == CHARGING)
            {
                StartCoroutine("RopeHorse");
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && state==ROPING)
            {
                // Dash
                DashCD();
                spd *= rateDash;

            }
            else if (Input.GetKeyDown(KeyCode.C) && canPull && state == ROPING)
            {
                // Pull
                PullCD();
                // Vector3 rope = World.horse.transform.position - transform.position;
                // World.horse.SetMoveVector(-rope.normalized * lenRope * ratePull);
            }

        }

        //************************************************

    }

    public void BeDizzy()
    {
        World.horse.state = 0;
        state = DIZZY;
    }

    IEnumerator DashCD()
    {
        canDash = false;
        yield return new WaitForSeconds(cdDash);
        canDash = true;
    }

    IEnumerator PullCD()
    {
        canPull = false;
        yield return new WaitForSeconds(cdPull);
        canPull = true;
    }

    IEnumerator RopeHorse()
    {
        state = THROWING;
        yield return new WaitForSeconds(aimer.CalDelay());
        if ((World.horse.transform.position - aimer.transform.position).magnitude <= aimer.R)
        {
            state = ROPING;
            // horse roped
        }


    }

}
