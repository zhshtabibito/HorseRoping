using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = (int)(GameController.Instance.timeMain / 60) + "     " + (int)(GameController.Instance.timeMain % 60);
    }

    public void SetTimePanel(float seconds)
    {
        text.text = (int)(seconds / 60) + "     " + (int)(seconds % 60);
    }
}
