using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBoard : MonoBehaviour
{
    //������ ũ�� ���ϱ�
    public int width = 6;   //����
    public int height = 8;  //����

    //������ ���� ���ϱ�
    public float spacingX;
    public float spacingY;

    //���� ������ ���� ����
    public GameObject[] potionPrefabs;

    public List<GameObject> potionsToDestroy = new();    //���� �ı�

    [SerializeField] private Potion selectedPotion; //�̵��� ���� ����

    [SerializeField] private bool isProcessingMove; //������ �̵��ϰ� �ִ��� Ȯ��

    //����
    public Node[,] potionBoard; //���� ����(2���� �迭)
    public GameObject potionBoardGO;


    //������ �������� ������(Inspector���� ����)
    public ArrayLayout arrayLayout;

    //�ڽ�
    public static PotionBoard Instance;

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
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);    //���콺�� �浹�� ��ü�� ����

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())    //���콺�� �浹�� ��ü�� null�� �ƴϰ�, Potion �̶�� Component�� ���� ����
            {
                if (isProcessingMove == true)
                {
                    return;
                }

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                Debug.Log("������ Ŭ���߽��ϴ�, ������ : " + potion.gameObject);

                SelectPotion(potion);
            }
        }
    }

    void InitializeBoard()  //����, ���� ����
    {
        DestroyPotions();
        potionBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;  //X�� ���� ���(2.5)
        spacingY = (float)((height - 1) / 2) + 1;   //Y�� ���� ���(3.5)

        //���� ����(���� �ϴ� -> ���� ���)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);  //������ ������ ��ġ
                if (arrayLayout.rows[y].row[x])
                {
                    potionBoard[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, potionPrefabs.Length);     //���� ����(Random)

                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity); //���� ����
                    potion.GetComponent<Potion>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion); //������ ���忡 ��ġ
                    potionsToDestroy.Add(potion);
                }
            }
        }

        if (CheckBoard() == true)
        {
            Debug.Log("��ġ�ϴ� �׸��� �����ϴ�, ���带 �ٽ� ����ϴ�.");
            //InitializeBoard();
        }
        else
        {
            Debug.Log("��ġ�ϴ� �׸��� �ֽ��ϴ�, ������ �����ϰڽ��ϴ�.");
        }
    }

    private void DestroyPotions()
    {
        if (potionsToDestroy != null)   //�ı��� ������ ���� ���
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

    public bool CheckBoard()    //��ġ�ϴ� ������ �ִ��� Ȯ��, ���� ����
    {
        Debug.Log("Checking Board");
        bool hasMatched = false;

        List<Potion> potionsToRemove = new();   //������ ����

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //���� ��尡 ��� �������� Ȯ��
                if (potionBoard[x, y].isUsable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();    //���� ���� ����

                    if (potion.isMatched == false)  //������ ��ġ�ϴ��� Ȯ��
                    {
                        MatchResult matchedPotions = IsConnected(potion);   //��ġ�� ����(����Ǿ� ����)

                        if (matchedPotions.connectedPotions.Count >= 3) //3�� �̻� ��ġ
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

        return hasMatched;
    }

    private MatchResult SuperMatch(MatchResult _matchedPotions)
    {
        //���� �Ǵ� �� ���� ��ġ
        if (_matchedPotions.direction == MatchDirection.Horizontal || _matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (Potion pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnentedPotions = new();
                //Check up
                CheckDirection(pot, new Vector2Int(0, 1), extraConnentedPotions);
                //Check down
                CheckDirection(pot, new Vector2Int(0, -1), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2�� �̻� ��ġ
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

        //���� Ȥ�� �� ���� ��ġ
        else if (_matchedPotions.direction == MatchDirection.Vertical || _matchedPotions.direction == MatchDirection.LongVertical)
        {
            foreach (Potion pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnentedPotions = new();
                //check right
                CheckDirection(pot, new Vector2Int(1, 0), extraConnentedPotions);
                //check left
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2�� �̻� ��ġ
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
        return null;    //������ ��ġ���� ���� ��
    }
    

    MatchResult IsConnected(Potion potion)  //MatchResult �ο�
    {
        List<Potion> connectedPotions = new();  //����� ����
        PotionType potionType = potion.potionType;  //���� ����

        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        if (connectedPotions.Count == 3)    //3�� ��ġ (Horizontal Match)
        {
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedPotions.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal
            };
        }
        //����� ���� ����
        connectedPotions.Clear();
        //�ʱ� ������ �ٽ� �߰�
        connectedPotions.Add(potion);

        //check up
        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        //check down
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);

        if (connectedPotions.Count == 3)    //3�� ��ġ (Horizontal Match)
        {
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].potionType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedPotions.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].potionType);

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

    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)    //���� Ȯ��(pot : ����, direction : �����̴� ����, connectedPotions : ���� ����)
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height) //��� �ȿ� �ִ��� Ȯ��
        {
            if (potionBoard[x, y].isUsable)  //���尡 ä���� �� �ִ��� Ȯ��
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();

                //�̿� ������ ���� �ȿ� �ְ�, ������ ������ ����
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

    #region ���� ��ȯ
    //���� ����
    public void SelectPotion(Potion _potion)
    {
        //���� ���õ� ������ ���ٸ� ��� Ŭ���� ������ selectedPotion���� ����
        if (selectedPotion == null)
        {
            Debug.Log(_potion);
            selectedPotion = _potion;
        }

        //���� ������ �ι� �����ϸ� ������ ������ null�� �������
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }

        //selectedPotion�� null�� �ƴϸ� ���� ������ �ƴ� ��� ��ȯ�� �õ���
        //selectedPotion�� null�� �ǵ���
        else if (selectedPotion != _potion)
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }
    
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)    //���� ��ü(logic)
    {
        //�ֺ��� ���ٸ� ������ �ʰ�
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        DoSwap(_currentPotion, _targetPotion);


        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }
    
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)    //��ġ ��ȯ
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        //��ġ ������Ʈ( #4 Swapping Potions2 2�� 5��)
        int tempXIndex = _currentPotion.xIndex;
        int tempYIndex = _currentPotion.yIndex;

        _currentPotion.xIndex = _targetPotion.xIndex;
        _currentPotion.yIndex = _targetPotion.yIndex;
        _targetPotion.xIndex = tempXIndex;
        _targetPotion.yIndex = tempYIndex;

        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);    //���� ���� -> ��ǥ ����
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);    //��ǥ ���� -> ���� ����
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion) //������ ��ġ�� �ǵ�����
    {
        yield return new WaitForSeconds(0.2f);

        bool hasMatch = CheckBoard();

        if (hasMatch == false)
        {
            DoSwap(_currentPotion, _targetPotion);
        }
        isProcessingMove = false;
    }

    //�ֺ��� �ִ��� Ȯ��
    private bool IsAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Mathf.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Mathf.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }

    //��ġ
    #endregion
}

public class MatchResult     //����� ���� ����, ���� Ȯ�� ��
{
    public List<Potion> connectedPotions;   //���� ���
    public MatchDirection direction;    //��ġ ����
}

public enum MatchDirection
{
    Vertical,   //����
    Horizontal, //����
    LongVertical,   //�� ���� ��ġ(3�� �̻� ��ġ)
    LongHorizontal, //�� ���� ��ġ
    Super,  //33
    None    //��ġ ���� ����
}