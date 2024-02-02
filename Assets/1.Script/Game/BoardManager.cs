using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    //보드의 크기 정하기
    [SerializeField] private int width;  //가로
    [SerializeField] private int height; //세로

    [SerializeField] private float imageSizeX;
    [SerializeField] private float imageSizeY;

    [SerializeField] private GameObject beadBG;
    [SerializeField] private Bead bead;

    //private Node[,] beadBoard; //물약 보드(2차원 배열)

    private List<List<Bead>> beads = new List<List<Bead>>();

    void Start()
    {
        CreateBeadBG();
        CreateBead();
    }

    /// <summary>
    /// 구슬의 부모생성...
    /// </summary>
    void CreateBeadBG()
    {
        for (int y = 0; y < height; y++)
        {
            beads.Add(new List<Bead>());
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX / 100f)), y * ((imageSizeY / 100f))); //구슬이 생성될 위치

                // 구슬 배경 미리 생성
                Instantiate(beadBG, position, Quaternion.identity)
                    .transform.SetParent(transform);
            }
        }
        // 부모의 보드 위치 수정
        transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));
    }

    /// <summary>
    /// 각자 구슬 생성
    /// </summary>
    void CreateBead()  //물약 생성
    {
        int y = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Bead b = Instantiate(bead, transform.GetChild(i));
            b.SetBead();
            beads[y].Add(b);
            if((i + 1) % width == 0)
                y++;
        }
        //일차하는 항목이 있는지 확인 한 다음 없다면 다시 생성
    }

    /// <summary>
    /// 상호작용이 가능한 구슬이 있는지 체크
    /// </summary>
    public void BeadBoradCheck()
    {
        List<List<bool>> check = new List<List<bool>>();

        for (int i = 0; i < beads.Count; i++)
        {
            check.Add(new List<bool>());
            int checkCnt = 0;

            for (int j = 0; j < beads[i].Count; j++)
            {
                check[i].Add(false);
                BeadType bType = beads[i][j].type;
                if (j + 1 < beads[i].Count && bType == beads[i][j + 1].type)
                {
                    checkCnt++;
                }
                else
                {
                    if (checkCnt >= 2)
                    {
                        for (int delcnt = j; delcnt >= j - checkCnt; delcnt--)
                        {
                            check[i][delcnt] = true;
                        }
                    }
                    checkCnt = 0;
                }
            }
        }

        /*// 세로 방향 계산
        for (int j = 0; j < beads[0].Count; j++)
        {
            int checkCnt = 0;
            for (int i = 0; i < beads.Count; i++)
            {
                BeadType bType = beads[i][j].type;
                if (i + 1 < beads.Count  && bType == beads[i + 1][j].type)
                {
                    checkCnt++;
                }
                else
                {
                    if (checkCnt >= 2)
                    {
                        for (int delcnt = i; delcnt >= i - checkCnt; delcnt--)
                        {
                            Destroy(beads[delcnt][j].gameObject);
                        }
                    }
                    checkCnt = 0;
                }
            }
        }*/

        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    Destroy(beads[i][j].gameObject);
                    beads[i][j] = null;
                }
                // Debug.Log(beads[i][j] + ", " + check[i][j] + ", " + i + ", " + j);
            }
        }
    }

    #region 구슬 교환

    public void TargetBead(Bead b)
    {
        Bead findBead = null;
        foreach (var yBead in beads)
        {
            foreach (var xBead in yBead)
            {
                if(xBead.Equals(b))
                {
                    findBead = xBead;
                    break;
                }
            }
            if (findBead != null)
                break;
        }

        if(findBead != null)
        {
            Destroy(findBead);
            Sprite targetSprite = findBead.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = findBead.GetComponent<Bead>().type;

            //스프라이트 변경
            findBead.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //타입 변경
            findBead.GetComponent<Bead>().type = Bead.Instance.target.GetComponent<Bead>().type;
            Bead.Instance.target.GetComponent<Bead>().type = targetBeadType;
        }
    }

    public Bead ChangeBead(Vector2 directionVector) //target의 beads를 갖고 오는게 더 효율적임
    {
        // 이동한 방향에 있는 게임 오브젝트 가져오기
        GameObject targetObject = GetTargetObjectDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().type;

            //스프라이트 변경
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //타입 변경
            targetObject.GetComponent<Bead>().type = Bead.Instance.target.GetComponent<Bead>().type;
            Bead.Instance.target.GetComponent<Bead>().type = targetBeadType;
        }
        return null;
    }

    private GameObject GetTargetObjectDirection(Vector2 directionVector)
    {
        Vector2 startPosition = Bead.Instance.target.transform.position;

        float raycastDistance = 1f; // 레이캐스트의 최대 거리 설정

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPosition, directionVector.normalized, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != Bead.Instance.target.gameObject)
            {
                GameObject targetObject = hit.collider.gameObject;
                return targetObject;
            }
        }
        return null;    //게임 오브젝트를 찾지 못 했을 때 null를 반환
    }
    #endregion
}