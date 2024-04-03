using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] public Image hpImage;
    [SerializeField] private GameObject gameObjects;
    [SerializeField] private Eskill eskill;
    
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Color originalColor;

    /// <summary>
    /// ��ü������ enemy�� ���ݹ޴� �Լ���
    /// </summary>

    private void Start()
    {
        hpImage.fillAmount = 1;
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// enemy ����
    /// </summary>
    /// <param name="Dmg"></param>
    public void Attack(int Dmg)
    {
        animator.SetTrigger("isHit");
        hpImage.fillAmount -= Dmg / 100f;
        StartCoroutine(eskill.ImageChange());

        StartCoroutine(FadeOver(0.9f));
    }

    /// <summary>
    /// enemy�� player���� ���� �޾��� �� 
    /// Ÿ�� ���� ��ó�� ��� ���� ���� �پ����� �þ
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator FadeOver(float duration)
    {
        StartCoroutine(Fade(gameObjects, originalColor.a, 0f, duration * 0.5f));
        yield return new WaitForSeconds(0.005f);
        StartCoroutine(Fade(gameObjects, 0f, originalColor.a, duration * 0.5f));

        spriteRenderer.color = originalColor;
    }

    private IEnumerator Fade(GameObject attackImage, float startAlpha, float targetAlpha, float duration)
    {
        SpriteRenderer[] attackChild = attackImage.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, lerpFactor);
            foreach (SpriteRenderer childRenderer in attackChild)
            {
                childRenderer.color = newColor;
            }
            yield return null;
        }
    }
}
