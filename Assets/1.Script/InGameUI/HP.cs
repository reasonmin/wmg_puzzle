using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] public Image hpImage;
    private SkillManagar skills;
    public SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        hpImage.fillAmount = 1;
    }

    private void Update()
    {

    }

    public void Attack()
    {
        hpImage.fillAmount -= 0.1f;

        if (hpImage.fillAmount == 0)
        {
            skills.enabled = false;
        }

        StartCoroutine(FadeAlphaOverTime(0.6f));
    }

    private IEnumerator FadeAlphaOverTime(float duration)
    {
        originalColor = spriteRenderer.color;

        // Fade out
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(originalColor.a, 0f, lerpFactor);
            spriteRenderer.color = newColor;
            yield return null;
        }

        // Wait for a while
        yield return new WaitForSeconds(0.2f); // Change this to the desired duration

        // Fade in
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(0f, originalColor.a, lerpFactor);
            spriteRenderer.color = newColor;
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }
}
