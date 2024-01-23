using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] Image hpImage;

    private void Start()
    {
        hpImage.fillAmount = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hpImage.fillAmount -= 0.1f;
        }

        if (hpImage.fillAmount == 0)
        {
            hpImage.fillAmount = 1;
        }

        //coolTimeImage.fillAmount = delta / cooltime;
    }
}
