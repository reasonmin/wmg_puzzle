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
            InitializeBoard();
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

    public bool CheckBoard()    //��ġ�ϴ� ���� �ִ��� Ȯ��, ���� ����
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

    MatchResult IsConnected(Potion potion)  //������ ��ġ��
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
    //���� ��ü(logic)
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        //�ֺ��� ���ٸ� ������ �ʰ�
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        //�ֺ��� �ִٸ� ��ȯ ����


        isProcessingMove = true;
    }
    //���� ��ȯ
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        //��ġ ������Ʈ( #4 Swapping Potions2 2�� 5��)
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
    Super,
    None    //��ġ ���� ����
}