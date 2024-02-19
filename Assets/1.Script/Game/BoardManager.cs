using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    //������ ũ�� ���ϱ�
    [SerializeField] private int width;  //����
    [SerializeField] private int height; //����

    [SerializeField] private float imageSizeX;
    [SerializeField] private float imageSizeY;

    [SerializeField] private GameObject beadBG;
    [SerializeField] private Bead bead;
    [SerializeField] private Animator animator;

    private List<List<Bead>> beads = new List<List<Bead>>();

    void Start()
    {
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
            beads.Add(new List<Bead>());
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
        int y = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Bead b = Instantiate(bead, transform.GetChild(i));
            b.SetBead(Random.Range(0, (int)BeadType.Dark + 1));
            beads[y].Add(b);

            if ((i + 1) % width == 0)
                y++;
        }
        //�����ϴ� �׸��� �ִ��� Ȯ�� �� ���� ���ٸ� �ٽ� ����
    }

    /// <summary>
    /// ��ȣ�ۿ��� ������ ������ �ִ��� üũ
    /// </summary>
    public void BeadBoardCheck()
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

        // ���� üũ
        for (int i = 0; i < height; i++)
        {
            int checkCnt = 0;
            for (int j = 0; j < width; j++)
            {
                BeadType bType = beads[i][j].Type;
                if (j + 1 < width && bType == beads[i][j + 1].Type)
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

        // ���� üũ
        for (int i = 0; i < width; i++)
        {
            int checkCnt = 0;
            for (int j = 0; j < height; j++)
            {
                BeadType bType = beads[j][i].Type;
                if (j + 1 < height && bType == beads[j + 1][i].Type)
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

        // üũ�Ȱ� ���� ��Ȱ��ȭ
        for (int i = 0; i < check.Count; i++)
        {
            for (int j = 0; j < check[i].Count; j++)
            {
                if (check[i][j])
                {
                    beads[i][j].gameObject.SetActive(false);
                }
            }
        }
        BeadDown();
    }

    #region ���� ��ȯ
    /// <summary>
    /// ���ڸ� ������ �ִ°Ͱ� ������ ��ü
    /// </summary>
    void BeadDown()
    {
        bool isChange = false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 1; j++)
            {
                if (beads[j][i].gameObject.activeInHierarchy == true &&
                    beads[j + 1][i].gameObject.activeInHierarchy == false)
                {
                    animator.SetTrigger("down");

                    // �Ӽ� ��ü
                    BeadType type = beads[j][i].Type;
                    beads[j][i].Type = beads[j + 1][i].Type;
                    beads[j + 1][i].Type = type;

                    // �������� ��, ������ ��
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
            // ���� ������ ���÷���
            bool isReflush = false;
            for (int i = 0; i < width; i++) // 8
            {
                for (int j = 0; j < height; j++)
                {
                    if (beads[j][i].gameObject.activeInHierarchy == false)
                    {
                        isReflush = true;
                        beads[j][i].gameObject.SetActive(true);
                        beads[j][i].SetBead(Random.Range(0, (int)BeadType.Dark + 1));
                    }
                }
            }

            if (isReflush)
                BeadBoardCheck();
        }
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
                if(beads[j][i].Equals(bead) == true)
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

        int w = y;
        int m = x;

        if (dir == Vector2.up)
            w = y - 1;
        else if (dir == Vector2.down)
            w = y + 1;
        else if (dir == Vector2.left)
            m = x - 1;
        else if (dir == Vector2.right)
            m = x + 1;

        Bead nextBead = beads[w][m];
        SwapBeads(bead, nextBead);

        if (!(m + 2 < width && CheckMatch(beads[w][m + 1], beads[w][m + 2]) || x + 2 < width && NextCheckMatch(beads[y][x + 1], beads[y][x + 2]) ||
            m - 2 >= 0 && CheckMatch(beads[w][m - 1], beads[w][m - 2]) || x - 2 >= 0 && NextCheckMatch(beads[y][x - 1], beads[y][x - 2]) ||
            w - 2 >= 0 && CheckMatch(beads[w - 1][m], beads[w - 2][m]) || y - 2 >= 0 && NextCheckMatch(beads[y - 1][x], beads[y - 2][x]) ||
            w + 2 < height && CheckMatch(beads[w + 1][m], beads[w + 2][m]) || y + 2 < height && NextCheckMatch(beads[y + 1][x], beads[y + 2][x]) ||
            w - 1 >= 0 && w + 1 < height && CheckMatch(beads[w + 1][m], beads[w - 1][m]) || y - 1 >= 0 && y + 1 < height && NextCheckMatch(beads[y + 1][x], beads[y - 1][x]) ||
            m + 1 < width && m - 1 >= 0 && CheckMatch(beads[w][m + 1], beads[w][m - 1]) || x + 1 < width && x - 1 >= 0 && NextCheckMatch(beads[y][x + 1], beads[y][x - 1])))
            SwapBeads(bead, nextBead);


        //��带 ��ȯ�ϴ� �Լ�
        void SwapBeads(Bead bead1, Bead bead2)
        {
            BeadType targetBeadType = bead1.Type;
            bead.Type = bead2.Type;
            bead2.Type = targetBeadType;
        }

        // �� ���� ������ Ȯ���ϴ� �Լ�
        bool CheckMatch(Bead bead1, Bead bead2)
        {
            return nextBead.Type == bead1.Type && nextBead.Type == bead2.Type;
        }
        bool NextCheckMatch(Bead bead1, Bead bead2)
        {
            return bead.Type == bead1.Type && bead.Type == bead2.Type;
        }
    }
    #endregion
}