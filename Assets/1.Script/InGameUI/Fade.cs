using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] private GameObject es;
    [SerializeField] private Animator eAni;
    [SerializeField] private Image hp;
    [SerializeField] private Image hpbg;
    [SerializeField] private GameObject gameClear;
    [SerializeField] private GameObject disablePan;
    [SerializeField] private Transform parent;

    private float destroytimer = 0f;
    private float destroytime = 2f;

    private bool tf = true;

    private Color originColor;

    /// <summary>
    /// enemy�� hp�� �� �������� �� 
    /// ���� ���� �ٿ� ������ ������� ��ó�� ����
    /// </summary>

    void Start()
    {
        originColor = new(255f, 255f, 255f);
        disablePan.SetActive(false);
    }

    void Update()
    {
        if (hp.fillAmount == 0 && tf)
        {
            tf = false;
            StartCoroutine(ChangeColor(es, hpbg));
            eAni.enabled = false;            
        }
    }

    private IEnumerator ChangeColor(GameObject g, Image image)
    {
        SpriteRenderer[] allchild = g.GetComponentsInChildren<SpriteRenderer>();
        float desvalue = 1f;
        while (desvalue > 0)
        {
            destroytimer += Time.deltaTime;
            desvalue = 1f - (destroytimer / destroytime);
            Color newColor = new(originColor.r, originColor.g, originColor.b, Mathf.Clamp01(desvalue));
            Debug.Log(desvalue);
            for (int i = 0; i < allchild.Length; i++)
            {
                allchild[i].color = newColor;
            }

            image.color = newColor;

            yield return null;
        }

        Instantiate(gameClear, parent);
        disablePan.SetActive(true);
        
    }
}
