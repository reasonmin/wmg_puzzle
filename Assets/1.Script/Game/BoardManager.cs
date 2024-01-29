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
                Vector2 position = new Vector2(x * ((imageSizeX / 100f)), y * ((imageSizeY / 100f))); //구슬이 생성될 위치

                // 구슬 배경 미리 생성
                Instantiate(beadBG, position, Quaternion.identity)
                    .transform.SetParent(transform);
            }
        }

        // 부모의 보드 위치 수정
        transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));
    }

    void CreateBead()  //물약 생성
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
            Debug.Log("일치하는 항목이 없습니다, 보드를 다시 만듬니다.");
            potionParent.transform.position = Vector2.zero;
            InitializeBoard();
        }
        */
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
    /// 구슬 바꾸기
    /// </summary>
    public void ChangeBead()
    {
        
    }
}
