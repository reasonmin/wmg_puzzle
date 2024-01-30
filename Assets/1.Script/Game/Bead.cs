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

public class Bead : Singleton<Bead>
{
    [SerializeField] private List<Sprite> sprite;

    public BeadType type; //����

    public Vector2 clampVec2;   //�̵� ����

    Vector2 directionVector = Vector2.zero;

    Vector2 startPos = new();   //���� ��ġ(������ ��)
    Vector2 endPos = new(); //�� ��ġ(���� ��)

    [HideInInspector] public Collider2D target = null; //���� ���� ����

    [HideInInspector] public bool isMatched;  //������ ���� �ȿ� �ִ��� Ȯ��

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
                Vector2 diff = vec - startPos;

                if (distanceY < 0.3f)
                {
                    if (diff.normalized.x > 0 && (diff.normalized.y > -0.5f  //������
                        && diff.normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        directionVector = Vector2.right;
                    }
                    else if (diff.normalized.x < 0 && (diff.normalized.y > -0.5f    //����
                        && diff.normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        directionVector = Vector2.left;
                    }
                }

                if(distanceX < 0.3f)
                {
                    if (diff.normalized.y > 0 && (diff.normalized.x > -0.5f    //����
                        && diff.normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        directionVector = Vector2.up;
                    }
                    else if (diff.normalized.y < 0 && (diff.normalized.x > -0.5f    // �Ʒ�
                        && diff.normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        directionVector = Vector2.down;
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
                BoardManager.Instance.ChangeBead(directionVector);
                //BoardManager.Instance.BeadBoradCheck();
                //��ġ�ϴ� �׸��� ���ٸ� �̵� �� ������ �� ���·� �ǵ�����
            }

            transform.localPosition = Vector2.zero;
            target = null;
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

    public void SetBead()   //�������� type(sprite) �����ֱ�
    {
        type = (BeadType)Random.Range(0, (int)BeadType.Dark + 1);
        GetComponent<SpriteRenderer>().sprite = sprite[(int)type];
    }
}