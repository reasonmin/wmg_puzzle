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
    public PotionType potionType;

    [SerializeField] private List<Sprite> sprite;


    Vector2 startPos = new();
    Vector2 endPos = new();
    Collider2D target = null;

    public Vector2 clampVec2;
    private bool isMoving = false;
    string direction;
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))    //마우스를 눌렀을 때
        {
            if (GetHit2D().collider != null)
            {
                target = GetHit2D().collider;
                startPos = transform.position;
                isMoving = true;
            }
        }

        if (Input.GetMouseButton(0))    //마우스를 누르고 있을 때
        {
            if (target != null)
            {
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                vec = Camera.main.ScreenToWorldPoint(vec);

                // 위치 제한을 위해 Mathf.Clamp 사용
                float clampedX = Mathf.Clamp(vec.x,  -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y, -(clampVec2.y), clampVec2.y);

                //transform.position과 startPos 사이의 거리 계산
                float distanceX = Mathf.Abs(transform.position.x - startPos.x);
                float distanceY = Mathf.Abs(transform.position.y - startPos.y);

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

                //Debug.Log((vec - startPos).normalized);
            }
        }

        if (Input.GetMouseButtonUp(0))  //마우스를 눌렀다가 놓았을 때
        {
            endPos = transform.position;

            //endPos와 startPos 사이의 거리 계산
            float distanceX = Mathf.Abs(endPos.x - startPos.x);
            float distanceY = Mathf.Abs(endPos.y - startPos.y);

            if (distanceX > 1 || distanceY > 1)
            {
                SetBeadSprite(direction);
            }
            transform.position = startPos;

            isMoving = false;
            if (!isMoving)
            {
                target = null;
            }
        }
    }
    public Bead SetBeadSprite(string direction)
    {
        // 이동한 방향에 있는 게임 오브젝트의 스프라이트 가져오기
        GameObject targetObject = GetTargetObjectInDirection(direction);
        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;

            targetObject.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = targetSprite;
        }

        return this;
    }

    private GameObject GetTargetObjectInDirection(string direction)
    {
        // 이동한 방향에 따라 탐색할 방향 벡터 설정
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

        // 이동한 방향에 있는 게임 오브젝트 탐색
        RaycastHit2D hit = Physics2D.Raycast(transform.localPosition, directionVector);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            GameObject targetObject = hit.collider.gameObject;
            Debug.Log(targetObject.GetComponent<Bead>().potionType);
            return targetObject;
        }

        return null;    //게임 오브젝트를 찾지 못 했을 때 null를 반환
    }


    RaycastHit2D GetHit2D()
    {
        // 현재 마우스 위치를 스크린 좌표로부터 레이로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이캐스트를 통해 레이와 충돌한 객체에 대한 정보 저장
        return Physics2D.Raycast(ray.origin, ray.direction);
    }
}
