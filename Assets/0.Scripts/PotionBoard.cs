using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBoard : MonoBehaviour
{
    //자신
    public static PotionBoard Instance;

    //보드의 크기 정하기
    public int width = 8;  //가로
    public int height = 7; //세로

    //보드의 간격 정하기
    public float spacingX;
    public float spacingY;

    //물약 프리텝 갖고 오기
    public GameObject[] potionPrefabs;

    public List<GameObject> potionsToDestroy = new();    //물약 파괴
    public GameObject potionParent;

    [SerializeField] private Potion selectedPotion; //이동할 물약 선택

    [SerializeField] private bool isProcessingMove; //물약이 이동하고 있는지 확인

    //보드
    public Node[,] potionBoard; //물약 보드(2차원 배열)
    public GameObject potionBoardGO;


    //물약을 생성하지 않을곳(Inspector에서 선택)
    public ArrayLayout arrayLayout;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeBoard();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);    //마우스와 충돌한 객체를 저장

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())    //마우스와 충돌한 객체가 null이 아니고, Potion 이라는 Component를 갖고 있음
            {
                if (isProcessingMove == true)
                {
                    return;
                }

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                Debug.Log("물약을 클릭했습니다, 물약은 : " + potion.gameObject);

                SelectPotion(potion);
            }
        }
    }

    void InitializeBoard()  //보드, 물약 생성
    {
        DestroyPotions();
        potionBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;  //X축 간격 계산(2.5)
        spacingY = (float)((height - 1) / 2) + 1;   //Y축 간격 계산(3.5)

        //보드 생성(좌측 하단 -> 우측 상단)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);  //물약이 생성될 위치
                if (arrayLayout.rows[y].row[x])
                {
                    potionBoard[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, potionPrefabs.Length);     //물약 생성(Random)

                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity); //물약 생성
                    potion.transform.SetParent(potionParent.transform);
                    potion.GetComponent<Potion>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion); //물약을 보드에 배치
                    potionsToDestroy.Add(potion);
                }
            }
        }

        if (CheckBoard(false))
        {
            Debug.Log("일치하는 항목이 없습니다, 보드를 다시 만듬니다.");
            //InitializeBoard();
        }
        else
        {
            Debug.Log("일치하는 항목이 있습니다, 게임을 시작하겠습니다.");
        }
    }

    private void DestroyPotions()
    {
        if (potionsToDestroy != null)   //파괴할 물약이 있을 경우
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
        Debug.Log("Checking Board");
        bool hasMatched = false;

        List<Potion> potionsToRemove = new();   //제거할 물약

        foreach (Node nodepotion in potionBoard)
        {
            if (nodepotion.potion != null)
            {
                nodepotion.potion.GetComponent<Potion>().isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //물약 노드가 사용 가능한지 확인
                if (potionBoard[x, y].isUsable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();    //물약 변수 생성

                    if (potion.isMatched == false)  //물약이 일치하는지 확인
                    {
                        MatchResult matchedPotions = IsConnected(potion);   //일치된 물약(연결되어 있음)

                        if (matchedPotions.connectedPotions.Count >= 3) //3개 이상 일치
                        {
                            MatchResult superMathedPotions = SuperMatch(matchedPotions);

                            potionsToRemove.AddRange(superMathedPotions.connectedPotions);

                            foreach (Potion pot in superMathedPotions.connectedPotions)
                                pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }
        if (_takeAction)
        {
            foreach (Potion potionToRemove in potionsToRemove)    //제거할 물약
            {
                potionToRemove.isMatched = false;
            }

            RemovAndRefill(potionsToRemove);

            if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }

        return hasMatched;
    }

    private void RemovAndRefill(List<Potion> _potionsToRemove)
    {
        //물약을 제거하고 해당 위치의 보드를 비워줌
        foreach (Potion potion in _potionsToRemove)
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
                    Debug.Log($"The location X :{x} Y : {y} is empty, attempting to refill it");
                    RefillPotion(x, y);
                }
            }
        }
    }

    private void RefillPotion(int x, int y)
    {
        int yOffset = 1;

        //현재 셀 위에 있는 셀은 Null이고 우리는 보드의 높이 아래에 있습니다.
        while (y + yOffset < height && potionBoard[x, y + yOffset].potion == null)
        {
            //yOffset 증가
            Debug.Log($"제 위의 물약은 Null이지만 아직 보드의 맨 위에 있지 않으므로 yOffset을 추가하고 다시 시도하십시오. 현재 오프셋은 다음과 같습니다 : {yOffset} 1개를 증가할려고 합니다.");
            yOffset++;
        }

        //we've either hit the top of the board or we found a potion 보드 위를 치거나 물약을 찾았을 때?
        if (y + yOffset < height && potionBoard[x, y + yOffset].potion != null)
        {
            //물약에 도달

            Potion potionAbove = potionBoard[x, y + yOffset].potion.GetComponent<Potion>();

            //적절한 위치로 이동
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, potionAbove.transform.position.z);
            Debug.Log("I've found a potion when refilling the board and it  was in the location : [" + x + "," + (y + yOffset) +"] we have moved it to the location : [" + x + "," + y +"]");
            //위치 이동
            potionAbove.MoveToTarget(targetPos);
            //update inclidces->?
            potionAbove.SetIndicies(x, y);
            //update our potionBoard
            potionBoard[x, y] = potionBoard[x, y + yOffset];
            //물약에서 나온 위치를 null로 설정
            potionBoard[x, y + yOffset] = new Node(true, null);
        }

        //if we've hit the top of the board without finding a potion 물약을 찾지 못하고 보드 위를 쳤을 때?
        if (y + yOffset ==  height)
        {
            Debug.Log("I've reached the top of the board without finding a potion");
            SpawnPotionAtTop(x);
        }
    }

    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = 8 - index;
        Debug.Log("About to spawn a potion, ideally i'd like to put it in the index if : " + index);
        //랜덤으로 물약 얻기
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        //지표 설정하기
        newPotion.GetComponent<Potion>().SetIndicies(x, index);
        //포션보드 위에 지표 설정하기
        potionBoard[x, index] = new Node(true, newPotion);
        //해당 위치로 움직이기
        Vector3 targetPostion = new Vector3(newPotion.transform.position.x, newPotion.transform.position.y - locationToMoveTo, newPotion.transform.position.z);
        newPotion.GetComponent<Potion>().MoveToTarget(targetPostion);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 7; y >= 0; y--)
        {
            if (potionBoard[x, y].potion == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }
    #region Cascading Potions

    #endregion

    private MatchResult SuperMatch(MatchResult _matchedPotions) //33
    {
        //가로 또는 긴 가로 일치
        if (_matchedPotions.direction == MatchDirection.Horizontal || _matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (Potion pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnentedPotions = new();
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
            foreach (Potion pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnentedPotions = new();
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


    MatchResult IsConnected(Potion potion)  //MatchResult 부여
    {
        List<Potion> connectedPotions = new();  //연결된 포션
        PotionType potionType = potion.potionType;  //포션 유형

        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        if (connectedPotions.Count == 3)    //3개 일치 (Horizontal Match)
        {
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedPotions.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].potionType);

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
            Debug.Log("일치 색상과 수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedPotions.Count > 3)    //3개 초과 일치 (Long horizontal Match)
        {
            Debug.Log("색상과 긴수직 일치가 있게 됩니다. 연결된 물약 : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.None
            };
        }
    }

    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)    //방향 확인(pot : 방향, direction : 움직이는 방향, connectedPotions : 물약 유형)
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height) //경계 안에 있는지 확인
        {
            if (potionBoard[x, y].isUsable)  //보드가 채워질 수 있는지 확인
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();

                //이웃 물약이 보드 안에 있고, 물약의 종류가 같음
                if (neighbourPotion.isMatched == false && neighbourPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }

            }
            else
            {
                break;
            }
        }
    }

    #region 물약 교환
    //물약 선택
    public void SelectPotion(Potion _potion)
    {
        //현재 선택된 물약이 없다면 방금 클릭한 포션을 selectedPotion으로 설정
        if (selectedPotion == null)
        {
            Debug.Log(_potion);
            selectedPotion = _potion;
        }

        //같은 물약을 두번 선택하면 선택한 물약을 null로 만들어줌
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }

        //selectedPotion이 null이 아니며 현재 포션이 아닌 경우 교환을 시도함
        //selectedPotion을 null로 되돌림
        else if (selectedPotion != _potion)
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }
    
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)    //물약 교체(logic)
    {
        //주변에 없다면 말하지 않고
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        DoSwap(_currentPotion, _targetPotion);


        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }
    
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)    //위치 교환
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        //위치 업데이트( #4 Swapping Potions2 2분 5초)
        int tempXIndex = _currentPotion.xIndex;
        int tempYIndex = _currentPotion.yIndex;

        _currentPotion.xIndex = _targetPotion.xIndex;
        _currentPotion.yIndex = _targetPotion.yIndex;
        _targetPotion.xIndex = tempXIndex;
        _targetPotion.yIndex = tempYIndex;

        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);    //현재 물약 -> 목표 물약
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);    //목표 물약 -> 현재 물약
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion) //원래의 위치로 되돌리기
    {
        yield return new WaitForSeconds(0.2f);

        bool hasMatch = CheckBoard(true);

        if (hasMatch == false)
        {
            DoSwap(_currentPotion, _targetPotion);
        }
        isProcessingMove = false;
    }

    //주변에 있는지 확인
    private bool IsAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Mathf.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Mathf.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }

    //일치
    #endregion
}

public class MatchResult     //연결된 물약 있음, 방향 확인 됨
{
    public List<Potion> connectedPotions;   //물약 목록
    public MatchDirection direction;    //일치 방향
}

public enum MatchDirection
{
    Vertical,   //수직
    Horizontal, //수평
    LongVertical,   //긴 수직 일치(3개 이상 일치)
    LongHorizontal, //긴 수평 일치
    Super,  //33
    None    //일치 방향 없음
}