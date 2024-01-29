using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    [SerializeField] private List<ChapterBoard> chapterBoards;
    [SerializeField] private Image StarImage;
    [SerializeField] private Image LockStageButtonImage;

    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject MainBoardPanel;
    [SerializeField] private GameObject RuleBookPanel;
    [SerializeField] private GameObject AudioPanel;
    [SerializeField] private GameObject LanguagePanel;

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SoundEffectSlider;
    [SerializeField] private GameObject MusicBanImage;
    [SerializeField] private GameObject SoundEffectBanImage;

    [SerializeField] private GameObject BeforeButton;
    [SerializeField] private GameObject StageView;
    [SerializeField] private GameObject StoreView;

    [SerializeField] private GameStartPanel gameStartPanel;
    [SerializeField] private ItemButtons itemButtons;

    [SerializeField] private RectTransform rTransform;

    public PlayerData playerData;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneChange.instance.y > 0)
        {
            rTransform.anchoredPosition = new Vector2(0, SceneChange.instance.y);
        }

        gameStartPanel.Panel.SetActive(false);

        ResetJson();
        LoadJson();// Player 정보 불러오기

        SetStageButton();// 불러온 정보로 스테이지 버튼 세팅
        SetVolume();// 음향 세팅

        SettingPanel.SetActive(false);// 설정창 끄기
        OnStageView();// 뷰를 스테이지 뷰로 설정
    }

    private void LoadJson() //불러오기
    {
        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = File.ReadAllText(filePath);

        playerData = JsonUtility.FromJson<PlayerData>(json);
    }

    private void SaveJson() //저장
    {
        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(filePath, json);
    }

    private void ResetJson() //초기화
    {
        List<ChapterData> chapterData = new List<ChapterData>();
        List<int> li = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < chapterBoards.Count; i++)
            chapterData.Add(new ChapterData(li));

        Item items = new Item();
        items.gold = 0;
        items.silver = 0;
        items.bronze = 0;

        playerData = new PlayerData(chapterData, items);

        string filePath = "Assets/Main/3.Data/PlayerData.json";
        string json = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(filePath, json);
    }

    private void SetVolume()
    {
        if (playerData.isMusic)
            MusicSlider.value = playerData.musicVolume;
        if (playerData.isMusic)
            SoundEffectSlider.value = playerData.soundEffectVolume;

        MusicBanImage.SetActive(!playerData.isMusic);
        SoundEffectBanImage.SetActive(!playerData.isSoundEffect);
    }

    private void SetStageButton()
    {
        // 스테이지 스타 개수 + 레밸 넘버 수정
        for (int i = 0; i < chapterBoards.Count; i++)
        {
            for (int j = 0; j < chapterBoards[i].stageButtons.Count; j++)
            {
                chapterBoards[i].stageButtons[j].stageNumText.text = (i + 1).ToString() + "-" + (j + 1).ToString();

                for (int k = 0; k < playerData.chapterDatas[i].stageDatas[j]; k++)
                {
                    chapterBoards[i].stageButtons[j].starImages[k].sprite = StarImage.sprite;
                }
            }
        }

        // 스테이지 해금
        for (int i = 0; i < playerData.curChapter; i++)
            for (int j = 0; j < playerData.curStage; j++)
                chapterBoards[i].stageButtons[j].gameObject.SetActive(true);

        chapterBoards[playerData.curChapter - 1].stageButtons[playerData.curStage - 1].ButtonImage.sprite = LockStageButtonImage.sprite;
    }

    public void OnLanguageChange(int n)
    {
        switch (n)
        {
            case (int)Language.English:
                playerData.language = Language.English;
                break;
            case (int)Language.Korean:
                playerData.language = Language.Korean;
                break;
        }

        SaveJson();
        //Debug.Log(playerData.language);
    }

    // 게임시작
    //-----------------------------------------------------
    public void SetGameStartPanel(string stageNum)
    {
        gameStartPanel.Title_Text.text = stageNum;
        itemButtons.bronze._Text.text = playerData.item.bronze.ToString();

        gameStartPanel.Panel.SetActive(true);
    }

    public void OnGameStartPanel()
    {
        gameStartPanel.Panel.SetActive(false);
    }

    public void OnGameStart()
    {
        SceneChange.instance.y = rTransform.anchoredPosition.y;
        SceneChange.instance.OnGoGame();
    }
    //-----------------------------------------------------

    // 음량 조절
    //-----------------------------------------------------
    public void OnMusicVolume()
    {
        if (playerData.isMusic)
            playerData.musicVolume = (int)MusicSlider.value;

        SaveJson();
        Debug.Log(playerData.musicVolume);
    }

    public void OnSoundEffectVolume()
    {
        if (playerData.isMusic)
            playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        SaveJson();
        Debug.Log(playerData.soundEffectVolume);
    }

    public void OnMusic()
    {
        playerData.isMusic = !playerData.isMusic;

        if (!playerData.isMusic)
            playerData.musicVolume = 0;
        else
            playerData.musicVolume = (int)MusicSlider.value;

        MusicBanImage.SetActive(!playerData.isMusic);

        SaveJson();
        Debug.Log(playerData.musicVolume);
    }

    public void OnSoundEffect()
    {
        playerData.isSoundEffect = !playerData.isSoundEffect;

        if (!playerData.isSoundEffect)
            playerData.soundEffectVolume = 0;
        else
            playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        SoundEffectBanImage.SetActive(!playerData.isSoundEffect);

        SaveJson();
        Debug.Log(playerData.soundEffectVolume);
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

public enum Language
{
    English,
    Korean
}

[System.Serializable]
public class Sentence
{
    public string English;
    public string Korean;
}

[System.Serializable]
public class GameStartPanel
{
    public GameObject Panel;
    public TMP_Text Title_Text;
}

[System.Serializable]
public class Item
{
    public int gold;
    public int silver;
    public int bronze;
}

[System.Serializable]
public class ItemButtons
{
    public ItemButton gold;
    public ItemButton silver;
    public ItemButton bronze;
}

[System.Serializable]
public class ChapterData
{
    public List<int> stageDatas;
    public ChapterData(List<int> _stageData)
    {
        stageDatas = _stageData;
    }
}

[System.Serializable]
public class PlayerData
{
    public int gold;
    public Item item;

    public Language language;

    public int musicVolume;
    public int soundEffectVolume;
    public bool isMusic;
    public bool isSoundEffect;

    public int curChapter;
    public int curStage;
    public List<ChapterData> chapterDatas = new();

    public PlayerData(List<ChapterData> _chapterData, Item _item)
    {
        chapterDatas = _chapterData;
        item = _item;

        gold = 0;
        language = Language.English;

        musicVolume = 50;
        soundEffectVolume = 50;
        isMusic = true;
        isSoundEffect = true;

        curChapter = 1;
        curStage = 1;
    }
}