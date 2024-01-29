using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum LanguageType
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

public class Lenguage : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    public Sentence sentence;

    // Update is called once per frame
    void Update()
    {
        switch (PlayerDataManager.instance.playerData.language)
        {
            case LanguageType.English:
                _Text.text = sentence.English;
                break;
            case LanguageType.Korean:
                _Text.text = sentence.Korean;
                break;
        }
    }
}
