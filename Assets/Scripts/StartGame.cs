using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public static void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public static void StartGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
