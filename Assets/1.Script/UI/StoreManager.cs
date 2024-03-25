using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;

    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject CoinShortage_Text;

    private GameObject obj = null;

    private void Awake()
    {
        instance = this;
    }

    public void BuyProduct(int Value, Item items)
    {
        if(Value <= PlayerDataManager.Instance.playerData.coin)
        {
            PlayerDataManager.Instance.playerData.item.bronze += items.bronze;
            PlayerDataManager.Instance.playerData.item.silver += items.silver;
            PlayerDataManager.Instance.playerData.item.gold += items.gold;

            PlayerDataManager.Instance.playerData.coin -= Value;
            MainManager.instance.SetCoin();
        }
        else
        {
            if (obj == null)
                obj = Instantiate(CoinShortage_Text, canvas);
            else
            {
                Destroy(obj);
                obj = Instantiate(CoinShortage_Text, canvas);
            }
        }
    }
}
