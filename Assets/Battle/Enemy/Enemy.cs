using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public int MaxHP; // �ִ� �����
    public int ATK; // ���ݷ�
    public int MaxGauge; // �ʻ�⸦ ���� ���ؼ� �ʿ��� ������
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

    // Slime(������)
    // Kobold(�ں�Ʈ)
    // Zombie(����)
    // Skeleton(���̷���)
    // Wolf(����)
    // Banshee(����)
    // Minotauros(�̳�Ÿ�츣��)
    // Ogre(�����)
    // Golem(��)
    // Vampire(������)
    // Wyvern(���̹�)

    // SlimeKing(������ŷ)
    // Gargoyle(������)
    // Dullahan(�����)
    // Medusa(�޵λ�)
    // Yeti(��Ƽ)
    // Phoenix(�Ǵн�)
    // Sphinx(����ũ��)
    // Kraken(ũ����)
    // Lich(��ġ)
    // Dragon(�巡��)
}
