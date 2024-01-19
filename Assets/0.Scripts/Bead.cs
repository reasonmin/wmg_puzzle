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
        if (Input.GetMouseButtonDown(0))    //마우스를 눌렀을 때
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

        if (Input.GetMouseButton(0))    //마우스를 누르고 있을 때
        {
            if (target != null)
            {
                Debug.Log(target);
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Debug.Log(vec);
                vec = Camera.main.ScreenToWorldPoint(vec);
                // 위치 제한을 위해 Mathf.Clamp 사용
                float clampedX = Mathf.Clamp(vec.x, -(clampVec2.x), clampVec2.x);
                float clampedY = Mathf.Clamp(vec.y, -(clampVec2.y), clampVec2.y);
                vec = new Vector2(clampedX, clampedY);

                if((vec - startPos).normalized.x > 0 && ((vec - startPos).normalized.y > -0.5f &&   //오른쪽
                    (vec - startPos).normalized.y < 0.5f))
                {
                    target.transform.position = new Vector2(vec.x, transform.position.y);
                    //Debug.Log("오른쪽으로 이동");
                }
                else if ((vec - startPos).normalized.x < 0 && ((vec - startPos).normalized.y > -0.5f && //왼쪽
                    (vec - startPos).normalized.y < 0.5f))
                {
                    target.transform.position = new Vector2(vec.x, transform.position.y);
                    //Debug.Log("왼쪽으로 이동");
                }
                else if ((vec - startPos).normalized.y > 0 && ((vec - startPos).normalized.x > -0.5f && //위쪽
                    (vec - startPos).normalized.x < 0.5f))
                {
                    target.transform.position = new Vector2(transform.position.x, vec.y);
                    //Debug.Log("위쪽으로 이동");
                }
                else if ((vec - startPos).normalized.y < 0 && ((vec - startPos).normalized.x > -0.5f // 아래
                    && (vec - startPos).normalized.x < 0.5f))
                {
                    target.transform.position = new Vector2(transform.position.x, vec.y);
                    //Debug.Log("아래쪽으로 이동");
                }

                //Debug.Log((vec - startPos).normalized);
            }
        }

        if (Input.GetMouseButtonUp(0))  //마우스를 눌렀다가 놓았을 때
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
        // 현재 마우스 위치를 스크린 좌표로부터 레이로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이캐스트를 통해 레이와 충돌한 객체에 대한 정보 저장
        return Physics2D.Raycast(ray.origin, ray.direction); ;
    }

    public Bead SetBeadSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = sprite[index];

        return this;
    }
}
