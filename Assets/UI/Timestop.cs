using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timestop : MonoBehaviour {

		public RectTransform TS;

		public void OnClickPause()
		{
				Time.timeScale = 1;
				//TS.gameObject.SetActive (false);
		}
		public	void  back()
		{
				Time.timeScale = 0;
				//TS.gameObject.SetActive (true);
		}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
