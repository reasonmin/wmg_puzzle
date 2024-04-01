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

    [SerializeField] private TMP_Text Coin_Text;

    public AudioSource Music_Audio;
    public AudioSource click_Audio;

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

        //PlayerDataManager.Instance.ResetJson();
        PlayerDataManager.Instance.LoadJson();// Player 정보 불러오기

        SetStageButton();// 불러온 정보로 스테이지 버튼 세팅
        SetVolume();// 음향 세팅
        SetCoin();// 코인 개수 세팅

        SettingPanel.SetActive(false);// 설정창 끄기

        StoreView.SetActive(false);
        StageView.SetActive(true);// 뷰를 스테이지 뷰로 설정

        Music_Audio.Play();
    }

    public void SetCoin()
    {
        Coin_Text.text = PlayerDataManager.Instance.playerData.coin.ToString();
        PlayerDataManager.Instance.SaveJson();
    }

    private void SetVolume()
    {
        if (PlayerDataManager.Instance.playerData.isMusic)
            MusicSlider.value = PlayerDataManager.Instance.playerData.musicVolume;
        if (PlayerDataManager.Instance.playerData.isSoundEffect)
            SoundEffectSlider.value = PlayerDataManager.Instance.playerData.soundEffectVolume;

        Music_Audio.volume = PlayerDataManager.Instance.playerData.musicVolume / 100f;
        click_Audio.volume = PlayerDataManager.Instance.playerData.soundEffectVolume / 100f;

        MusicBanImage.SetActive(!PlayerDataManager.Instance.playerData.isMusic);
        SoundEffectBanImage.SetActive(!PlayerDataManager.Instance.playerData.isSoundEffect);
    }

    private void SetStageButton()
    {
        // 스테이지 스타 개수 + 레밸 넘버 수정
        for (int i = 0; i < chapterBoards.Count; i++)
        {
            for (int j = 0; j < chapterBoards[i].stageButtons.Count; j++)
            {
                chapterBoards[i].stageButtons[j].stageNumText.text = (i + 1).ToString() + "-" + (j + 1).ToString();
                chapterBoards[i].stageButtons[j].ChapterNum = i + 1;
                chapterBoards[i].stageButtons[j].stageNum = j + 1;
                for (int k = 0; k < PlayerDataManager.Instance.playerData.chapterDatas[i].stageDatas[j]; k++)
                {
                    chapterBoards[i].stageButtons[j].starImages[k].sprite = StarImage.sprite;
                }
            }
        }

        // 스테이지 해금
        for (int i = 0; i < PlayerDataManager.Instance.playerData.curChapter; i++)
            for (int j = 0; j < PlayerDataManager.Instance.playerData.curStage; j++)
                chapterBoards[i].stageButtons[j].gameObject.SetActive(true);

        chapterBoards[PlayerDataManager.Instance.playerData.curChapter - 1].stageButtons[PlayerDataManager.Instance.playerData.curStage - 1].ButtonImage.sprite = LockStageButtonImage.sprite;
    }

    public void OnLanguageChange(int n)
    {
        click_Audio.Play();

        switch (n)
        {
            case (int)LanguageType.English:
                PlayerDataManager.Instance.playerData.language = LanguageType.English;
                break;
            case (int)LanguageType.Korean:
                PlayerDataManager.Instance.playerData.language = LanguageType.Korean;
                break;
        }

        PlayerDataManager.Instance.SaveJson();
        //Debug.Log(playerData.language);
    }

    // 게임시작
    //-----------------------------------------------------
    public void SetGameStartPanel(string stageNum)
    {
        click_Audio.Play();

        gameStartPanel.Title_Text.text = stageNum;
        gameStartPanel.bronze._Text.text = PlayerDataManager.Instance.playerData.item.bronze.ToString();
        gameStartPanel.silver._Text.text = PlayerDataManager.Instance.playerData.item.silver.ToString();
        gameStartPanel.gold._Text.text = PlayerDataManager.Instance.playerData.item.gold.ToString();

        gameStartPanel.bronze.ReSetting();
        gameStartPanel.silver.ReSetting();
        gameStartPanel.gold.ReSetting();

        gameStartPanel.Panel.SetActive(true);
    }

    public void OnGameStartPanel()
    {
        click_Audio.Play();

        gameStartPanel.Panel.SetActive(false);
    }

    public void OnGameStart()
    {
        click_Audio.Play();

        if (gameStartPanel.bronze.CheckImage.activeSelf)
            PlayerDataManager.Instance.playerData.item.bronze--;
        if (gameStartPanel.silver.CheckImage.activeSelf)
            PlayerDataManager.Instance.playerData.item.silver--;
        if (gameStartPanel.gold.CheckImage.activeSelf)
            PlayerDataManager.Instance.playerData.item.gold--;

        PlayerDataManager.Instance.SaveJson();

        PlayerPrefs.SetString("bronze", gameStartPanel.bronze.CheckImage.activeSelf.ToString());
        PlayerPrefs.SetString("silver", gameStartPanel.silver.CheckImage.activeSelf.ToString());
        PlayerPrefs.SetString("gold", gameStartPanel.gold.CheckImage.activeSelf.ToString());

        SceneChange.instance.y = rTransform.anchoredPosition.y;
        SceneChange.instance.OnGoGame();
    }
    //-----------------------------------------------------

    // 음량 조절
    //-----------------------------------------------------
    public void OnMusicVolume()
    {
        if (PlayerDataManager.Instance.playerData.isMusic)
            PlayerDataManager.Instance.playerData.musicVolume = (int)MusicSlider.value;

        Music_Audio.volume = PlayerDataManager.Instance.playerData.musicVolume / 100f;
        PlayerDataManager.Instance.SaveJson();
        //Debug.Log(PlayerDataManager.Instance.playerData.musicVolume);
    }

    public void OnSoundEffectVolume()
    {
        if (PlayerDataManager.Instance.playerData.isSoundEffect)
            PlayerDataManager.Instance.playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        click_Audio.volume = PlayerDataManager.Instance.playerData.soundEffectVolume / 100f;
        PlayerDataManager.Instance.SaveJson();
        //Debug.Log(PlayerDataManager.Instance.playerData.soundEffectVolume);
    }

    public void OnMusic()
    {
        click_Audio.Play();

        PlayerDataManager.Instance.playerData.isMusic = !PlayerDataManager.Instance.playerData.isMusic;

        if (!PlayerDataManager.Instance.playerData.isMusic)
            PlayerDataManager.Instance.playerData.musicVolume = 0;
        else
            PlayerDataManager.Instance.playerData.musicVolume = (int)MusicSlider.value;

        MusicBanImage.SetActive(!PlayerDataManager.Instance.playerData.isMusic);

        Music_Audio.volume = PlayerDataManager.Instance.playerData.musicVolume / 100f;
        PlayerDataManager.Instance.SaveJson();
        //Debug.Log(PlayerDataManager.Instance.playerData.musicVolume);
    }

    public void OnSoundEffect()
    {
        click_Audio.Play();

        PlayerDataManager.Instance.playerData.isSoundEffect = !PlayerDataManager.Instance.playerData.isSoundEffect;

        if (!PlayerDataManager.Instance.playerData.isSoundEffect)
            PlayerDataManager.Instance.playerData.soundEffectVolume = 0;
        else
            PlayerDataManager.Instance.playerData.soundEffectVolume = (int)SoundEffectSlider.value;

        SoundEffectBanImage.SetActive(!PlayerDataManager.Instance.playerData.isSoundEffect);

        click_Audio.volume = PlayerDataManager.Instance.playerData.soundEffectVolume / 100f;
        PlayerDataManager.Instance.SaveJson();
        //Debug.Log(PlayerDataManager.Instance.playerData.soundEffectVolume);
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
        click_Audio.Play();

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
        click_Audio.Play();

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