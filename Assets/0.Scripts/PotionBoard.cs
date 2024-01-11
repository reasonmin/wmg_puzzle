using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBoard : MonoBehaviour
{
    //보드의 크기 정하기
    public int width = 6;   //가로
    public int height = 8;  //세로

    //보드의 간격 정하기
    public float spacingX;
    public float spacingY;

    //물약 프리텝 갖고 오기
    public GameObject[] potionPrefabs;

    public List<GameObject> potionsToDestroy = new();    //물약 파괴

    [SerializeField] private Potion selectedPotion; //이동할 물약 선택

    [SerializeField] private bool isProcessingMove; //물약이 이동하고 있는지 확인

    //보드
    public Node[,] potionBoard; //물약 보드(2차원 배열)
    public GameObject potionBoardGO;


    //물약을 생성하지 않을곳(Inspector에서 선택)
    public ArrayLayout arrayLayout;

    //자신
    public static PotionBoard Instance;

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
                    potion.GetComponent<Potion>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion); //물약을 보드에 배치
                    potionsToDestroy.Add(potion);
                }
            }
        }

        if (CheckBoard() == true)
        {
            Debug.Log("일치하는 항목이 없습니다, 보드를 다시 만듬니다.");
            InitializeBoard();
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

    public bool CheckBoard()    //일치하는 물이 있는지 확인, 물약 제거
    {
        Debug.Log("Checking Board");
        bool hasMatched = false;

        List<Potion> potionsToRemove = new();   //제거할 물약

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
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);

                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        return hasMatched;
    }

    MatchResult IsConnected(Potion potion)  //물약이 일치함
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
    //물약 교체(logic)
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        //주변에 없다면 말하지 않고
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        //주변에 있다면 교환 시작


        isProcessingMove = true;
    }
    //실제 교환
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        //위치 업데이트( #4 Swapping Potions2 2분 5초)
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
    Super,
    None    //일치 방향 없음
}