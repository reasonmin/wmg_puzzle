using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill: MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Button fillButton;
    [SerializeField] private Image hp;
    [SerializeField] private KeyCode key;

    private float fillTime = 2f;
    private bool isFilling = false;
    private float fillTimer = 0f;

    void Start()
    {
        fillButton.onClick.AddListener(StartFilling);
        fillImage.fillAmount = 0;
    }

    void Update()
    {
        if(fillImage.fillAmount == 0)
        {
            fillButton.interactable = false;
        }

        if (isFilling)
        {
            fillButton.interactable = false;
            fillTimer += Time.deltaTime;
            float fillvalue = 1f - (fillTimer / fillTime);
            fillImage.fillAmount = Mathf.Clamp01(fillvalue);

            if (fillTimer >= fillTime)
            {
                isFilling = false;
                fillTimer = 0f;
            }
        }

        if(Input.GetKeyDown(key) && fillImage.fillAmount != 1)
        {
            fillButton.interactable = false;
            fillImage.fillAmount += 0.5f;
        }

        if (fillImage.fillAmount == 1)
        {
            fillButton.interactable = true;
        }

        /*
        if(hp.fillAmount == 0)
        {
            hp.fillAmount = 1;
        }
        */
        
    }

    private void StartFilling()
    {
        isFilling = true;
        hp.fillAmount -= 0.5f;
    }
}
