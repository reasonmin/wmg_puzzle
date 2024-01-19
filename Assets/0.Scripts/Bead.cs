using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BeadType
{
    Red,
    Blue,
    Pink,
    Green,
    White
}

public class Bead : MonoBehaviour
{
    public PotionType potionType;

    [SerializeField] private List<Sprite> sprite;


    Vector2 startPos = new();
    Collider2D target = null;

    public Vector2 clampVec2;
    private bool isMoving = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //���콺�� ������ ��
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                startPos = transform.position;
                isMoving = true;
                Debug.Log("startPos : " + startPos);
                Debug.Log("potionType : " + potionType);
            }
        }

        if (Input.GetMouseButton(0))    //���콺�� ������ ���� ��
        {
            if (target != null)
            {
                Debug.Log(target);
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Debug.Log(vec);
                vec = Camera.main.ScreenToWorldPoint(vec);
                // ��ġ ������ ���� Mathf.Clamp ���
                float clampedX = Mathf.Clamp(vec.x, -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y, -(clampVec2.y), clampVec2.y);
                vec = new Vector2(clampedX, clampedY);

                if((vec - startPos).normalized.x > 0 && ((vec - startPos).normalized.y > -0.5f &&   //������
                    (vec - startPos).normalized.y < 0.5f))
                {
                    target.transform.position = new Vector2(vec.x, transform.position.y);
                    //Debug.Log("���������� �̵�");
                }
                else if ((vec - startPos).normalized.x < 0 && ((vec - startPos).normalized.y > -0.5f && //����
                    (vec - startPos).normalized.y < 0.5f))
                {
                    target.transform.position = new Vector2(vec.x, transform.position.y);
                    //Debug.Log("�������� �̵�");
                }
                else if ((vec - startPos).normalized.y > 0 && ((vec - startPos).normalized.x > -0.5f && //����
                    (vec - startPos).normalized.x < 0.5f))
                {
                    target.transform.position = new Vector2(transform.position.x, vec.y);
                    //Debug.Log("�������� �̵�");
                }
                else if ((vec - startPos).normalized.y < 0 && ((vec - startPos).normalized.x > -0.5f // �Ʒ�
                    && (vec - startPos).normalized.x < 0.5f))
                {
                    target.transform.position = new Vector2(transform.position.x, vec.y);
                    //Debug.Log("�Ʒ������� �̵�");
                }

                //Debug.Log((vec - startPos).normalized);
            }
        }

        if (Input.GetMouseButtonUp(0))  //���콺�� �����ٰ� ������ ��
        {
                transform.position = startPos;
                isMoving = false;
                {
                if (isMoving != true)
                    target = null;

                }
        }
    }

    RaycastHit2D GetHit2D()
    {
        // ���� ���콺 ��ġ�� ��ũ�� ��ǥ�κ��� ���̷� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ����ĳ��Ʈ�� ���� ���̿� �浹�� ��ü�� ���� ���� ����
        return Physics2D.Raycast(ray.origin, ray.direction); ;
    }

    public Bead SetBeadSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = sprite[index];

        return this;
    }
}
