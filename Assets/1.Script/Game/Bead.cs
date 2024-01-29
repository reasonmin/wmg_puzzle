using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BeadType
{
    Fire,
    Ice,
    Heal,
    Light,
    Dark
}

public class Bead : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprite;

    public BeadType type; //����

    public Vector2 clampVec2;   //�̵� ����

    Vector2 startPos = new();   //���� ��ġ(������ ��)
    Vector2 endPos = new(); //�� ��ġ(���� ��)

    [HideInInspector] public Collider2D target = null; //���� ���� ����

    [HideInInspector] public bool isMoving;  //������ �̵������� Ȯ��(true : �̵� ��, false : �̵� �� �ƴ�)

    [HideInInspector] public bool isMatched;  //������ ���� �ȿ� �ִ��� Ȯ��

    string direction;   //������ �̵��� ����
    
    //------------------------------------
    [HideInInspector] public int xIndex;  //������ x��ǥ
    [HideInInspector] public int yIndex;  //������ y��ǥ
    //------------------------------------

    public bool isUsable;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //���콺�� ������ ��
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                startPos = Vector2.zero;
                isMoving = true;
            }
        }

        if (Input.GetMouseButton(0))    //���콺�� ������ ���� ��
        {
            if (target == GetComponent<Collider2D>())
            {
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                vec = Camera.main.ScreenToWorldPoint(vec);

                // ��ġ ������ ���� Mathf.Clamp ���
                float clampedX = Mathf.Clamp(vec.x - transform.parent.transform.position.x,  -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y - transform.parent.transform.position.y, -(clampVec2.y), clampVec2.y);

                //transform.position�� startPos ������ �Ÿ� ���
                float distanceX = Mathf.Abs(transform.localPosition.x - startPos.x);
                float distanceY = Mathf.Abs(transform.localPosition.y - startPos.y);

                vec = new Vector2(clampedX, clampedY);

                if (distanceY < 0.3f)
                {
                    if ((vec - startPos).normalized.x > 0 && ((vec - startPos).normalized.y > -0.5f  //������
                        && (vec - startPos).normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);   //transform.position.y
                        direction = "right";
                    }
                    else if ((vec - startPos).normalized.x < 0 && ((vec - startPos).normalized.y > -0.5f    //����
                        && (vec - startPos).normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        direction = "left";
                    }
                }

                if(distanceX < 0.3f)
                {
                    if ((vec - startPos).normalized.y > 0 && ((vec - startPos).normalized.x > -0.5f    //����
                        && (vec - startPos).normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y); //transform.position.x
                        direction = "up";
                    }
                    else if ((vec - startPos).normalized.y < 0 && ((vec - startPos).normalized.x > -0.5f    // �Ʒ�
                        && (vec - startPos).normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        direction = "down";
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))  //���콺�� �����ٰ� ������ ��
        {
            endPos = transform.localPosition;

            //endPos�� startPos ������ �Ÿ� ���
            float distanceX = Mathf.Abs(endPos.x - startPos.x);
            float distanceY = Mathf.Abs(endPos.y - startPos.y);

            if (distanceX > 0.7f || distanceY > 0.7f)
            {
                /*
                PotionBoard.Instance.SetBeadSprite(direction);

                if (PotionBoard.Instance.CheckBoard(false) == true)
                {
                    Debug.Log("test");
                }
                */
                BoardManager.Instance.ChangeBead();
                BoardManager.Instance.BeadBoradCheck();
            }

            transform.localPosition = Vector2.zero;

            isMoving = false;
            if (!isMoving)
            {
                target = null;
            }
        }
    }

    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    RaycastHit2D GetHit2D()
    {
        // ���� ���콺 ��ġ�� ��ũ�� ��ǥ�κ��� ���̷� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ����ĳ��Ʈ�� ���� ���̿� �浹�� ��ü�� ���� ���� ����
        return Physics2D.Raycast(ray.origin, ray.direction);
    }

    public void SetBead()
    {
        type = (BeadType)Random.Range(0, (int)BeadType.Dark + 1);
        GetComponent<SpriteRenderer>().sprite = sprite[(int)type];
    }
}
