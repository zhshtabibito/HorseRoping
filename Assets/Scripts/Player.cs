using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static int FREE = 0;
    private static int CHARGING = 1;
    private static int ROPING = 2;
    private static int DIZZY = 3;

    public int id;
    public int score = 0;
    public int state = 0;

    public float spd;
    public float lenRope;

    public GameObject AimerObj;
    private Aimer aimer;

    private bool canDash; private float cdDash; private float vDash;
    private bool canPull; private float cdPull; private float disPull;

    // Start is called before the first frame update
    void Start()
    {
        aimer = AimerObj.GetComponent<Aimer>();
    }

    // Update is called once per frame
    void Update()
    {
        // for test on PC
        //***********************************************
        Vector2 v = Vector2.zero;
        if (id == 1 && state != DIZZY)
        {
            float LR = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            float UD = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            transform.position += new Vector3(LR * spd * Time.deltaTime, UD * spd * Time.deltaTime, 0);

            if (Input.GetKey(KeyCode.Space) && (state == FREE || state == CHARGING))
            {
                state = CHARGING;
                aimer.AddR();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                if((World.horse.transform.position - aimer.transform.position).magnitude <= aimer.R)
                {
                    // horse roped

                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && state==ROPING)
            {
                // Dash


            }

            if (Input.GetKeyDown(KeyCode.C) && canPull && state == ROPING)
            {
                // Pull

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

}
