using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public static SceneChange instance;

    public float y;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnGoStage();
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    public void OnGoStage()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnGoGame()
    {
        SceneManager.LoadScene("Game");
    }
}
