using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PotionType
{
    Fire,
    Ice,
    Heal,
    Light,
    Dark
}
public class Potion : MonoBehaviour
{
    public Potion potionType; //����

    public int xIndex;  //������ x��ǥ
    public int yIndex;  //������ y��ǥ

    public Potion(int _x, int _y)   //������
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void MoveToTarget()
    {

    }
}