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
                Vector2 position = new Vector2(x * ((imageSizeX / 100f)), -(y * ((imageSizeY / 100f)))); //구슬이 생성될 위치

                // 구슬 배경 미리 생성
                Instantiate(beadBG, position, Quaternion.identity)
                    .transform.SetParent(transform);
            }
        }
        // 부모의 보드 위치 수정
        transform.position = new Vector2(-3.95f, 1);
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
            b.SetBead(Random.Range(0, (int)BeadType.Dark + 1));
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
            for (int j = 0; j < beads[i].Count; j++)
            {
                check[i].Add(false);
            }
        }

        // 가로 체크
        for (int i = 0; i < beads.Count; i++)
        {
            int checkCnt = 0;
            for (int j = 0; j < beads[i].Count; j++)
            {
                BeadType bType = beads[i][j].Type;
                if (j + 1 < beads[i].Count && bType == beads[i][j + 1].Type)
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

        // 세로 체크
        for (int i = 0; i < width; i++) // 8
        {
            int checkCnt = 0;
            for (int j = 0; j < height - 1; j++)
            {
                BeadType bType = beads[j][i].Type;
                if (i + 1 < beads[j].Count && bType == beads[j + 1][i].Type)
                {
                    checkCnt++;
                }
                else
                {
                    if (checkCnt >= 2)
                    {
                        for (int delcnt = j; delcnt >= j - checkCnt; delcnt--)
                        {
                            check[delcnt][i] = true;
                        }
                    }
                    checkCnt = 0;
                }
            }
        }
        // 체크된것 전부 비활성화
        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    //Destroy(beads[i][j].gameObject);
                    beads[i][j].gameObject.SetActive(false);
                }
            }
        }
        BeadDown();
    }

    #region 구슬 교환
    /// <summary>
    /// 빈자리 구슬을 있는것과 데이터 교체
    /// </summary>
    void BeadDown()
    {
        bool isChange = false;
        for (int i = 0; i < width; i++) // 8
        {
            for (int j = 0; j < height - 1; j++)
            {
                if (beads[j][i].gameObject.activeInHierarchy == true &&
                    beads[j + 1][i].gameObject.activeInHierarchy == false)
                {
                    // 속성 교체
                    BeadType type = beads[j][i].Type;
                    beads[j][i].Type = beads[j + 1][i].Type;
                    beads[j + 1][i].Type = type;

                    // 다음것은 켬, 내것은 끔
                    beads[j][i].gameObject.SetActive(false);
                    beads[j + 1][i].gameObject.SetActive(true);

                    isChange = true;
                }
            }
        }

        if (isChange == true)
        {
            BeadDown();
        }
        else
        {
            // 구슬 데이터 리플레쉬
            for (int i = 0; i < width; i++) // 8
            {
                for (int j = 0; j < height; j++)
                {
                    if (beads[j][i].gameObject.activeInHierarchy == false)
                    {
                        beads[j][i].gameObject.SetActive(true);
                        beads[j][i].SetBead(Random.Range(0, (int)BeadType.Dark + 1));
                    }
                }
            }
        }
    }

    public Bead ChangeBead(Vector2 directionVector) //target의 beads를 갖고 오는게 더 효율적임
    {
        // 이동한 방향에 있는 게임 오브젝트 가져오기
        GameObject targetObject = GetTargetObjectDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().Type;

            //스프라이트 변경
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //타입 변경
            targetObject.GetComponent<Bead>().Type = Bead.Instance.target.GetComponent<Bead>().Type;
            Bead.Instance.target.GetComponent<Bead>().Type = targetBeadType;
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