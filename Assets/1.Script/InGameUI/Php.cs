using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Php : MonoBehaviour
{
    public Animator ani;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Eskill eskill;
    private float damage = 50;
    private float Heal = 216;

    private void Start()
    {
        StartCoroutine(AttackManage());
    }

    void Update()
    {

    }

    public void FillHp(RectTransform rect)
    {
        Vector2 currentSize = rectTransform.sizeDelta;
        if(currentSize.x != 720)
        {
            currentSize.x += Heal;
            currentSize.x = Mathf.Max(0, currentSize.x);
            rectTransform.sizeDelta = currentSize;
            rectTransform.pivot = new Vector2(0f, 0.5f);
        }
    }

    public void UnfillHp(RectTransform rect)
    {
        Vector2 currentSize = rectTransform.sizeDelta;
        currentSize.x -= damage;
        currentSize.x = Mathf.Max(0, currentSize.x);
        rectTransform.sizeDelta = currentSize;
        rectTransform.pivot = new Vector2(0f, 0.5f);
    }

    private IEnumerator Attacked(bool t)
    {
        if(t)
        {
            while (true)
            {
                ani.SetTrigger("attack");
                UnfillHp(rectTransform);

                yield return new WaitForSeconds(5f);
            }
        }
    }

    private IEnumerator AttackManage()
    {
        StartCoroutine(Attacked(false));
        yield return new WaitForSeconds(5f);
        StartCoroutine(Attacked(true));
    }
}
