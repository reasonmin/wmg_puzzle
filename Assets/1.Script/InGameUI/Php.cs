using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Php : MonoBehaviour
{
    public Animator ani;

    [SerializeField] private RectTransform rectTransform;

    private void Start()
    {
        StartCoroutine(AttackManage());
    }

    void Update()
    {

    }

    public void ModifyHp(RectTransform rect, float amount)
    {
        Vector2 currentSize = rectTransform.sizeDelta;
        currentSize.x += amount;
        currentSize.x = Mathf.Clamp(currentSize.x, 0f, 720f);
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
                ModifyHp(rectTransform, -50);

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
