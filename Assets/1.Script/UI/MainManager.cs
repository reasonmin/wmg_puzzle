using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Item
{
    public int gold;
    public int silver;
    public int bronze;
}

[System.Serializable]
public class GameStartPanel
{
    public GameObject Panel;
    public TMP_Text Title_Text;
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

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public List<ChapterBoard> chapterBoards;
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

    [SerializeField] private RectTransform rTransform;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneChange.instance.y > 0)
            rTransform.anchoredPosition = new Vector2(0, SceneChange.instance.y);

        gameStartPanel.Panel.SetActive(false);

        PlayerDataManager.instance.ResetJson();
        PlayerDataManager.instance.LoadJson();// Player 정보 불러오기

        SetStageButton();// 불러온 정보로 스테이지 버튼 세팅
        SetVolume();// 음향 세팅

        SettingPanel.SetActive(false);// 설정창 끄기
        OnStageView();// 뷰를 스테이지 뷰로 설정
    }

    private void SetVolume()
    {
        if (PlayerDataManager.instance.playerData.isMusic)
            MusicSlider.value = PlayerDataManager.instance.playerData.musicVolume;
        if (PlayerDataManager.instance.playerData.isMusic)
            SoundEffectSlider.value = PlayerDataManager.instance.playerData.soundEffectVolume;

        MusicBanImage.SetActive(!PlayerDataManager.instance.playerData.isMusic);
        SoundEffectBanImage.SetActive(!PlayerDataManager.instance.playerData.isSoundEffect);
    }

    private void SetStageButton()
    {
        // 스테이지 스타 개수 + 레밸 넘버 수정
        for (int i = 0; i < chapterBoards.Count; i++)
        {
            for (int j = 0; j < chapterBoards[i].stageButtons.Count; j++)
            {
                chapterBoards[i].stageButtons[j].stageNumText.text = (i + 1).ToString() + "-" + (j + 1).ToString();

                for (int k = 0; k < PlayerDataManager.instance.playerData.chapterDatas[i].stageDatas[j]; k++)
                {
                    chapterBoards[i].stageButtons[j].starImages[k].sprite = StarImage.sprite;
                }
            }
        }

        // 스테이지 해금
        for (int i = 0; i < PlayerDataManager.instance.playerData.curChapter; i++)
            for (int j = 0; j < PlayerDataManager.instance.playerData.curStage; j++)
                chapterBoards[i].stageButtons[j].gameObject.SetActive(true);

        chapterBoards[PlayerDataManager.instance.playerData.curChapter - 1].stageButtons[PlayerDataManager.instance.playerData.curStage - 1].ButtonImage.sprite = LockStageButtonImage.sprite;
    }

    public void OnLanguageChange(int n)
    {
        switch (n)
        {
            case (int)LanguageType.English:
                PlayerDataManager.instance.playerData.language = LanguageType.English;
                break;
            case (int)LanguageType.Korean:
                PlayerDataManager.instance.playerData.language = LanguageType.Korean;
                break;
        }

        PlayerDataManager.instance.SaveJson();
        //Debug.Log(playerData.language);
    }

    // 게임시작
    //-----------------------------------------------------
    public void SetGameStartPanel(string stageNum)
    {
        gameStartPanel.Title_Text.text = stageNum;
        gameStartPanel.bronze._Text.text = PlayerDataManager.instance.playerData.item.bronze.ToString();
        gameStartPanel.silver._Text.text = PlayerDataManager.instance.playerData.item.silver.ToString();
        gameStartPanel.gold._Text.text = PlayerDataManager.instance.playerData.item.gold.ToString();

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
        if (PlayerDataManager.instance.playerData.isMusic)
            PlayerDataManager.instance.playerData.musicVolume = (int)MusicSlider.value;

        PlayerDataManager.instance.SaveJson();
        Debug.Log(PlayerDataManager.instance.playerData.musicVolume);
    }

    public void OnSoundEffectVolume()
    {
        if (PlayerDataManager.instance.playerData.isMusic)
            PlayerDataManager.instance.playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        PlayerDataManager.instance.SaveJson();
        Debug.Log(PlayerDataManager.instance.playerData.soundEffectVolume);
    }

    public void OnMusic()
    {
        PlayerDataManager.instance.playerData.isMusic = !PlayerDataManager.instance.playerData.isMusic;

        if (!PlayerDataManager.instance.playerData.isMusic)
            PlayerDataManager.instance.playerData.musicVolume = 0;
        else
            PlayerDataManager.instance.playerData.musicVolume = (int)MusicSlider.value;

        MusicBanImage.SetActive(!PlayerDataManager.instance.playerData.isMusic);

        PlayerDataManager.instance.SaveJson();
        Debug.Log(PlayerDataManager.instance.playerData.musicVolume);
    }

    public void OnSoundEffect()
    {
        PlayerDataManager.instance.playerData.isSoundEffect = !PlayerDataManager.instance.playerData.isSoundEffect;

        if (!PlayerDataManager.instance.playerData.isSoundEffect)
            PlayerDataManager.instance.playerData.soundEffectVolume = 0;
        else
            PlayerDataManager.instance.playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        SoundEffectBanImage.SetActive(!PlayerDataManager.instance.playerData.isSoundEffect);

        PlayerDataManager.instance.SaveJson();
        Debug.Log(PlayerDataManager.instance.playerData.soundEffectVolume);
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