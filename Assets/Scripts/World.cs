using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;
public class World : MonoBehaviour
{
    public static int MAIN = 0;
    public static int JIASHI = 1;
    public static int stateGame;
    public static float timeMain;

    public static Horse horse;
    private static bool scoring;
    public static List<Player> players;
    
    // Start is called before the first frame update
    void Start()
    {
        horse = GameObject.Find("Horse").GetComponent<Horse>();
        players = new List<Player>();
        players.Add(GameObject.Find("P1").GetComponent<Player>());
        players.Add(GameObject.Find("P2").GetComponent<Player>());

        timeMain = 150f;
        StartCoroutine("TimingEnd");
    }

    IEnumerator TimingEnd()
    {
        yield return new WaitForSeconds(timeMain);
        Player winner = players[0];
        foreach (Player player in players)
            if (player.score > winner.score)
                winner = player;

        if (horse.state != winner.id && horse.state != 0)
            stateGame = JIASHI;
        else
            GameOver(winner);
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

    public static void HandleHorseState()
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
        Debug.Log(players[0].score.ToString() + " vs " + players[1].score.ToString());
        scoring = false;
    }

    public static void GameOver(Player winner)
    {
        Debug.Log("Game Over!");
        Debug.Log("Palyer" + winner.id.ToString() + " wins!");
        Time.timeScale = 0;
    }


}
