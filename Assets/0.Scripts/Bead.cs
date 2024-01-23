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
    //�ڽ�
    public static Bead Instance;

    [SerializeField] private List<Sprite> sprite;

    public BeadType beadType; //����
    public Vector2 clampVec2;   //�̵� ����

    Vector2 startPos = new();   //���� ��ġ(������ ��)
    Vector2 endPos = new(); //�� ��ġ(���� ��)

    Collider2D target = null; //���� ���� ����
    private bool isMoving = false;  //������ �̵������� Ȯ��(true : �̵� ��, false : �̵� �� �ƴ�)

    string direction;   //������ �̵��� ����
    
    //------------------------------------
    public int xIndex;  //������ x��ǥ
    public int yIndex;  //������ y��ǥ
    //------------------------------------

    private void Awake()
    {
        Instance = this;
    }

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

                //Debug.Log($"{vec.x}, {vec.y}");
                //Debug.Log($"{transform.position.x}, {transform.position.y}");
                // ��ġ ������ ���� Mathf.Clamp ���
                float clampedX = Mathf.Clamp(vec.x - transform.parent.transform.position.x,  -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y - transform.parent.transform.position.y, -(clampVec2.y), clampVec2.y);
                Debug.Log($"{gameObject.name}");
                Debug.Log($"{vec.x - transform.parent.transform.position.x}, {vec.y - transform.parent.transform.position.y}");
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

                //Debug.Log((vec - startPos).normalized);
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
                SetBeadSprite(direction);
            }
            transform.localPosition = Vector2.zero;

            isMoving = false;
            if (!isMoving)
            {
                target = null;
            }
        }
    }
    public Bead SetBeadSprite(string direction)
    {
        // �̵��� ���⿡ �ִ� ���� ������Ʈ�� ��������Ʈ ��������
        GameObject targetObject = GetTargetObjectInDirection(direction);
        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().beadType;

            //��������Ʈ ����
            targetObject.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //���� Ÿ�� ����
            targetObject.GetComponent<Bead>().beadType = beadType;
            gameObject.GetComponent<Bead>().beadType = targetBeadType;
        }

        return this;
    }

    private GameObject GetTargetObjectInDirection(string direction)
    {
        // �̵��� ���⿡ ���� Ž���� ���� ���� ����
        Vector2 directionVector = Vector2.zero;
        switch (direction)
        {
            case "right":
                directionVector = Vector2.right;
                break;
            case "left":
                directionVector = Vector2.left;
                break;
            case "up":
                directionVector = Vector2.up;
                break;
            case "down":
                directionVector = Vector2.down;
                break;
        }

        // �̵��� ���⿡ �ִ� ���� ������Ʈ Ž��
        RaycastHit2D hit = Physics2D.Raycast(transform.localPosition, directionVector);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            GameObject targetObject = hit.collider.gameObject;
            Debug.Log(targetObject.GetComponent<Bead>().beadType);
            return targetObject;
        }

        return null;    //���� ������Ʈ�� ã�� �� ���� �� null�� ��ȯ
    }


    RaycastHit2D GetHit2D()
    {
        // ���� ���콺 ��ġ�� ��ũ�� ��ǥ�κ��� ���̷� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ����ĳ��Ʈ�� ���� ���̿� �浹�� ��ü�� ���� ���� ����
        return Physics2D.Raycast(ray.origin, ray.direction);
    }
}
