using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill: MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Button fillButton;
    [SerializeField] private Image hp;

    private float fillTime = 5f;
    private bool isFilling = false;
    private float fillTimer = 0f;

    void Start()
    {
        fillButton.onClick.AddListener(StartFilling);
    }

    void Update()
    {
        // 채우기 중인 경우
        if (isFilling)
        {
            fillButton.interactable = false;
            fillTimer += Time.deltaTime / fillTime;
            fillImage.fillAmount = Mathf.Clamp01(fillTimer);

            if (fillTimer >= 1f)
            {
                fillButton.interactable = true;
                isFilling = false;
                fillTimer = 0f;
            }
        }

        if(hp.fillAmount == 0)
        {
            hp.fillAmount = 1;
        }
    }

    private void StartFilling()
    {
        isFilling = true;
        hp.fillAmount -= 0.1f;
    }
}
