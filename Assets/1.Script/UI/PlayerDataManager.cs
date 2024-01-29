using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public int gold;
    public Item item;

    public LanguageType language;

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
        language = LanguageType.English;

        musicVolume = 50;
        soundEffectVolume = 50;
        isMusic = true;
        isSoundEffect = true;

        curChapter = 1;
        curStage = 1;
    }
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    public PlayerData playerData;
    private string filePath = "Assets/8.Data/PlayerData.json";

    public void LoadJson() //�ҷ�����
    {
        string json = File.ReadAllText(filePath);

        playerData = JsonUtility.FromJson<PlayerData>(json);
    }

    public void SaveJson() //����
    {
        string json = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(filePath, json);
    }

    public void ResetJson() //�ʱ�ȭ
    {
        List<ChapterData> chapterData = new List<ChapterData>();
        List<int> li = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < MainManager.instance.chapterBoards.Count; i++)
            chapterData.Add(new ChapterData(li));

        Item items = new Item();
        items.gold = 0;
        items.silver = 0;
        items.bronze = 0;

        playerData = new PlayerData(chapterData, items);

        string json = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(filePath, json);
    }
}
