using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] es;
    [SerializeField] private Animator eAni;
    [SerializeField] private Image hp;
    [SerializeField] private Image hpbg;
    [SerializeField] private GameObject hpGameobject;

    private float destroytimer = 0f;
    private float destroytime = 2f;

    private Color originColor;

    void Start()
    {
        originColor = es[1].color;
    }

    void Update()
    {
        if (hp.fillAmount == 0)
        {
            destroytimer += Time.deltaTime;
            float desvalue = 1f - (destroytimer / destroytime);
            Color newColor = new(originColor.r, originColor.g, originColor.b, Mathf.Clamp01(desvalue));
            for (int i = 0; i < 4; i++)
            {
                es[i].color = newColor;
            }
            eAni.enabled = false;
            hpbg.color = newColor;

            if (newColor.a == 0f)
            {
                Destroy(hpGameobject);
            }
            
        }
    }
}
