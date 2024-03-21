using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManagar: Singleton<SkillManagar>
{
    [SerializeField] private Skill[] skills;
    [SerializeField] private HP hps;
    [SerializeField] private Image hp;

    private float fillTime = 2f;
    private float fillTimer = 0f;

    void Start()
    {
        //fillButton.onClick.AddListener(StartFilling);
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].fillButton.onClick.AddListener(skills[i].StartFilling);

            skills[i].fillImage.fillAmount = 0;
            skills[i].fillButton.interactable = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].isFilling)
            {
                skills[i].fillButton.interactable = false;
                fillTimer += Time.deltaTime;
                float fillvalue = 1f - (fillTimer / fillTime);
                skills[i].fillImage.fillAmount = Mathf.Clamp01(fillvalue);
            }
        }
    }

    public IEnumerator BeadBurst(BeadType beadType)
    {
        float score = 0.05f;

        switch (beadType)
        {
            case BeadType.Fire:
                skills[0].fillImage.fillAmount += score;
                break;
            case BeadType.Light:
                skills[1].fillImage.fillAmount += score;
                break;
            case BeadType.Ice:
                skills[2].fillImage.fillAmount += score;
                break;
            case BeadType.Dark:
                skills[3].fillImage.fillAmount += score;
                break;
            case BeadType.Heal:
                skills[4].fillImage.fillAmount += score;
                break;
            default:
                break;
        }

        for (int i = 0; i < skills.Length; i++)
            if (skills[i].fillImage.fillAmount == 1)
                skills[i].fillButton.interactable = true;

        yield return true;
    }

    public void UseSkill(int t)
    {
        skills[t].fillButton.interactable = false;
        skills[t].fillImage.fillAmount = 0;
        hps.Attack();
    }
}
