using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static int MAIN = 0;
    public static int JIASHI = 1;
    public int stateGame;
    public float timeMain;

    public Horse horse;
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
        {
            if (player.score > winner.score)
            {
                winner = player;
            }
        }
        if(horse.state != winner.id)
        {
            stateGame = JIASHI;
        }
        else
        {
            GameOver(winner);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(stateGame == JIASHI)
        {

        }
    }

    public void GameOver(Player winner)
    {
        Debug.Log("Game Over!");
        Debug.Log("Palyer" + winner.id.ToString() + " wins!");
        Time.timeScale = 0;
    }
}
