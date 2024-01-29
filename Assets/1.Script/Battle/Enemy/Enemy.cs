using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public int MaxHP; // 최대 생명력
    public int ATK; // 공격력
    public int MaxGauge; // 필살기를 쓰기 위해서 필요한 게이지
}

public class Enemy : MonoBehaviour
{
    public static Enemy Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    public int HP;
    public int Gauge;
    public int ATK;
    public bool Play;

    void Start()
    {
        Play = false;
    }

    public void SetData(EnemyData _enemyData)
    {
        HP = _enemyData.MaxHP;
        Gauge = _enemyData.MaxGauge;
        ATK = _enemyData.ATK;
    }

    public IEnumerator StartTurn()
    {
        Play = true;
        yield return new WaitWhile(() => Play);
    }

    public IEnumerator Attack()
    {
        Gauge++;
        yield return null;
    }

    public void Hit(int DMG)
    {
        HP = HP - DMG;
    }

    public void Death()
    {

    }

    // Slime(슬라임)
    // Kobold(코볼트)
    // Zombie(좀비)
    // Skeleton(스켈레톤)
    // Wolf(늑대)
    // Banshee(벤시)
    // Minotauros(미노타우르스)
    // Ogre(오우거)
    // Golem(골렘)
    // Vampire(흠혈귀)
    // Wyvern(와이번)

    // SlimeKing(슬라임킹)
    // Gargoyle(가고일)
    // Dullahan(듀라한)
    // Medusa(메두사)
    // Yeti(예티)
    // Phoenix(피닉스)
    // Sphinx(스핑크스)
    // Kraken(크라켄)
    // Lich(리치)
    // Dragon(드래곤)
}
