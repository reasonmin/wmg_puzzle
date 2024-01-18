using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class Item
{
    public int candy;
}

[System.Serializable]
public class ChapterData
{
    public List<int> stageData;
    public ChapterData(List<int> _stageData)
    {
        stageData = _stageData;
    }
}

[System.Serializable]
public class ChapterDatas
{
    public Item item;
    public List<ChapterData> chapterData = new();

    public ChapterDatas(List<ChapterData> _chapterData, Item _item)
    {
        chapterData = _chapterData;
        item = _item;
    }
}

public class MainManager : MonoBehaviour
{
    [SerializeField] private List<ChapterBoard> chapterBoards;
    [SerializeField] private Image Star;

    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject MainBoardPanel;
    [SerializeField] private GameObject RuleBookPanel;
    [SerializeField] private GameObject AudioPanel;
    [SerializeField] private GameObject BeforeButton;
    [SerializeField] private GameObject StageView;
    [SerializeField] private GameObject StoreView;

    private ChapterDatas chapterDatas;

    private void LoadJson() //불러오기
    {
        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = File.ReadAllText(filePath);

        chapterDatas = JsonUtility.FromJson<ChapterDatas>(json);
    }

    private void SaveJson() //저장
    {
        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = JsonUtility.ToJson(chapterDatas, true);

        File.WriteAllText(filePath, json);
    }

    private void ResetJson() //초기화
    {
        List<ChapterData> chapterData = new List<ChapterData>();
        List<int> li = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < chapterBoards.Count; i++)
            chapterData.Add(new ChapterData(li));

        Item items = new Item();
        items.candy = 0;

        chapterDatas = new ChapterDatas(chapterData, items);

        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = JsonUtility.ToJson(chapterDatas, true);

        File.WriteAllText(filePath, json);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadJson();

        for (int i = 0; i < chapterBoards.Count; i++)
        {
            for (int j = 0; j < chapterBoards[i].stageButtons.Count; j++)
            {
                chapterBoards[i].stageButtons[j].stageNumText.text = (i + 1).ToString() + "-" + (j + 1).ToString();

                for (int k = 0; k < chapterDatas.chapterData[i].stageData[j]; k++)
                {
                    chapterBoards[i].stageButtons[j].starImages[k].sprite = Star.sprite;
                }
            }
        }

        SettingPanel.SetActive(false);
        OnStageView();
    }

    // Setting 관련 함수
    //-----------------------------------------------------
    public void OnSettingPanel()
    {
        ResetPanel(MainBoardPanel);
        BeforeButton.SetActive(false);
        SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void ResetPanel(GameObject Panel)
    {
        MainBoardPanel.SetActive(false);
        RuleBookPanel.SetActive(false);
        AudioPanel.SetActive(false);

        Panel.SetActive(true);
    }

    public void OnBefore()
    {
        ResetPanel(MainBoardPanel);
        BeforeButton.SetActive(false);
    }

    public void OnRuleBookPanel()
    {
        ResetPanel(RuleBookPanel);
        BeforeButton.SetActive(true);
    }

    public void OnAudioPanel()
    {
        ResetPanel(AudioPanel);
        BeforeButton.SetActive(true);
    }
    //-----------------------------------------------------

    // View 관련 함수
    //-----------------------------------------------------
    public void ResetView(GameObject View)
    {
        StageView.SetActive(false);
        StoreView.SetActive(false);

        View.SetActive(true);
    }

    public void OnStageView()
    {
        ResetView(StageView);
    }

    public void OnStoreView()
    {
        ResetView(StoreView);
    }
    //-----------------------------------------------------
}
