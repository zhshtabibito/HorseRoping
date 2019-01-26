using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;
public class World : MonoBehaviour
{
    static protected World s_WorldInstance;
    static public World WorldInstance{ get { return s_WorldInstance; } }

    public UIController uiController;

    //public int MAIN = 0;
    public int JIASHI = 1;
    public int stateGame;
    public float timeMain;

    public Horse horse;
    private bool scoring;
    public List<Player> players;

    private void Awake()
    {
        s_WorldInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        horse = GameObject.Find("Horse").GetComponent<Horse>();
        players = new List<Player>();
        players.Add(GameObject.Find("P1").GetComponent<Player>());
        players.Add(GameObject.Find("P2").GetComponent<Player>());

        StartGame();
    }

    IEnumerator TimingEnd()
    {
        //timing to game over
        while (true)
        {
            if (timeMain < 0)
            {
                Player winner = players[0];
                foreach (Player player in players)
                    if (player.score > winner.score)
                        winner = player;

                if (horse.state != winner.playerID && horse.state != 0)
                    stateGame = JIASHI;
                else
                    GameOver(winner);
                yield break;
            }
            else
            {
                timeMain -= Time.deltaTime;
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(horse.state != 0 && !scoring)
        {
            scoring = true;
            StartCoroutine("AddScore");
        }
        else if(horse.state == 0)
        {
            scoring = false;
            StopCoroutine("AddScore");
        }    
    }

    public void CheckGameOver()
    {
        // called when score add
        if(stateGame != JIASHI)
            foreach (Player player in players)
                if (player.score >= 100)
                    GameOver(player);
    }

    public void HandleHorseState()
    {
        // called when horse or player roped
        if (stateGame == JIASHI)
        {
            Player winner = players[0];
            foreach (Player player in players)
                if (player.score > winner.score)
                    winner = player;
            GameOver(winner);
        }
    }

    IEnumerator AddScore()
    {
        yield return new WaitForSeconds(0.4f);
        players[horse.state - 1].score += 1;
        CheckGameOver();
        Debug.Log(players[0].score.ToString() + " vs " + players[1].score.ToString());
        scoring = false;
    }

    public void GameOver(Player winner)
    {
        //Debug.Log("Game Over!");
        //Debug.Log("Palyer" + winner.playerID.ToString() + " wins!");
        Time.timeScale = 0;
        uiController.EndGame(winner);
    }

    public void StartGame()
    {
        timeMain = 150f;
        uiController.DisablePanel();
        StartCoroutine("TimingEnd");
        Time.timeScale = 1;
    }

}
