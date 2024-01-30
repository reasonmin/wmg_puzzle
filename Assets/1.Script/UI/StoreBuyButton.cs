using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreBuyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;

    public void OnBuy()
    {
        StoreManager.instance.BuyProduct(int.Parse(_Text.text));
    }
}
