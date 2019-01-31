using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;
public class GameController : MonoBehaviour
{
    static protected GameController _instance;
    static public GameController Instance{ get { return _instance; } }

    public UIController uiController;

    public List<Player> players;
    public Horse horse;
    public float maxTime;
    public float curTime;
    public int winScore;

    private void Awake()
    {
        _instance = this;
    }


    private void Start()
    {
        players = new List<Player>(FindObjectsOfType<Player>());
        horse = FindObjectOfType<Horse>();
    }

    private void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].score >= winScore)
            {

            }
        }
    }

    public void GameEnd(Player winPlayer)
    {
        
    }

    public void GamePause(bool Pause)
    {
        if (Pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
