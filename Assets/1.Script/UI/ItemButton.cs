using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemButton : MonoBehaviour
{
    public TMP_Text _Text;
    [SerializeField] private GameObject CheckImage;

    public void OnUseItem()
    {
        _Text.gameObject.SetActive(!_Text.gameObject.activeSelf);
        CheckImage.SetActive(!CheckImage.activeSelf);
    }

    public void ReSetting()
    {
        _Text.gameObject.SetActive(true);
        CheckImage.SetActive(false);
    }
}
