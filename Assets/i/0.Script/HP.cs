using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] public Image hpImage;

    private void Start()
    {
        hpImage.fillAmount = 1;
    }

    private void Update()
    {   
        /*
        if (hpImage.fillAmount == 0)
        {
            hpImage.fillAmount = 1;
        }
        */
    }

    public void Damage()
    {
        hpImage.fillAmount -= 0.1f;
    }
}
