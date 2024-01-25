using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    EnemyData enemyData = new();

    // Start is called before the first frame update
    void Start()
    {
        enemyData.MaxHP = 100;
        enemyData.MaxGauge = 5;
        enemyData.ATK = 10;

        Enemy.Instance.SetData(enemyData);
    }

    private bool D = true;

    private void Update()
    {
        if (Enemy.Instance.Play && D)
        {
            D = false;
            StartCoroutine(StartTurn());
        }
    }

    public IEnumerator StartTurn()
    {
        if (Enemy.Instance.Gauge >= enemyData.MaxGauge)
            StartCoroutine(SkillAttack());
        else
            yield return StartCoroutine(Enemy.Instance.Attack());
        Enemy.Instance.Play = false;
        D = true;
    }

    public IEnumerator SkillAttack()
    {
        Enemy.Instance.Gauge = 0;
        yield return null;
    }
}
