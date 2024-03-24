using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public Image fillImage;
    public Button fillButton;

    private float fillTime = 2f;
    private float fillTimer = 0f;

    public bool isFilling = false;

    private void Start()
    {
        fillButton.onClick.AddListener(StartFilling);
    }

    private void Update()
    {
        if(isFilling)
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
    }

    public void StartFilling()
    {
        isFilling = true;
    }
}
