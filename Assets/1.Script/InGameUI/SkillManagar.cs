using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManagar: Singleton<SkillManagar>
{
    [SerializeField] private Image[] fillImage;
    [SerializeField] private Button[] fillButton;
    [SerializeField] private HP hps;
    [SerializeField] private Image hp;

    void Start()
    {
        //fillButton.onClick.AddListener(StartFilling);
        for (int i = 0; i < fillImage.Length; i++)
        {
            fillImage[i].fillAmount = 0;
            fillButton[i].interactable = false;
        }
    }

    public IEnumerator BeadBurst(BeadType beadType)
    {
        float score = 0.05f;

        switch (beadType)
        {
            case BeadType.Fire:
                fillImage[0].fillAmount += score;
                break;
            case BeadType.Light:
                fillImage[1].fillAmount += score;
                break;
            case BeadType.Ice:
                fillImage[2].fillAmount += score;
                break;
            case BeadType.Dark:
                fillImage[3].fillAmount += score;
                break;
            case BeadType.Heal:
                fillImage[4].fillAmount += score;
                break;
            default:
                break;
        }

        for (int i = 0; i < fillImage.Length; i++)
            if (fillImage[i].fillAmount == 1)
                fillButton[i].interactable = true;

        yield return true;
    }

    public void UseSkill(int t)
    {
        fillButton[t].interactable = false;
        fillImage[t].fillAmount = 0;
        hps.Attack();
    }
}
