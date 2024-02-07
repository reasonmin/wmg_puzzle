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
    }

    /// <summary>
    /// ������ �θ����...
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
            if((i + 1) % width == 0)
                y++;
        }
        //�����ϴ� �׸��� �ִ��� Ȯ�� �� ���� ���ٸ� �ٽ� ����
    }

    /// <summary>
    /// ��ȣ�ۿ��� ������ ������ �ִ��� üũ
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

        // ���� üũ
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

        // ���� üũ
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
        // üũ�Ȱ� ���� ��Ȱ��ȭ
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

    #region ���� ��ȯ
    /// <summary>
    /// ���ڸ� ������ �ִ°Ͱ� ������ ��ü
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

    public Bead ChangeBead(Vector2 directionVector) //target�� beads�� ���� ���°� �� ȿ������
    {
        // �̵��� ���⿡ �ִ� ���� ������Ʈ ��������
        GameObject targetObject = GetTargetObjectDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().Type;

            //��������Ʈ ����
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //Ÿ�� ����
            targetObject.GetComponent<Bead>().Type = Bead.Instance.target.GetComponent<Bead>().Type;
            Bead.Instance.target.GetComponent<Bead>().Type = targetBeadType;
        }
        return null;
    }

    private GameObject GetTargetObjectDirection(Vector2 directionVector)
    {
        Vector2 startPosition = Bead.Instance.target.transform.position;

        float raycastDistance = 1f; // ����ĳ��Ʈ�� �ִ� �Ÿ� ����

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPosition, directionVector.normalized, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != Bead.Instance.target.gameObject)
            {
                GameObject targetObject = hit.collider.gameObject;
                return targetObject;
            }
        }
        return null;    //���� ������Ʈ�� ã�� �� ���� �� null�� ��ȯ
    }
    #endregion
}