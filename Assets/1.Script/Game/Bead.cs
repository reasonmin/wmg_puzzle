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
    Dark,
    None
}

public enum SpecialBT
{
    Normal,
    Three,
    Four,
    Five
}

public class Bead : MonoBehaviour
{
    [SerializeField] private Sprite[] _NormalSprite;
    [SerializeField] private Sprite[] _FourSprite;
    [SerializeField] private Sprite _FiveSprite;

    public SpecialBT stype;
    // ����
    public BeadType type;
    public BeadType Type
    {
        get { return type; }
        set
        {
            type = value;

            switch (stype)
            {
                case SpecialBT.Normal:
                    GetComponent<SpriteRenderer>().sprite = _NormalSprite[(int)type];
                    break;
                case SpecialBT.Three:
                    break;
                case SpecialBT.Four:
                    GetComponent<SpriteRenderer>().sprite = _FourSprite[(int)type];
                    break;
                case SpecialBT.Five:
                    GetComponent<SpriteRenderer>().sprite = _FiveSprite;
                    type = BeadType.None;
                    break;
                default:
                    break;
            }
        }
    }

    public Vector2 clampVec2;   //�̵� ����

    Vector2 directionVector = Vector2.zero;

    Vector2 startPos = new();   //���� ��ġ(������ ��)
    Vector2 endPos = new(); //�� ��ġ(���� ��)

    [HideInInspector] public Collider2D target = null; //���� ���� ����

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //���콺�� ������ ��
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                target.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
                float clampedX = Mathf.Clamp(vec.x - transform.parent.transform.position.x, -(clampVec2.x), clampVec2.x);
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

                if (distanceX < 0.3f)
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
                BoardManager.Instance.ChangeBead(this, directionVector);
                transform.localPosition = Vector2.zero;
                BoardManager.Instance.BeadBoardCheck();

                //��ġ�ϴ� �׸��� ���ٸ� �̵� �� ������ �� ���·� �ǵ�����
            }
            else
                transform.localPosition = Vector2.zero;

            if (target != null)
                target.GetComponent<SpriteRenderer>().sortingOrder = 0;

            target = null;
        }
    }

    RaycastHit2D GetHit2D()
    {
        // ���� ���콺 ��ġ�� ��ũ�� ��ǥ�κ��� ���̷� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ����ĳ��Ʈ�� ���� ���̿� �浹�� ��ü�� ���� ���� ����
        return Physics2D.Raycast(ray.origin, ray.direction);
    }

    public void SetBead(int rand, SpecialBT specialBT)   //�������� type(sprite) �����ֱ�
    {
        type = (BeadType)rand;
        stype = specialBT;
        Type = type;
    }
}