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

    public bool isMoving;   //물약이 이동중인지 확인(true : 이동 중, false : 이동 중 아님)


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

    public void MoveToTarget(Vector2 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }
    
    private IEnumerator MoveCoroutine(Vector2 targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;    //물약이 이동하는데 걸리는 시간

        Vector2 startPosition = transform.position; //시작 위치
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition; //위치 이동
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
