using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Php : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    private float shrinkAmount = 72.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 currentSize = rectTransform.sizeDelta;

            currentSize.x -= shrinkAmount;

            currentSize.x = Mathf.Max(0, currentSize.x);

            rectTransform.sizeDelta = currentSize;

            rectTransform.pivot = new Vector2(0f, 0.5f);
        }
    }
}
