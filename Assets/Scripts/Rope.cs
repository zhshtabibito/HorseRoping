using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private bool isThrowing;
    private SpriteRenderer sr;
    public Vector3 vec;
    public float time;
    public GameObject line;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (isThrowing)
        {
            transform.position = transform.position + vec * Time.deltaTime / time;
            line.GetComponent<LineRenderer>().SetPosition(0, GetComponentInParent<Player>().transform.position);
            line.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        }
        else if(GetComponentInParent<Player>().state == Player.ROPING)
        {
            line.GetComponent<LineRenderer>().SetPosition(0, GetComponentInParent<Player>().transform.position);
            line.GetComponent<LineRenderer>().SetPosition(0, World.horse.transform.position);
        }
    }

    public void Throw(float time, Vector3 vec)
    {
        this.time = time;
        this.vec = vec;
        transform.position = GetComponentInParent<Player>().transform.position;
        line.GetComponent<LineRenderer>().enabled = true;
        line.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        line.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        StartCoroutine("TimingAndThrow");
    }

    IEnumerator TimingAndThrow()
    {
        isThrowing = true;
        sr.enabled = true;
        yield return new WaitForSeconds(time);
        isThrowing = false;
        sr.enabled = false;

    }

    public void BreakLine()
    {
        line.GetComponent<LineRenderer>().enabled = false;
    }



}
