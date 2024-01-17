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
    [SerializeField] private List<Sprite> sprite;


    Vector2 startPos;
    Collider2D target = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetHit2D().collider != null)
            {
                startPos = transform.position;
                target = GetHit2D().collider;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (target != null)
            {
                Vector2 vec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                vec = Camera.main.ScreenToWorldPoint(vec);
                //transform.position = vec;

                if((vec - startPos).normalized.x > 0 && ((vec - startPos).normalized.y > -0.5f &&   //오른쪽
                    (vec - startPos).normalized.y < 0.5f))
                {
                    transform.position = new Vector2(vec.x, transform.position.y);
                    Debug.Log("오른쪽으로 이동");
                }

                else if ((vec - startPos).normalized.x < 0 && ((vec - startPos).normalized.y > -0.5f && //왼쪽
                    (vec - startPos).normalized.y < 0.5f))
                {
                    transform.position = new Vector2(vec.x, transform.position.y);
                    Debug.Log("왼쪽으로 이동");
                }

                else if ((vec - startPos).normalized.y > 0 && ((vec - startPos).normalized.x > -0.5f && //위쪽
                    (vec - startPos).normalized.x < 0.5f))
                {
                    transform.position = new Vector2(vec.x, transform.position.y);
                    Debug.Log("위쪽으로 이동");
                }

                else if ((vec - startPos).normalized.y < 0 && ((vec - startPos).normalized.x > -0.5f && //아래쪽
                   (vec - startPos).normalized.x < 0.5f))
                {
                    transform.position = new Vector2(vec.x, transform.position.y);
                    Debug.Log("아래쪽으로 이동");
                }

                Debug.Log((vec - startPos).normalized);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            transform.position = startPos;
            target = null;
        }
    }

    RaycastHit2D GetHit2D()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        return hit;
    }

    public Bead SetBeadSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = sprite[index];

        return this;
    }

    public void OnDrag()
    {
        transform.position = Input.mousePosition;
        Debug.Log("OnDrag");
    }

    public void OnDrag(BaseEventData data)
    {
        Debug.Log("Drag");
    }
}
