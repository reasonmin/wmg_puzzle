using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManagar: Singleton<SkillManagar>
{
    [SerializeField] private Skill[] skills;
    [SerializeField] private HP hps;
    [SerializeField] private Image hp;
    [SerializeField] private Php php;
    [SerializeField] private RectTransform recT;

    void Start()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].fillImage.fillAmount = 0;
            skills[i].fillButton.interactable = false;
        }
    }

    public void UseSilverItem()
    {
        float score = 0.5f;

        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].fillImage.fillAmount += score;
            if (skills[i].fillImage.fillAmount == 1)
                skills[i].fillButton.interactable = true;
        }
    }

    public IEnumerator BeadBurst(BeadType beadType)
    {

        switch (beadType)
        {
            case BeadType.Fire:
                skills[0].fillImage.fillAmount += 0.1f;
                break;
            case BeadType.Light:
                skills[1].fillImage.fillAmount += 0.06f;
                break;
            case BeadType.Ice:
                skills[2].fillImage.fillAmount += 0.07f;
                break;
            case BeadType.Dark:
                skills[3].fillImage.fillAmount += 0.02f;
                break;
            case BeadType.Heal:
                skills[4].fillImage.fillAmount += 0.05f;
                break;
            default:
                break;
        }

        for (int i = 0; i < skills.Length; i++)
            if (skills[i].fillImage.fillAmount == 1)
                skills[i].fillButton.interactable = true;

        yield return 0;
    }

    public void UseSkill(int t)
    {
        skills[t].fillButton.interactable = false;
        skills[t].fillImage.fillAmount = 0;
        if(t == 4)
        {
            php.FillHp(recT);
        }
        else
        {
            hps.Attack();
        }
    }
}
