using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Php : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Animator ani;

    private void Start()
    {
        StartCoroutine(Attacked());
    }

    public void ModifyHp(RectTransform rect, float amount)
    {
        Vector2 currentSize = rectTransform.sizeDelta;
        currentSize.x += amount;
        currentSize.x = Mathf.Clamp(currentSize.x, 0f, 720f);
        rectTransform.sizeDelta = currentSize;
        rectTransform.pivot = new Vector2(0f, 0.5f);
    }

    private IEnumerator Attacked()
    {
        float speed = 4f;

        while (true)
        {
            float time = 0f;
            while (time < speed)
            {
                yield return new WaitWhile(() => SkillManagar.Instance.Ongold);
                yield return new WaitForSeconds(0.2f);
                time += 0.2f;
            }

            ani.SetTrigger("attack");
            ModifyHp(rectTransform, -50);
        }
    }
}
