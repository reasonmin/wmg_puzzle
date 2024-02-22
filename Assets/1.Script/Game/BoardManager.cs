using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : Singleton<BoardManager>
{
    //������ ũ�� ���ϱ�
    [SerializeField] private int width;  //����
    [SerializeField] private int height; //����

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
    /// ������ �θ����
    /// </summary>
    void CreateBeadBG()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX / 100f)), -(y * ((imageSizeY / 100f)))); //������ ������ ��ġ

                // ���� ��� �̸� ����
                Instantiate(beadBG, position, Quaternion.identity)
                    .transform.SetParent(transform);
            }
        }
        // �θ��� ���� ��ġ ����
        transform.position = new Vector2(-3.95f, 1);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    void CreateBead()  //���� ����
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
    /// ���� üũ
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
    /// ���� üũ
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
    /// ��ȣ�ۿ��� ������ ������ �ִ��� üũ
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

        // ���� üũ
        RowCheck(ref check);

        // ���� üũ
        ColCheck(ref check);

        // üũ�Ȱ� ���� ��Ȱ��ȭ
        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    beads[i, j].gameObject.SetActive(false);
                }
            }
        }
        StartCoroutine(BeadDown());
    }

    #region ���� ��ȯ
    /// <summary>
    /// ���ڸ� ������ �ִ°Ͱ� ������ ��ü
    /// </summary>
    IEnumerator BeadDown()
    {
        bool isChange = false;
        for (int i = 0; i < width; i++)
        {
            int n = i;
            for (int j = 0; j < height - 1; j++)
            {
                int m = j;
                if (beads[j, i].gameObject.activeInHierarchy == true &&
                    beads[j + 1, i].gameObject.activeInHierarchy == false)
                {
                    beads[j, i].transform.DOLocalMove(new Vector2(0, -1.25f), 1)
                        .OnComplete(() =>
                        {
                            beads[m, n].transform.localPosition = Vector2.zero;
                        });

                    yield return new WaitForSeconds(0.3f);

                    // �Ӽ� ��ü
                    BeadType type = beads[j, i].Type;
                    beads[j, i].Type = beads[j + 1, i].Type;
                    beads[j + 1, i].Type = type;

                    // �������� ��, ������ ��
                    beads[j, i].gameObject.SetActive(false);
                    beads[j + 1, i].gameObject.SetActive(true);

                    isChange = true;
                }
            }
        }

        if (isChange == true)
            StartCoroutine(BeadDown());

        else
        {
            // ���� ������ ���÷���
            bool isReflush = false;
            for (int i = 0; i < width; i++) // 8
            {
                for (int j = 0; j < height; j++)
                {
                    if (beads[j, i].gameObject.activeInHierarchy == false)  //activeInHierarchy�� �������� �� ����
                    {
                        isReflush = true;
                        beads[j, i].gameObject.SetActive(true);
                        beads[j, i].SetBead(Random.Range(0, (int)BeadType.Dark + 1));
                    }
                }
            }

            if (isReflush)
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

        // ���� üũ
        RowCheck(ref check);

        // ���� üũ
        ColCheck(ref check);

        // üũ�Ȱ� ���� ��Ȱ��ȭ
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
    /// Bead Type, Sprite ����
    /// </summary>
    public void ChangeBead(Bead bead, Vector2 dir)
    {
        int x = -1; 
        int y = -1;
        for (int i = 0; i <= height; i++) //����
        {
            for (int j = 0; j <= width; j++) //����
            {
                if(beads[j, i].Equals(bead) == true)
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
        if(IsMoveCheck() == false)
        {
            SwapBeads(bead, nextBead);
        }
        //��带 ��ȯ�ϴ� �Լ�
        void SwapBeads(Bead bead1, Bead bead2)
        {
            BeadType targetBeadType = bead1.Type;
            bead.Type = bead2.Type;
            bead2.Type = targetBeadType;
        }
    }
    #endregion
}