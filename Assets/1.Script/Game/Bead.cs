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

    public BeadType type; //종류

    public Vector2 clampVec2;   //이동 범위

    Vector2 startPos = new();   //시작 위치(눌렀을 때)
    Vector2 endPos = new(); //끝 위치(놨을 때)

    [HideInInspector] public Collider2D target = null; //내가 누른 구슬

    [HideInInspector] public bool isMoving;  //구슬이 이동중인지 확인(true : 이동 중, false : 이동 중 아님)

    [HideInInspector] public bool isMatched;  //물약이 보드 안에 있는지 확인

    string direction;   //구슬이 이동한 방향
    
    //------------------------------------
    [HideInInspector] public int xIndex;  //보드의 x좌표
    [HideInInspector] public int yIndex;  //보드의 y좌표
    //------------------------------------

    public bool isUsable;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //마우스를 눌렀을 때
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                startPos = Vector2.zero;
                isMoving = true;
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

                if (distanceY < 0.3f)
                {
                    if ((vec - startPos).normalized.x > 0 && ((vec - startPos).normalized.y > -0.5f  //오른쪽
                        && (vec - startPos).normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);   //transform.position.y
                        direction = "right";
                    }
                    else if ((vec - startPos).normalized.x < 0 && ((vec - startPos).normalized.y > -0.5f    //왼쪽
                        && (vec - startPos).normalized.y < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(vec.x, startPos.y);
                        direction = "left";
                    }
                }

                if(distanceX < 0.3f)
                {
                    if ((vec - startPos).normalized.y > 0 && ((vec - startPos).normalized.x > -0.5f    //위쪽
                        && (vec - startPos).normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y); //transform.position.x
                        direction = "up";
                    }
                    else if ((vec - startPos).normalized.y < 0 && ((vec - startPos).normalized.x > -0.5f    // 아래
                        && (vec - startPos).normalized.x < 0.5f))
                    {
                        target.transform.localPosition = new Vector2(startPos.x, vec.y);
                        direction = "down";
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
        // 현재 마우스 위치를 스크린 좌표로부터 레이로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이캐스트를 통해 레이와 충돌한 객체에 대한 정보 저장
        return Physics2D.Raycast(ray.origin, ray.direction);
    }

    public void SetBead()
    {
        type = (BeadType)Random.Range(0, (int)BeadType.Dark + 1);
        GetComponent<SpriteRenderer>().sprite = sprite[(int)type];
    }
}
