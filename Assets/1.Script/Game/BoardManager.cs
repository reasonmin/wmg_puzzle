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

        BeadType nowBead = beads[y][x].Type;

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
        beads[y][x].Type = nextBead.Type;

        // ���������� �� ���� ���ӵ� ���
        if (m + 2 < width && CheckMatch(beads[w][m + 1], beads[w][m + 2]))
            SwapBeads(nowBead, nextBead);
        // �������� �� ���� ���ӵ� ���
        else if (m - 2 >= 0 && CheckMatch(beads[w][m - 1], beads[w][m - 2]))
            SwapBeads(nowBead, nextBead);
        // ���� �� ���� ���ӵ� ���
        else if (w - 2 >= 0 && CheckMatch(beads[w - 1][m], beads[w - 2][m]))
            SwapBeads(nowBead, nextBead);
        // �Ʒ��� �� ���� ���ӵ� ���
        else if (w + 2 < height && CheckMatch(beads[w + 1][m], beads[w + 2][m]))
            SwapBeads(nowBead, nextBead);
        // ���η� �� ���� ���ӵ� ���
        else if (w - 1 >= 0 && w + 1 < height && CheckMatch(beads[w + 1][m], beads[w - 1][m]))
            SwapBeads(nowBead, nextBead);
        // ���η� �� ���� ���ӵ� ���
        else if (m + 1 < width && m - 1 >= 0 && CheckMatch(beads[w][m + 1], beads[w][m - 1]))
            SwapBeads(nowBead, nextBead);
        else
            beads[y][x].Type = nowBead;


        //��带 ��ȯ�ϴ� �Լ�
        void SwapBeads(BeadType bead1, Bead bead2)
        {
            BeadType targetBeadType = bead1;
            bead.Type = bead2.Type;
            bead2.Type = targetBeadType;
        }

        // �� ���� ������ Ȯ���ϴ� �Լ�
        bool CheckMatch(Bead bead1, Bead bead2)
        {
            return  nowBead == bead1.Type && nowBead == bead2.Type;
        }
    }
    #endregion
}