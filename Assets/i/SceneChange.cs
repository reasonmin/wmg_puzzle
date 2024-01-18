using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public static SceneChange instance;
    //public RectTransform ScrollContents;



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
            //setRectPosition();
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

    /*
    public void setRectPosition()
    {
        float y = ScrollContents.anchoredPosition.y;
        ScrollContents.anchoredPosition = new Vector2(0, y);
    }
    */

}
