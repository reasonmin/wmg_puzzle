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
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }
    //MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;    //������ �̵��ϴµ� �ɸ��� �ð�

        Vector2 startPosition = transform.position; //���� ��ġ
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            //transform.position = Vector2.Lerp(startPosition, targetPos, t);
            transform.position = Vector2.MoveTowards(transform.position, _targetPos, 1f * Time.deltaTime);   //�̵� �ִϸ��̼�


            elaspedTime += Time.deltaTime;

            yield return null;  //5�� 20��
        }

        transform.position = _targetPos; //��ġ �̵�
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
