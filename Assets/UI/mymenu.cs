using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mymenu : MonoBehaviour
{
    public RectTransform PauseTr;
     ///public static GameState gameState = UIGameState.Menu;
    public void Pause()
    {
        
        PauseTr.gameObject.SetActive(true);
    }
    public void mainmenu()
    {
        
    
       
        PauseTr.gameObject.SetActive(false);
        
    }
    
   public void restartbtn()
    {
        SceneManager.LoadScene(1);

    }
    public void restartbtn2(){
        SceneManager.LoadScene(0);
        ///gameState = UIGameState.Menu;
        //restart.gameObject.SetActive(false);
    }
    public void quit(){
        Application.Quit();
    }
    
    void Start()
    {
        
    }

}