using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndPanal : MonoBehaviour
{
    public void OnClear()
    {
        PlayerDataManager.Instance.playerData.chapterDatas[PlayerDataManager.Instance.playerData.curChapter - 1].stageDatas[PlayerDataManager.Instance.playerData.curStage - 1] = 3;
        PlayerDataManager.Instance.playerData.curStage++;
        if(PlayerDataManager.Instance.playerData.curStage == 11)
        {
            PlayerDataManager.Instance.playerData.curChapter++;
            PlayerDataManager.Instance.playerData.curStage = 0;
        }
        PlayerDataManager.Instance.SaveJson();
        SceneManager.LoadScene("Main");
    }

    public void OnDie()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnReTray(bool isClear)
    {
        if (isClear)
        {
            PlayerDataManager.Instance.playerData.chapterDatas[PlayerDataManager.Instance.playerData.curChapter - 1].stageDatas[PlayerDataManager.Instance.playerData.curStage - 1] = 3;
            PlayerDataManager.Instance.playerData.curStage++;
            if (PlayerDataManager.Instance.playerData.curStage == 11)
            {
                PlayerDataManager.Instance.playerData.curChapter++;
                PlayerDataManager.Instance.playerData.curStage = 0;
            }
            PlayerDataManager.Instance.SaveJson();
        }

        SceneManager.LoadScene("Game");
    }
}
