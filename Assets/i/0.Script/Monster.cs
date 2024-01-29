using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [SerializeField] private Image eImage;
    [SerializeField] private Image hp;

    private float destroytimer = 0f;
    private float destroytime = 2f;

    private Color originColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hp.fillAmount == 0)
        {
            destroytimer += Time.deltaTime;
            float desvalue = 1f - (destroytimer / destroytime);
            Color newColor = new(originColor.r, originColor.g, originColor.b, Mathf.Clamp01(desvalue));
            eImage.color = newColor;
            hp.color = newColor;
        }
    }
}
