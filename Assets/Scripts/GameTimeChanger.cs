using UnityEngine;
using PlayerController;

public static class GameTimeChanger
{
    public static void StopTime()
    {
        Time.timeScale = 0;
        InputHandler.Instance.SetInputLockState(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void PlayTime()
    {
        Time.timeScale = 1;
        InputHandler.Instance.SetInputLockState(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
