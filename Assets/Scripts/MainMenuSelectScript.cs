using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelectScript : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("Loading Screen");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Loading Screen");
        PlayerPrefs.DeleteAll();
    }
}
