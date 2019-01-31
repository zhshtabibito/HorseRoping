using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject panel;
    public GameObject menu;

    public GameObject[] playerPanel = new GameObject[4];
    public Slider[] scoreSlider = new Slider[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        //for (int i = 0; i < GameController.Instance.players.Count; i++)
        //{
        //    playerPanel[i].SetActive(true);
        //    scoreSlider[i].value = GameController.Instance.players[i].score / 100f;
        //}
        //Debug.Log(GameController.Instance.players[0].score);
    }

    public void EndGame(Player player)
    {
        panel.SetActive(true);
        panel.GetComponentInChildren<Text>().text = "Player " + player.playerID.ToString() + "  WIN";
    }

    public void DisablePanel()
    {
        panel.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("Start");
    }
}
