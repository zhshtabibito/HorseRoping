using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    static protected GameController _instance;
    static public GameController Instance{ get { return _instance; } }

    public UIController uiController;

    public List<Player> players;
    public List<Vector3> playersSpawnPos;
    public Vector3 horseSpawnPos;
    public Horse horse;
    public float maxTime;
    public float curTime;
    public int winScore;
    public bool isPause;

    private void Awake()
    {
        _instance = this;
    }


    private void Start()
    {
        playersSpawnPos = new List<Vector3>();
        for (int i = 0; i < players.Count; i++)
        {
            playersSpawnPos.Add(players[i].gameObject.transform.position);
        }
        horseSpawnPos = horse.transform.position;
        curTime = maxTime;
    }

    private void Update()
    {
        if (!isPause)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].score > winScore)
                {
                    EndGame(players[i]);
                }
            }
            if (horse.state != 0)
            {
                players[horse.state - 1].score += Time.deltaTime;
            }
            curTime -= Time.deltaTime;
            if (curTime < 0)
            {
                Player winPlayer = players[0];
                for (int i = 1; i < players.Count; i++)
                {
                    if (players[i].score > winPlayer.score)
                        winPlayer = players[i];
                }
                EndGame(winPlayer);
            }
        }
    }

    public void EndGame(Player winPlayer)
    {
        isPause = true;
        Time.timeScale = 0f;
        Debug.Log("nmsl");
        uiController.EndGame(winPlayer);
    }

    public void PauseGame()
    {
        if (!isPause)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].playerInput.ReleaseControl(false);
                players[i].playerInput.GainControl();
            }
            isPause = true;
            Time.timeScale = 0;
            Debug.Log("nmsl");
            uiController.Pause();
        }
        else
        {
            Unpause();
        }
    }

    public void Unpause()
    {
        //if the timescale is already > 0, we 
        if (Time.timeScale > 0)
            return;
        uiController.Continue();
        StartCoroutine(UnpauseCoroutine());
    }
    public IEnumerator UnpauseCoroutine()
    {
        Time.timeScale = 1;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerInput.GainControl();
        }
        //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
        //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        isPause = false;
    }


    public void InitGame()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Respawn(true, playersSpawnPos[i]);
        }
        horse.Respawn(horseSpawnPos);
        curTime = maxTime;
        uiController.RestartGame();
        Time.timeScale = 1;
        isPause = false;
    }

    public void BackToHome()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
    }
}
