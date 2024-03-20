using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill: MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Button fillButton;
    [SerializeField] private HP hps;
    [SerializeField] private Image hp;
    [SerializeField] private Skill skillCs;
    [SerializeField] private KeyCode key;
    [SerializeField] private BeadType beadsType;

    private Bead[,] beads;

    private float fillTime = 2f;
    private bool isFilling = false;
    private float fillTimer = 0f;
    private int height = 8;
    private int width = 7;

    void Start()
    {
        fillButton.onClick.AddListener(StartFilling);
        fillImage.fillAmount = 0;
    }

    void Update()
    {
        fill();

        if (isFilling)
        {
            fillTimer += Time.deltaTime;
            float fillvalue = 1f - (fillTimer / fillTime);
            fillImage.fillAmount = Mathf.Clamp01(fillvalue);

            if (fillTimer >= fillTime)
            {
                isFilling = false;
                fillTimer = 0f;
            }
        }

        bool[,] check = new bool[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                check[i, j] = false;
            }
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (check[i, j])
                {
                    // beads[i, j]가 null이 아니고, 비활성화된 경우에만 조건 검사
                    if (beads[i, j] != null && !beads[i, j].gameObject.activeSelf)
                    {
                        //BeadType beadType = beads[i, j].Type;
                        if (BoardManager.lastSetActiveBeadType == this.beadsType && fillImage.fillAmount != 1)
                        {
                            fillButton.interactable = false;
                            fillImage.fillAmount += 0.5f;
                        }
                    }
                }
            }
        }


    }

    private void StartFilling()
    {
        isFilling = true;
        hps.Damage(hp, skillCs);
    }

    private void fill()
    {
        if (fillImage.fillAmount != 1)
        {
            fillButton.interactable = false;
        }
        else
        {
            fillButton.interactable = true;
        }
    }
}
