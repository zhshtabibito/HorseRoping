using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimer : MonoBehaviour
{
    public GameObject playerObj;
    private Player player;
    public GameObject arrowL;
    public GameObject arrowR;

    public float R;
    private readonly float Rmin = 1;
    private readonly float Rmax = 3;
    private readonly float spd = 5;
    private readonly float step = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<Player>();
        arrowR.transform.localScale = new Vector2(-1, 1);
        ResetAimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.state <=1 && player.id == 1) // not roping
        {
            // PC test
            //***********************************************
            float LR = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            float UD = Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            transform.position += new Vector3(LR * spd * Time.deltaTime, UD * spd * Time.deltaTime, 0);
            //***********************************************
            Vector3 rope = transform.position - player.transform.position;
            float mag = rope.magnitude;
            if(mag > player.lenRope)
            {
                rope = rope * (player.lenRope / rope.magnitude);
                transform.position = player.transform.position + rope;
            }
            arrowL.transform.position = transform.position - new Vector3(R, 0, 0);
            arrowR.transform.position = transform.position + new Vector3(R, 0, 0);
        }


    }

    public void ResetAimer()
    {
        Debug.Log("Aimer reset");
        R = Rmin;
        Vector3 temp = transform.position - new Vector3(R, 0, 0);
        arrowL.transform.position = transform.position - new Vector3(R, 0, 0);
        arrowR.transform.position = transform.position + new Vector3(R, 0, 0);
        GetComponent<SpriteRenderer>().enabled = true;
        arrowL.GetComponent<SpriteRenderer>().enabled = true;
        arrowR.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void HideAimer()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        arrowL.GetComponent<SpriteRenderer>().enabled = false;
        arrowR.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void AddR()
    {
        R += step*Time.deltaTime;
        if (R > Rmax)
            R = Rmax;
    }

    public float CalDelay()
    {
        return 0.8f-0.2f*(R-Rmin)/step;
    }
}
