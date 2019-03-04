using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject panel;
    public GameObject menu;

    public Text timeMinutes;
    public Text timeSeconds;

    public List<GameObject> playerPanel = new List<GameObject>();
    public List<Slider> scoreSlider = new List<Slider>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerPanel.Count; i++)
        {
            scoreSlider.Add(playerPanel[i].GetComponentInChildren<Slider>());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < GameController.Instance.players.Count; i++)
        {
            playerPanel[i].SetActive(true);
            scoreSlider[i].value = GameController.Instance.players[i].score / GameController.Instance.winScore;
        }
        timeMinutes.text = ((int)(GameController.Instance.curTime / 60f)).ToString();
        timeSeconds.text = ((int)(GameController.Instance.curTime % 60f)).ToString();
    }

    public void EndGame(Player player)
    {
        panel.SetActive(true);
        panel.GetComponentInChildren<Text>().text = "Player " + player.playerID.ToString() + "  WIN";
    }

    public void RestartGame()
    {
        panel.SetActive(false);
    }

    public void Pause()
    {
        menu.SetActive(true);
    }

    public void Continue()
    {
        menu.SetActive(false);
    }
}
