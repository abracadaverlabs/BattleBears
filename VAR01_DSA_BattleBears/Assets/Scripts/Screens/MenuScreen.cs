using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : UIScreen
{
    
    public void OnStartButton ()
    {
        UIManager.instance.Show<GameScreen>();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
 
}
