using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchDirection
{
    Vertical,   //수직
    Horizontal, //수평
    LongVertical,   //긴 수직 일치(3개 이상 일치)
    LongHorizontal, //긴 수평 일치
    Super,  //33
    None    //일치 방향 없음
}

public class PotionBoard : MonoBehaviour
{
    //자신
    public static PotionBoard Instance;

    //보드의 크기 정하기
    public int width = 8;  //가로
    public int height = 7; //세로

    public float imageSizeX;
    public float imageSizeY;


    //물약 프리텝 갖고 오기
    public GameObject beadPrefab;
    

    public GameObject potionParent;

    [HideInInspector] public List<GameObject> potionsToDestroy = new();    //물약 파괴

    //보드의 간격 정하기
    private float spacingX;
    private float spacingY;

    //보드
    public Node[,] beadBoard; //물약 보드(2차원 배열)

    //-----------------------------------------------------
    public List<Bead> beads = new();
    //-----------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()  //물약 생성
    {
        DestroyPotions();
        beadBoard = new Node[width, height];

        //보드 생성(좌측 하단 -> 우측 상단)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX/100f)), y * ((imageSizeY/100f))); //구슬이 생성될 위치

                GameObject bead = Instantiate(beadPrefab, position, Quaternion.identity); //구슬 생성
                bead.transform.SetParent(potionParent.transform);   //potionParent안에 생성

                if (beadBoard != null)
                {
                    beadBoard[x, y] = new Node(true, bead); //물약을 보드에 배치

                }
                potionsToDestroy.Add(bead);
            }
        }

        // 부모의 보드 위치 수정
        potionParent.transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));

        if (CheckBoard(false))
        {
            Debug.Log("일치하는 항목이 없습니다, 보드를 다시 만듬니다.");
            potionParent.transform.position = Vector2.zero;
            InitializeBoard();
        }
    }


    private void DestroyPotions()
    {
        //파괴할 물약이 있을 경우
        if (potionsToDestroy != null)   
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

    public bool CheckBoard(bool _takeAction)    //일치하는 물약이 있는지 확인, 물약 제거(_takeAction = true : 제거, false : 제거 안 함)
    {
        bool hasMatched = false;

        List<Bead> beadsToRemove = new();   //제거할 물약

        if (beadBoard != null)
        {
            foreach (Node nodebead in beadBoard)
            {
                if (nodebead.bead != null)
                {
                    //nodebead.bead.GetComponent<Bead_BG>().bead.GetComponent<Bead>().isMatched = false;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //물약 노드가 사용 가능한지 확인
                if (beadBoard != null)
                {
                    if (beadBoard[x, y].isUsable)
                    {
                        /*
                        Bead bead = beadBoard[x, y].bead.GetComponent<Bead_BG>().bead.GetComponent<Bead>();
                        if (bead != null)
                        {
                            if (bead.isMatched == false)  //물약이 일치하는지 확인
                            {
                                MatchResult matchedBeads = IsConnected(bead);   //일치된 물약(연결되어 있음)

                                if (matchedBeads.connectedBeads.Count >= 3) //3개 이상 일치
                                {
                                    MatchResult superMathedBeads = SuperMatch(matchedBeads);

                                    beadsToRemove.AddRange(superMathedBeads.connectedBeads);

                                    foreach (Bead pot in superMathedBeads.connectedBeads)
                                        pot.isMatched = true;

                                    hasMatched = true;
                                }
                            }
                        }
                        */
                    }
                }
            }
        }

        if (_takeAction == true)
        {
            foreach (Bead beadToRemove in beadsToRemove)    //제거할 물약
            {
                beadToRemove.isMatched = false;
            }

            RemovAndRefill(beadsToRemove);

            if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }

        return hasMatched;
    }

    #region 삭제, 재설치
    private void RemovAndRefill(List<Bead> _potionsToRemove)
    {
        //물약을 제거하고 해당 위치의 보드를 비워줌
        foreach (Bead potion in _potionsToRemove)
        {
            //x, y 인덱스를 임시로 저장
            int _xIndex = potion.xIndex;
            int _yIndex = potion.yIndex;

            //Destoy potion
            Destroy(potion.gameObject);

            //빈 보드 만들기
            beadBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (beadBoard[x, y].bead == null)
                {
                    //Debug.Log($"위치 X :{x} Y : {y} 는 비었고, 다시 채우려고 합니다.");
                    RefillPotion(x, y);
                }
            }
        }
    }

    private void RefillPotion(int x, int y)
    {
        int yOffset = 1;

        // 현재 셀 위에 있는 셀은 Null이고 우리는 보드의 높이 아래에 있습니다.
        while (y + yOffset < height && beadBoard[x, y + yOffset].bead == null)
        {
            // yOffset 증가
            Debug.Log($"제 위의 물약은 Null이지만 아직 보드의 맨 위에 있지 않으므로 yOffset을 추가하고 다시 시도하십시오. 현재 오프셋은 다음과 같습니다: {yOffset} 1개를 증가할려고 합니다.");
            yOffset++;
        }

        // 물약을 찾지 못하고 보드 위를 눌렀을 때
        if (y + yOffset == height)
        {
            Debug.Log("물약을 찾지 못하고 보드 맨 위에 도달했습니다.");
            SpawnPotionAtTop(x);
        }
        // 물약에 도달
        else if (y + yOffset < height && beadBoard[x, y + yOffset].bead != null)
        {
            // 물약 위에 있는 물약 개체 가져오기
            Bead potionAbove = beadBoard[x, y + yOffset].bead.GetComponent<Bead>();

            // 적절한 위치로 이동
            Debug.Log("보드를 다시 채울 때 물약을 찾았고, 그것의 위치는: [" + x + "," + (y + yOffset) + "] 물약을 움직일 위치는: [" + x + "," + y + "]");
            // 인덱스 업데이트
            potionAbove.SetIndicies(x, y);
            // 포션 보드 업데이트
            beadBoard[x, y] = beadBoard[x, y + yOffset];
            // 물약에서 나온 위치를 null로 설정
            beadBoard[x, y + yOffset] = new Node(true, null);
        }
    }
    
    
    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = width - index;
        Debug.Log("물약을 spawn하려는데, 이상적으로 인덱스에 물약을 넣고 싶다 if: " + index);
        //랜덤으로 물약 얻기
        GameObject newPotion = Instantiate(beadPrefab, new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);

        //좌표 설정하기
        newPotion.GetComponent<Bead>().SetIndicies(x, index);
        //포션 보드 위에 좌표 설정하기
        beadBoard[x, index] = new Node(true, newPotion);
        //해당 위치로 움직이기
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = -1; // 초기값을 -1로 설정
        for (int y = height - 1; y >= 0; y--) // y의 범위 설정 변경
        {
            if (x >= 0 && x < beadBoard.GetLength(0) && y >= 0 && y < beadBoard.GetLength(1)) // 배열 범위 확인
            {
                if (beadBoard[x, y].bead == null)
                {
                    lowestNull = y;
                    break; // null을 찾았으면 반복문을 종료합니다.
                }
            }
        }
        return lowestNull;
    }
    #endregion

    private MatchResult SuperMatch(MatchResult _matchedBeads) //33
    {
        //가로 또는 긴 가로 일치
        if (_matchedBeads.direction == MatchDirection.Horizontal || _matchedBeads.direction == MatchDirection.LongHorizontal)
        {
            foreach (Bead pot in _matchedBeads.connectedBeads)
            {
                List<Bead> extraConnentedBeads = new();
                //Check up
                CheckDirection(pot, new Vector2Int(0, 1), extraConnentedBeads);
                //Check down
                CheckDirection(pot, new Vector2Int(0, -1), extraConnentedBeads);

                if (extraConnentedBeads.Count >= 2)   //2개 이상 일치
                {
                    Debug.Log("Super Horizontal Match");
                    extraConnentedBeads.AddRange(_matchedBeads.connectedBeads);

                    return new MatchResult
                    {
                        connectedBeads = extraConnentedBeads,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedBeads = _matchedBeads.connectedBeads,
                direction = _matchedBeads.direction
            };
        }

        //세로 혹은 긴 세로 일치
        else if (_matchedBeads.direction == MatchDirection.Vertical || _matchedBeads.direction == MatchDirection.LongVertical)
        {
            foreach (Bead pot in _matchedBeads.connectedBeads)
            {
                List<Bead> extraConnentedPotions = new();
                //check right
                CheckDirection(pot, new Vector2Int(1, 0), extraConnentedPotions);
                //check left
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2개 이상 일치
                {
                    Debug.Log("Super Vertical Match");
                    extraConnentedPotions.AddRange(_matchedBeads.connectedBeads);

                    return new MatchResult
                    {
                        connectedBeads = extraConnentedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedBeads = _matchedBeads.connectedBeads,
                direction = _matchedBeads.direction
            };
        }
        return null;    //모든것이 일치하지 않을 때
    }

    MatchResult IsConnected(Bead bead)  //MatchResult(물약 목록, 일치 방향) 부여
    {
        /*
        List<Bead> connectedBeads = new();  //연결된 포션
        BeadType beadType = bead.beadType;  //포션 유형

        connectedBeads.Add(bead);

        //check right
        CheckDirection(bead, new Vector2Int(1, 0), connectedBeads);
        //check left
        CheckDirection(bead, new Vector2Int(-1, 0), connectedBeads);

        if (connectedBeads.Count == 3)    //3개 일치 (Horizontal Match)
        {
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedBeads.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.LongHorizontal
            };
        }
        //연결된 물약 제거
        connectedBeads.Clear();
        //초기 물약을 다시 추가
        connectedBeads.Add(bead);

        //check up
        CheckDirection(bead, new Vector2Int(0, 1), connectedBeads);
        //check down
        CheckDirection(bead, new Vector2Int(0, -1), connectedBeads);

        if (connectedBeads.Count == 3)    //3개 일치 (Horizontal Match)
        {
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedBeads.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.LongVertical
            };
        }
        else   //일치 항목이 없음
        {
            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.None
            };
        }
        */
        return null;
    }

   void CheckDirection(Bead bead, Vector2Int direction, List<Bead> connectedBeads)
   {
        /*
        BeadType beadType = bead.beadType;
        int x = bead.xIndex + direction.x;
        int y = bead.yIndex + direction.y;

        return;
        while (x >= 0 && x < width && y >= 0 && y < height) // 경계 안에 있는지 확인
        {
            if (!beadBoard[x, y].isUsable) // 보드가 채워질 수 없는 경우
                break;

            
            Bead neighbourBead = beadBoard[x, y].bead.GetComponent<Bead_BG>().bead.GetComponent<Bead>();

            // 이웃 물약이 보드 안에 있고, 물약의 종류가 같음
            if (!neighbourBead.isMatched && neighbourBead.beadType == beadType)
            {
                connectedBeads.Add(neighbourBead);

                x += direction.x;
                y += direction.y;
            }
            else
            {
                break;
            }
            
        }
        */
   }


    #region 구슬 교환
    public Bead SetBeadSprite(Vector2 directionVector)
    {
        
        // 이동한 방향에 있는 게임 오브젝트의 스프라이트 가져오기
        GameObject targetObject = GetTargetObjectInDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().type;

            //스프라이트 변경
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //포션 타입 변경
            targetObject.GetComponent<Bead>().type = Bead.Instance.target.GetComponent<Bead>().type;
            Bead.Instance.target.GetComponent<Bead>().type = targetBeadType;
        }
        
        return null;
    }

    private GameObject GetTargetObjectInDirection(Vector2 directionVector)
    {
        // 이동한 방향에 있는 게임 오브젝트 탐색
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

public class MatchResult     //연결된 물약 있음, 방향 확인 됨
{
    public List<Bead> connectedBeads;   //물약 목록
    public MatchDirection direction;    //일치 방향
}