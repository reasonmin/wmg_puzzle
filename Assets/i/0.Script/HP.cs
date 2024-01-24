using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private Image hpImage;

    private void Start()
    {
        //hpImage.fillAmount = 1;
        Hp = 0;
    }

    private void Update()
    {   
        if (hpImage.fillAmount == 0)
        {
            hpImage.fillAmount = 1;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            hp += 10;
        }
    }

    private void Damage()
    {
        hpImage.fillAmount -= 0.1f;
    }

    private float Maxhp = 100;
    private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hpImage.rectTransform.sizeDelta = new Vector2((hp / Maxhp) * 720, 30);
        }
    }
}
