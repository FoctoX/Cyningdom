using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private void Start()
    {
        LoadedScene();
    }

    private void LoadedScene()
    {
        SceneManager.LoadSceneAsync("Tutorial");
    }
}
