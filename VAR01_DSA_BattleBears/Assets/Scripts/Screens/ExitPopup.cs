using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPopup : UIScreen
{

    public void OnConfirmButton()
    {
        UIManager.instance.Show<MenuScreen>();
        SceneManager.UnloadSceneAsync("GameScene");
    }

    public void OnCancelButton()
    {
        UIManager.instance.Show<GameScreen>();
    }

}
