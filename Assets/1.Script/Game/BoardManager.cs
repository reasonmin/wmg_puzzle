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
    // Start is called before the first frame update
    void Start()
    {
        CreateBeadBG();
        CreateBead();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        /*
        if (CheckBoard(false))
        {
            Debug.Log("��ġ�ϴ� �׸��� �����ϴ�, ���带 �ٽ� ����ϴ�.");
            potionParent.transform.position = Vector2.zero;
            InitializeBoard();
        }
        */
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
            int checkCnt = 0;
            for (int j = 0; j < beads[i].Count - 1; j++)
            {
                BeadType bType = beads[i][j].type;
                if (bType == beads[i][j + 1].type)
                {
                    checkCnt++;
                }
                else
                {
                    if(checkCnt >= 2)
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

    /// <summary>
    /// ���� �ٲٱ�
    /// </summary>
    public void ChangeBead()
    {
        
    }
}
