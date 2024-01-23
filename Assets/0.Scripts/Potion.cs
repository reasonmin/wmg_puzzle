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
    public Potion potionType; //종류

    public int xIndex;  //보드의 x좌표
    public int yIndex;  //보드의 y좌표

    public Potion(int _x, int _y)   //생성자
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