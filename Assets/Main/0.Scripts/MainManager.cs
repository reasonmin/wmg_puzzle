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
    public int musicVolume;
    public int soundEffectVolume;
    public bool isMusic;
    public bool isSoundEffect;

    public Item item;
    public List<ChapterData> chapterData = new();
    
    public ChapterDatas( List<ChapterData> _chapterData, Item _item)
    {
        chapterData = _chapterData;
        item = _item;

        musicVolume = 50;
        musicVolume = 50;
        isMusic = true;
        isSoundEffect = true;
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
    [SerializeField] private GameObject LanguagePanel;

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SoundEffectSlider;

    [SerializeField] private GameObject BeforeButton;
    [SerializeField] private GameObject StageView;
    [SerializeField] private GameObject StoreView;

    private ChapterDatas chapterDatas;

    // Start is called before the first frame update
    void Start()
    {
        //ResetJson();
        LoadJson();// Player 정보 불러오기

        SetStageStar();// 불러온 정보로 스테이지 스타 세팅

        SettingPanel.SetActive(false);// 설정창 끄기
        OnStageView();// 뷰를 스테이지 뷰로 설정
    }

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

    private void SetStageStar()
    {
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
    }

    // 음량 조절
    //-----------------------------------------------------
    public void OnMusicVolume()
    {
        if (chapterDatas.isMusic)
            chapterDatas.musicVolume = (int)MusicSlider.value;

        Debug.Log(chapterDatas.musicVolume);
    }

    public void OnSoundEffectVolume()
    {
        if (chapterDatas.isMusic)
            chapterDatas.soundEffectVolume = (int)SoundEffectSlider.value;

        Debug.Log(chapterDatas.soundEffectVolume);
    }

    public void OnMusic()
    {
        chapterDatas.isMusic = !chapterDatas.isMusic;

        if (!chapterDatas.isMusic)
            chapterDatas.musicVolume = 0;
        else
            chapterDatas.musicVolume = (int)MusicSlider.value;

        Debug.Log(chapterDatas.musicVolume);
    }

    public void OnSoundEffect()
    {
        chapterDatas.isSoundEffect = !chapterDatas.isSoundEffect;

        if (!chapterDatas.isSoundEffect)
            chapterDatas.soundEffectVolume = 0;
        else
            chapterDatas.soundEffectVolume = (int)SoundEffectSlider.value;

        Debug.Log(chapterDatas.soundEffectVolume);
    }
    //-----------------------------------------------------

    // SettingPanel 관련 함수
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
        LanguagePanel.SetActive(false);

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

    public void OnLanguagePanel()
    {
        ResetPanel(LanguagePanel);
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
