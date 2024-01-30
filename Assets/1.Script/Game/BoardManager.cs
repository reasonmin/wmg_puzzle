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

    //private Node[,] beadBoard; //���� ����(2���� �迭)

    private List<List<Bead>> beads = new List<List<Bead>>();

    void Start()
    {
        CreateBeadBG();
        CreateBead();
    }

    void CreateBeadBG()
    {
        for (int y = 0; y < height; y++)
        {
            beads.Add(new List<Bead>());
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX / 100f)), y * ((imageSizeY / 100f))); //������ ������ ��ġ

                // ���� ��� �̸� ����
                Instantiate(beadBG, position, Quaternion.identity)
                    .transform.SetParent(transform);
            }
        }
        // �θ��� ���� ��ġ ����
        transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));
    }

    void CreateBead()  //���� ����
    {
        int y = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Bead b = Instantiate(bead, transform.GetChild(i));
            b.SetBead();
            beads[y].Add(b);
            if(y != 0 && i % height == 0)
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

        for (int i = 0; i < beads.Count; i++)   //7
        {
            check.Add(new List<bool>());
            int checkCnt = 0;
            for (int j = 0; j < beads[i].Count - 1; j++)    //55
            {
                BeadType bType = beads[i][j].type;
                if (bType == beads[i][j + 1].type)
                {
                    checkCnt++;
                }
                else
                {
                    if(checkCnt >= 2)   //������ type�� ���� bead�� 3�� �̻� �ִٸ� ����
                    {
                        for (int delcnt = checkCnt; delcnt >= 0; delcnt--)
                        {
                            Destroy(beads[i][j].gameObject);
                        }
                    }
                }
            }
        }
    }

    #region ���� ��ȯ
    public Bead ChangeBead(Vector2 directionVector) //�� ��� ���� target�� beads�� ���� ���°� �� ȿ������
    {
        // �̵��� ���⿡ �ִ� ���� ������Ʈ ��������
        GameObject targetObject = GetTargetObjectInDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().type;

            //��������Ʈ ����
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //Ÿ�� ����
            targetObject.GetComponent<Bead>().type = Bead.Instance.target.GetComponent<Bead>().type;
            Bead.Instance.target.GetComponent<Bead>().type = targetBeadType;
        }
        return null;
    }

    private GameObject GetTargetObjectInDirection(Vector2 directionVector)
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