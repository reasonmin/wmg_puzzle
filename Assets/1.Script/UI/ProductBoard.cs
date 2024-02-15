using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProductBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text BuyButton_Text;

    public void OnBuy()
    {
        StoreManager.instance.BuyProduct(int.Parse(BuyButton_Text.text));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
