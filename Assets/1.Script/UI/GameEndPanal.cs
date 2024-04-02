using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndPanal : MonoBehaviour
{
    public void OnClear(bool t)
    {
        if (BoardManager.Instance.timeFlow < 40)
            PlayerDataManager.Instance.playerData.chapterDatas[SceneChange.instance.ChapterNum - 1].stageDatas[SceneChange.instance.stageNum - 1] = 3;
        else if (BoardManager.Instance.timeFlow < 80)
            PlayerDataManager.Instance.playerData.chapterDatas[SceneChange.instance.ChapterNum - 1].stageDatas[SceneChange.instance.stageNum - 1] = 2;
        else
            PlayerDataManager.Instance.playerData.chapterDatas[SceneChange.instance.ChapterNum - 1].stageDatas[SceneChange.instance.stageNum - 1] = 1;

        if (SceneChange.instance.ChapterNum == PlayerDataManager.Instance.playerData.curChapter && SceneChange.instance.stageNum == PlayerDataManager.Instance.playerData.curStage)
        {
            PlayerDataManager.Instance.playerData.curStage++;
            if (PlayerDataManager.Instance.playerData.curStage == 11)
            {
                PlayerDataManager.Instance.playerData.curChapter++;
                PlayerDataManager.Instance.playerData.curStage = 0;
            }
        }
        
        PlayerDataManager.Instance.SaveJson();

        if(t)
            SceneManager.LoadScene("Main");
    }

    public void OnDie()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnReTray(bool isClear)
    {
        if (isClear)
            OnClear(false);

        SceneManager.LoadScene("Game");
    }
}
