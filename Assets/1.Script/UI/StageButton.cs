using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public Image ButtonImage;
    public TMP_Text stageNumText;
    public int stageNum;
    public int ChapterNum;
    public List<Image> starImages;

    public void OnStart()
    {
        SceneChange.instance.stageNum = stageNum;
        SceneChange.instance.ChapterNum = ChapterNum;
        MainManager.instance.SetGameStartPanel(stageNumText.text);
    }
}
