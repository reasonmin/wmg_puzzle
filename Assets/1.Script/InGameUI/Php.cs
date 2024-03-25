using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Php : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    private float damage = 72.0f;
    private float Heal = 72.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 currentSize = rectTransform.sizeDelta;
            currentSize.x -= damage;
            currentSize.x = Mathf.Max(0, currentSize.x);
            rectTransform.sizeDelta = currentSize;
            rectTransform.pivot = new Vector2(0f, 0.5f);
        }
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
}
