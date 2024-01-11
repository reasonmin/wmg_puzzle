using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Potion : MonoBehaviour
{
    public PotionType potionType;

    public int xIndex;  //������ x��ǥ
    public int yIndex;  //������ y��ǥ

    public bool isMatched;  //������ ���� �ȿ� �ִ��� Ȯ��
    private Vector2 currentPos; //���� ���� ��ġ
    private Vector2 targetPos;  //������ �̵��� ��ġ

    public bool isMoving;   //������ �̵������� Ȯ��
    

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

    //MoveToTarget

    //MoveCoroutine
}

public enum PotionType  //���� ����
{
    Red,
    Blue,
    Pink,
    Green,
    White
}
