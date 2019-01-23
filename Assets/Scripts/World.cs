using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;
public class World : MonoBehaviour
{
    public static int MAIN = 0;
    public static int JIASHI = 1;
    public int stateGame;
    public float timeMain;

    public static Horse horse;
    public List<Player> players;
    
    // Start is called before the first frame update
    void Start()
    {
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

    }

    public void CheckGameOver()
    {
        // called when score add
        foreach (Player player in players)
            if (player.score >= 100)
                GameOver(player);
    }

    public void HandleHorseState()
    {
        // called when 
        if (stateGame == JIASHI)
        {
            Player winner = players[0];
            foreach (Player player in players)
                if (player.score > winner.score)
                    winner = player;
            GameOver(winner);
        }
    }

    public void GameOver(Player winner)
    {
        Debug.Log("Game Over!");
        Debug.Log("Palyer" + winner.id.ToString() + " wins!");
        Time.timeScale = 0;
    }
}
