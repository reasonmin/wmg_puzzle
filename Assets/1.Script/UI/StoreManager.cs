using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;

    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject CoinShortage_Text;

    GameObject obj = null;

    private void Awake()
    {
        instance = this;
    }

    public void BuyProduct(int Value)
    {
        if(Value <= Singleton<PlayerDataManager>.Instance.playerData.coin)
        {
            Singleton<PlayerDataManager>.Instance.playerData.coin -= Value;
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
