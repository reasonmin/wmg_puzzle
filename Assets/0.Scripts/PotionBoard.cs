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

    //보드의 간격 정하기
    public float spacingX;
    public float spacingY;

    //물약 프리텝 갖고 오기
    public GameObject[] potionPrefabs;
    public GameObject potionParent;

    [HideInInspector] public List<GameObject> potionsToDestroy = new();    //물약 파괴

    private bool isProcessingMove; //물약이 이동하고 있는지 확인(true : 이동 중, false : 이동 중 아님)

    //보드
    public Node[,] potionBoard; //물약 보드(2차원 배열)

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

    void InitializeBoard()  //보드, 물약 생성
    {
        DestroyPotions();
        potionBoard = new Node[width, height];

        //보드 생성(좌측 하단 -> 우측 상단)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX/100f)), y * ((imageSizeY/100f))); //물약이 생성될 위치

                if (true)
                {
                    //potionBoard[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, potionPrefabs.Length);     //물약 생성(Random)

                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity); //물약 생성
                    potion.transform.SetParent(potionParent.transform);
                    //potion.GetComponent<Bead>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion); //물약을 보드에 배치
                    potionsToDestroy.Add(potion);
                }
            }
        }

        // 부모의 보드 위치 수정
        //potionParent.transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));

        if (CheckBoard(false))
        {
            //Debug.Log("일치하는 항목이 없습니다, 보드를 다시 만듬니다.");
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

    public bool CheckBoard(bool _takeAction)    //일치하는 물약이 있는지 확인, 물약 제거
    {
        bool hasMatched = false;

        List<Bead> potionsToRemove = new();   //제거할 물약

        foreach (Node nodepotion in potionBoard)
        {
            if (nodepotion.potion != null)
            {
                //nodepotion.potion.GetComponent<Potion>().isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //물약 노드가 사용 가능한지 확인
                if (potionBoard[x, y].isUsable)
                {
                    Bead potion = potionBoard[x, y].potion.GetComponent<Bead>();    //물약 변수 생성

                    if (/*potion.isMatched == false && */potion != null)  //물약이 일치하는지 확인
                    {
                        MatchResult matchedPotions = IsConnected(potion);   //일치된 물약(연결되어 있음)

                        if (matchedPotions.connectedPotions.Count >= 3) //3개 이상 일치
                        {
                            MatchResult superMathedPotions = SuperMatch(matchedPotions);

                            potionsToRemove.AddRange(superMathedPotions.connectedPotions);

                            foreach (Bead pot in superMathedPotions.connectedPotions)
                                //pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        if (_takeAction)
        {
            foreach (Bead potionToRemove in potionsToRemove)    //제거할 물약
            {
                //potionToRemove.isMatched = false;
            }

            RemovAndRefill(potionsToRemove);

            if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }

        return hasMatched;
    }

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
            potionBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].potion == null)
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
        while (y + yOffset < height && potionBoard[x, y + yOffset].potion == null)
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
        else if (y + yOffset < height && potionBoard[x, y + yOffset].potion != null)
        {
            // 물약 위에 있는 물약 개체 가져오기
            Bead potionAbove = potionBoard[x, y + yOffset].potion.GetComponent<Bead>();

            // 적절한 위치로 이동
            Debug.Log("보드를 다시 채울 때 물약을 찾았고, 그것의 위치는: [" + x + "," + (y + yOffset) + "] 물약을 움직일 위치는: [" + x + "," + y + "]");
            // 인덱스 업데이트
            potionAbove.SetIndicies(x, y);
            // 포션 보드 업데이트
            potionBoard[x, y] = potionBoard[x, y + yOffset];
            // 물약에서 나온 위치를 null로 설정
            potionBoard[x, y + yOffset] = new Node(true, null);
        }
    }
    
    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = width - index;
        Debug.Log("물약을 spawn하려는데, 이상적으로 인덱스에 물약을 넣고 싶다 if: " + index);
        // 랜덤으로 물약 얻기
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        // 지표 설정하기
        newPotion.GetComponent<Bead>().SetIndicies(x, index);
        // 포션 보드 위에 지표 설정하기
        potionBoard[x, index] = new Node(true, newPotion);
        // 해당 위치로 움직이기
        Vector3 targetPosition = new Vector3(newPotion.transform.position.x, newPotion.transform.position.y - locationToMoveTo, newPotion.transform.position.z);
        //newPotion.GetComponent<Potion>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = -1; // 초기값을 -1로 설정
        for (int y = height - 1; y >= 0; y--) // y의 범위 설정 변경
        {
            if (x >= 0 && x < potionBoard.GetLength(0) && y >= 0 && y < potionBoard.GetLength(1)) // 배열 범위 확인
            {
                if (potionBoard[x, y].potion == null)
                {
                    lowestNull = y;
                    break; // null을 찾았으면 반복문을 종료합니다.
                }
            }
        }
        return lowestNull;
    }

    #region 일치 항목
    private MatchResult SuperMatch(MatchResult _matchedPotions) //33
    {
        //가로 또는 긴 가로 일치
        if (_matchedPotions.direction == MatchDirection.Horizontal || _matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (Bead pot in _matchedPotions.connectedPotions)
            {
                List<Bead> extraConnentedPotions = new();
                //Check up
                CheckDirection(pot, new Vector2Int(0, 1), extraConnentedPotions);
                //Check down
                CheckDirection(pot, new Vector2Int(0, -1), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2개 이상 일치
                {
                    Debug.Log("Super Horizontal Match");
                    extraConnentedPotions.AddRange(_matchedPotions.connectedPotions);

                    return new MatchResult
                    {
                        connectedPotions = extraConnentedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = _matchedPotions.connectedPotions,
                direction = _matchedPotions.direction
            };
        }

        //세로 혹은 긴 세로 일치
        else if (_matchedPotions.direction == MatchDirection.Vertical || _matchedPotions.direction == MatchDirection.LongVertical)
        {
            foreach (Bead pot in _matchedPotions.connectedPotions)
            {
                List<Bead> extraConnentedPotions = new();
                //check right
                CheckDirection(pot, new Vector2Int(1, 0), extraConnentedPotions);
                //check left
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2개 이상 일치
                {
                    Debug.Log("Super Vertical Match");
                    extraConnentedPotions.AddRange(_matchedPotions.connectedPotions);

                    return new MatchResult
                    {
                        connectedPotions = extraConnentedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = _matchedPotions.connectedPotions,
                direction = _matchedPotions.direction
            };
        }
        return null;    //모든것이 일치하지 않을 때
    }
    #endregion


    MatchResult IsConnected(Bead potion)  //MatchResult(물약 목록, 일치 방향) 부여
    {
        List<Bead> connectedPotions = new();  //연결된 포션
        //PotionType potionType = potion.potionType;  //포션 유형

        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        if (connectedPotions.Count == 3)    //3개 일치 (Horizontal Match)
        {
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedPotions.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal
            };
        }
        //연결된 물약 제거
        connectedPotions.Clear();
        //초기 물약을 다시 추가
        connectedPotions.Add(potion);

        //check up
        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        //check down
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);

        if (connectedPotions.Count == 3)    //3개 일치 (Horizontal Match)
        {
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedPotions.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical
            };
        }
        else   //일치 항목이 없음
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.None
            };
        }
    }

    void CheckDirection(Bead pot, Vector2Int direction, List<Bead> connectedPotions)    //방향 확인(pot : 포션, direction : 움직이는 방향, connectedPotions : 물약 유형)
    {
        //PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height) //경계 안에 있는지 확인
        {
            if (potionBoard[x, y].isUsable)  //보드가 채워질 수 있는지 확인
            {
                Bead neighbourPotion = potionBoard[x, y].potion.GetComponent<Bead>();

                //이웃 물약이 보드 안에 있고, 물약의 종류가 같음
                /*if (neighbourPotion.isMatched == false && neighbourPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }*/

            }
            else
            {
                break;
            }
        }
    }

    #region 구슬 교환
    public Bead SetBeadSprite(string direction)
    {
        // 이동한 방향에 있는 게임 오브젝트의 스프라이트 가져오기
        GameObject targetObject = GetTargetObjectInDirection(direction);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().beadType;

            //스프라이트 변경
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //포션 타입 변경
            targetObject.GetComponent<Bead>().beadType = Bead.Instance.target.GetComponent<Bead>().beadType;
            Bead.Instance.target.GetComponent<Bead>().beadType = targetBeadType;
        }

        return null;
    }

    private GameObject GetTargetObjectInDirection(string direction)
    {
        // 이동한 방향에 따라 탐색할 방향 벡터 설정
        Vector2 directionVector = Vector2.zero;
        switch (direction)
        {
            case "right":
                directionVector = Vector2.right;
                break;
            case "left":
                directionVector = Vector2.left;
                break;
            case "up":
                directionVector = Vector2.up;
                break;
            case "down":
                directionVector = Vector2.down;
                break;
        }

        // 이동한 방향에 있는 게임 오브젝트 탐색
        RaycastHit2D hit = Physics2D.Raycast(Bead.Instance.target.transform.localPosition, directionVector);

        if (hit.collider != null && hit.collider.gameObject != Bead.Instance.target.gameObject)
        {
            GameObject targetObject = hit.collider.gameObject;
            Debug.Log(targetObject.GetComponent<Bead>().beadType);
            return targetObject;
        }
        return null;    //게임 오브젝트를 찾지 못 했을 때 null를 반환
    }
    #endregion
}

public class MatchResult     //연결된 물약 있음, 방향 확인 됨
{
    public List<Bead> connectedPotions;   //물약 목록
    public MatchDirection direction;    //일치 방향
}