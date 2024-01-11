using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }
    //MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;    //물약이 이동하는데 걸리는 시간

        Vector2 startPosition = transform.position; //시작 위치
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            //transform.position = Vector2.Lerp(startPosition, targetPos, t);
            transform.position = Vector2.MoveTowards(transform.position, _targetPos, 1f * Time.deltaTime);   //이동 애니매이션


            elaspedTime += Time.deltaTime;

            yield return null;  //5분 20초
        }

        transform.position = _targetPos; //위치 이동
        isMoving = false;
    }
}

public enum PotionType  //물약 종류
{
    Red,
    Blue,
    Pink,
    Green,
    White
}
