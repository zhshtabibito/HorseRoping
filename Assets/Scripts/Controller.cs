using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Mathf.Approximately(Input.GetAxis("moveHorizontal 1"), 0f))
            Debug.Log("moveHorizontal 1: " + Input.GetAxis("moveHorizontal 1"));

        if (!Mathf.Approximately(Input.GetAxis("moveHorizontal 2"), 0f))
            Debug.Log("moveHorizontal 2: " + Input.GetAxis("moveHorizontal 2"));
    }
}
