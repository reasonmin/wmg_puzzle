using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;

    [SerializeField] private WarningMessage CoinShortage_Text;

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
            CoinShortage_Text.gameObject.SetActive(true);
            CoinShortage_Text.Act();
        }
    }
}
