using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemButton : MonoBehaviour
{
    public TMP_Text _Text;
    public GameObject CheckImage;

    public void OnUseItem()
    {
        if(_Text.text != "0")
        {
            _Text.gameObject.SetActive(!_Text.gameObject.activeSelf);
            CheckImage.SetActive(!CheckImage.activeSelf);
        }
    }

    public void ReSetting()
    {
        _Text.gameObject.SetActive(true);
        CheckImage.SetActive(false);
    }
}
