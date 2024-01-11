using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Potion : MonoBehaviour
{
    public PotionType potionType;

    public int xIndex;  //보드의 x좌표
    public int yIndex;  //보드의 y좌표

    public bool isMatched;  //물약이 보드 안에 있는지 확인
    private Vector2 currentPos; //현재 물약 위치
    private Vector2 targetPos;  //물약이 이동할 위치

    public bool isMoving;   //물약이 이동중인지 확인
    

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

    //MoveToTarget

    //MoveCoroutine
}

public enum PotionType  //물약 종류
{
    Red,
    Blue,
    Pink,
    Green,
    White
}
