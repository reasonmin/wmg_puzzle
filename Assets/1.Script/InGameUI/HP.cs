using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] public Image hpImage;
    [SerializeField] private GameObject gameObjects;
    private SkillManagar skills;
    public SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        hpImage.fillAmount = 1;
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {

    }

    public void Attack()
    {
        hpImage.fillAmount -= 0.2f;

        if (hpImage.fillAmount == 0)
        {
            skills.enabled = false;
        }

        StartCoroutine(FadeOver(0.9f));
    }


    private IEnumerator FadeOver(float duration)
    {
        StartCoroutine(FadeOutGetChild(gameObjects, duration));
        yield return new WaitForSeconds(0.005f);
        StartCoroutine(FadeInGetChildren(gameObjects, duration));

        spriteRenderer.color = originalColor;
    }

    private IEnumerator FadeOutGetChild(GameObject attackImage, float duration)
    {
        SpriteRenderer[] attackChild = attackImage.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(originalColor.a, 140f, lerpFactor);
            for (int i = 0; i < attackChild.Length; i++)
            {
                attackChild[i].color = newColor;
            }

            yield return null;
        }
    }

    private IEnumerator FadeInGetChildren(GameObject attackImage, float duration)
    {
        SpriteRenderer[] attackChild = attackImage.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(0f, originalColor.a, lerpFactor);
            for (int i = 0; i < attackChild.Length; i++)
            {
                attackChild[i].color = newColor;
            }

            yield return null;
        }
    }
}
