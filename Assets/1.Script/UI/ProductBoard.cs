using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProductBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text BuyButton_Text;
    [SerializeField] private Item items;

    public void OnBuy()
    {
        StoreManager.instance.BuyProduct(int.Parse(BuyButton_Text.text), items);
    }
}
