using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Potion : MonoBehaviour
{
    public PotionType potionType;

    public int xIndex;  //������ x��ǥ
    public int yIndex;  //������ y��ǥ

    public bool isMatched;  //������ ���� �ȿ� �ִ��� Ȯ��

    public bool isMoving;   //������ �̵������� Ȯ��(true : �̵� ��, false : �̵� �� �ƴ�)


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

    public void MoveToTarget(Vector2 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }
    
    private IEnumerator MoveCoroutine(Vector2 targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;    //������ �̵��ϴµ� �ɸ��� �ð�

        Vector2 startPosition = transform.position; //���� ��ġ
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition; //��ġ �̵�
        isMoving = false;
    }
}

public enum PotionType  //���� ����
{
    Red,
    Blue,
    Pink,
    Green,
    White
}
