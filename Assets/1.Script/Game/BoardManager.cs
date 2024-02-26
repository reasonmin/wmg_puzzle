using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : Singleton<BoardManager>
{
    //보드의 크기 정하기
    [SerializeField] private int width;  //가로
    [SerializeField] private int height; //세로

    [SerializeField] private float imageSizeX;
    [SerializeField] private float imageSizeY;

    [SerializeField] private GameObject beadBG;
    [SerializeField] private Bead bead;

    private Bead[,] beads;

    void Start()
    {
        beads = new Bead[height, width];

        CreateBeadBG();
        CreateBead();

        BeadBoardCheck();
    }

    /// <summary>
    /// 구슬의 부모생성
    /// </summary>
    void CreateBeadBG()
    {
        for (int y = 0; y < height; y++)
        {
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
        int t = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Bead b = Instantiate(bead, transform.GetChild(t));
                b.SetBead(Random.Range(0, (int)BeadType.Dark + 1));
                beads[i, j] = b;
                t++;
            }
        }
    }

    /// <summary>
    /// 가로 체크
    /// </summary>
    void RowCheck(ref List<List<bool>> check)
    {
        for (int i = 0; i < height; i++)
        {
            int checkCnt = 0;
            for (int j = 0; j < width; j++)
            {
                BeadType bType = beads[i, j].Type;
                if (j + 1 < width && bType == beads[i, j + 1].Type)
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
    }

    /// <summary>
    /// 세로 체크
    /// </summary>
    void ColCheck(ref List<List<bool>> check)
    {
        for (int i = 0; i < width; i++)
        {
            int checkCnt = 0;
            for (int j = 0; j < height; j++)
            {
                BeadType bType = beads[j, i].Type;
                if (j + 1 < height && bType == beads[j + 1, i].Type)
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
    }

    /// <summary>
    /// 상호작용이 가능한 구슬이 있는지 체크
    /// </summary>
    public void BeadBoardCheck()
    {
        List<List<bool>> check = new List<List<bool>>();

        for (int i = 0; i < height; i++)
        {
            check.Add(new List<bool>());
            for (int j = 0; j < width; j++)
            {
                check[i].Add(false);
            }
        }

        // 가로 체크
        RowCheck(ref check);

        // 세로 체크
        ColCheck(ref check);

        bool isRefresh = false;
        // 체크된것 전부 비활성화
        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    isRefresh = true;
                    beads[i, j].gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            if (beads[0, i].gameObject.activeInHierarchy == false)  //activeInHierarchy가 꺼져있을 때 동작
            {
                beads[0, i].gameObject.SetActive(true);
                beads[0, i].SetBead(Random.Range(0, (int)BeadType.Dark + 1));

                beads[0, i].transform.localPosition = new Vector2(0, 1.25f);
                beads[0, i].transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.Linear);
            }
        }

        StartCoroutine(BeadDown(isRefresh));
    }

    #region 구슬 교환
    /// <summary>
    /// 빈자리 구슬을 있는것과 데이터 교체
    /// </summary>
    IEnumerator BeadDown(bool isRefresh)
    {
        float speed = 0.2f;

        bool isChange = false;
        for (int i = height - 2; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                if (beads[i, j].gameObject.activeInHierarchy == true &&
                    beads[i + 1, j].gameObject.activeInHierarchy == false)
                {
                    //yield return new WaitForSeconds(0.1f);
                    isChange = true;

                    // 속성 교체
                    BeadType type = beads[i, j].Type;
                    beads[i, j].Type = beads[i + 1, j].Type;
                    beads[i + 1, j].Type = type;

                    // 다음것은 켬, 내것은 끔
                    beads[i, j].gameObject.SetActive(false);
                    beads[i + 1, j].gameObject.SetActive(true);

                    beads[i + 1, j].transform.localPosition = new Vector2(0, 1.25f);
                    beads[i + 1, j].transform.DOLocalMoveY(0, speed).SetEase(Ease.Linear);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            if (beads[0, i].gameObject.activeInHierarchy == false)  //activeInHierarchy가 꺼져있을 때 동작
            {
                beads[0, i].gameObject.SetActive(true);
                beads[0, i].SetBead(Random.Range(0, (int)BeadType.Dark + 1));

                beads[0, i].transform.localPosition = new Vector2(0, 1.25f);
                beads[0, i].transform.DOLocalMoveY(0, speed).SetEase(Ease.Linear);
            }
        }

        if (isChange == true)
        {
            yield return new WaitForSeconds(speed);
            StartCoroutine(BeadDown(isRefresh));
        }
        else if(isRefresh)
        {
            Debug.Log("End");
            BeadBoardCheck();
        }
    }

    public bool IsMoveCheck()
    {
        List<List<bool>> check = new List<List<bool>>();

        for (int i = 0; i < height; i++)
        {
            check.Add(new List<bool>());
            for (int j = 0; j < width; j++)
            {
                check[i].Add(false);
            }
        }

        // 가로 체크
        RowCheck(ref check);

        // 세로 체크
        ColCheck(ref check);

        // 체크된것 전부 비활성화
        bool isCheck = false;
        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    isCheck = true;
                }
            }
        }

        return isCheck;
    }

    /// <summary>
    /// Bead Type, Sprite 변경
    /// </summary>
    public void ChangeBead(Bead bead, Vector2 dir)
    {
        int x = -1;
        int y = -1;
        for (int i = 0; i <= height; i++) //세로
        {
            for (int j = 0; j <= width; j++) //가로
            {
                if (beads[j, i].Equals(bead) == true)
                {
                    x = i;
                    y = j;
                    break;
                }
            }

            if (x != -1 && y != -1)
            {
                break;
            }
        }

        int nextY = y;
        int nextX = x;

        if (dir == Vector2.up)
            nextY = y - 1;
        else if (dir == Vector2.down)
            nextY = y + 1;
        else if (dir == Vector2.left)
            nextX = x - 1;
        else if (dir == Vector2.right)
            nextX = x + 1;

        Bead nextBead = beads[nextY, nextX];
        SwapBeads(bead, nextBead);
        if (IsMoveCheck() == false)
        {
            SwapBeads(bead, nextBead);
        }
        //비드를 교환하는 함수
        void SwapBeads(Bead bead1, Bead bead2)
        {
            BeadType targetBeadType = bead1.Type;
            bead.Type = bead2.Type;
            bead2.Type = targetBeadType;
        }
    }
    #endregion
}