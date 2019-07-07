using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : UIScreen
{
    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.instance.Show<ExitPopup>();
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
