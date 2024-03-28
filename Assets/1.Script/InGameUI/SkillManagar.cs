using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManagar: Singleton<SkillManagar>
{
    [SerializeField] private Skill[] skills;
    [SerializeField] private HP hps;
    [SerializeField] private Php php;
    [SerializeField] private RectTransform recT;

    public bool Ongold = false;

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
                skills[1].fillImage.fillAmount += 0.08f;
                break;
            case BeadType.Ice:
                skills[2].fillImage.fillAmount += 0.05f;
                break;
            case BeadType.Dark:
                skills[3].fillImage.fillAmount += 0.02f;
                break;
            case BeadType.Heal:
                skills[4].fillImage.fillAmount += 0.04f;
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
        switch (t)
        {
            case 0:
                hps.Attack(5);
                break;
            case 1:
                hps.Attack(7);
                break;
            case 2:
                hps.Attack(10);
                break;
            case 3:
                hps.Attack(20);
                break;
            case 4:
                php.ModifyHp(recT, 216);
                break;
        }
    }
}
