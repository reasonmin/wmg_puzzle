using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Text : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    public Sentence sentence;

    // Update is called once per frame
    void Update()
    {
        switch (MainManager.instance.playerData.language)
        {
            case Language.English:
                _Text.text = sentence.English;
                break;
            case Language.Korean:
                _Text.text = sentence.Korean;
                break;
        }
    }
}
