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

    // 종류
    private BeadType type;
    public BeadType Type
    {
        get { return type; }
        set
        {
            type = value;
            GetComponent<SpriteRenderer>().sprite = sprite[(int)type];
        }
    }

    public Vector2 clampVec2;   //이동 범위

    Vector2 directionVector = Vector2.zero;

    Vector2 startPos = new();   //시작 위치(눌렀을 때)
    Vector2 endPos = new(); //끝 위치(놨을 때)

    [HideInInspector] public Collider2D target = null; //내가 누른 구슬

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //마우스를 눌렀을 때
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                startPos = Vector2.zero;
            }
        }

        if (Input.GetMouseButton(0))    //마우스를 누르고 있을 때
        {
            if (target == GetComponent<Collider2D>())
            {
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                vec = Camera.main.ScreenToWorldPoint(vec);

                // 위치 제한을 위해 Mathf.Clamp 사용
                float clampedX = Mathf.Clamp(vec.x - transform.parent.transform.position.x,  -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y - transform.parent.transform.position.y, -(clampVec2.y), clampVec2.y);

                //transform.position과 startPos 사이의 거리 계산
                float distanceX = Mathf.Abs(transform.localPosition.x - startPos.x);
                float distanceY = Mathf.Abs(transform.localPosition.y - startPos.y);

                vec = new Vector2(clampedX, clampedY);
                Vector2 diff = vec - startPos;

                if (distanceY < 0.3f)
                {
                    if (diff.normalized.x > 0 && (diff.normalized.y > -0.5f  //오른쪽
                        && diff.normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        directionVector = Vector2.right;
                    }
                    else if (diff.normalized.x < 0 && (diff.normalized.y > -0.5f    //왼쪽
                        && diff.normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        directionVector = Vector2.left;
                    }
                }

                if(distanceX < 0.3f)
                {
                    if (diff.normalized.y > 0 && (diff.normalized.x > -0.5f    //위쪽
                        && diff.normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        directionVector = Vector2.up;
                    }
                    else if (diff.normalized.y < 0 && (diff.normalized.x > -0.5f    // 아래
                        && diff.normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        directionVector = Vector2.down;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))  //마우스를 눌렀다가 놓았을 때
        {
            endPos = transform.localPosition;

            //endPos와 startPos 사이의 거리 계산
            float distanceX = Mathf.Abs(endPos.x - startPos.x);
            float distanceY = Mathf.Abs(endPos.y - startPos.y);

            if (distanceX > 0.7f || distanceY > 0.7f)
            {
                BoardManager.Instance.ChangeBead(this, directionVector);
                transform.localPosition = Vector2.zero;
                BoardManager.Instance.BeadBoardCheck();

                //일치하는 항목이 없다면 이동 한 구슬을 원 상태로 되돌리기
            }
            else
                transform.localPosition = Vector2.zero;
            target = null;
        }
    }

    RaycastHit2D GetHit2D()
    {
        // 현재 마우스 위치를 스크린 좌표로부터 레이로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이캐스트를 통해 레이와 충돌한 객체에 대한 정보 저장
        return Physics2D.Raycast(ray.origin, ray.direction);
    }

    public void SetBead(int rand)   //랜덤으로 type(sprite) 정해주기
    {
        type = (BeadType)rand;
        GetComponent<SpriteRenderer>().sprite = sprite[(int)Type];
    }
}
