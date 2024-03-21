using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] public Image hpImage;
    private SkillManagar skills;

    private void Start()
    {
        hpImage.fillAmount = 1;
    }

    public void Attack()
    {
        hpImage.fillAmount -= 0.1f;

        if (hpImage.fillAmount == 0)
        {
            skills.enabled = false;
        }
    }
}
