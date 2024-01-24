using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchDirection
{
    Vertical,   //����
    Horizontal, //����
    LongVertical,   //�� ���� ��ġ(3�� �̻� ��ġ)
    LongHorizontal, //�� ���� ��ġ
    Super,  //33
    None    //��ġ ���� ����
}

public class PotionBoard : MonoBehaviour
{
    //�ڽ�
    public static PotionBoard Instance;

    //������ ũ�� ���ϱ�
    public int width = 8;  //����
    public int height = 7; //����

    public float imageSizeX;
    public float imageSizeY;

    //������ ���� ���ϱ�
    public float spacingX;
    public float spacingY;

    //���� ������ ���� ����
    public GameObject[] potionPrefabs;
    public GameObject potionParent;

    [HideInInspector] public List<GameObject> potionsToDestroy = new();    //���� �ı�

    private bool isProcessingMove; //������ �̵��ϰ� �ִ��� Ȯ��(true : �̵� ��, false : �̵� �� �ƴ�)

    //����
    public Node[,] potionBoard; //���� ����(2���� �迭)

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

    void InitializeBoard()  //����, ���� ����
    {
        DestroyPotions();
        potionBoard = new Node[width, height];

        //���� ����(���� �ϴ� -> ���� ���)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX/100f)), y * ((imageSizeY/100f))); //������ ������ ��ġ

                if (true)
                {
                    //potionBoard[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, potionPrefabs.Length);     //���� ����(Random)

                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity); //���� ����
                    potion.transform.SetParent(potionParent.transform);
                    //potion.GetComponent<Bead>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion); //������ ���忡 ��ġ
                    potionsToDestroy.Add(potion);
                }
            }
        }

        // �θ��� ���� ��ġ ����
        //potionParent.transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));

        if (CheckBoard(false))
        {
            //Debug.Log("��ġ�ϴ� �׸��� �����ϴ�, ���带 �ٽ� ����ϴ�.");
            potionParent.transform.position = Vector2.zero;
            InitializeBoard();
        }
    }


    private void DestroyPotions()
    {
        //�ı��� ������ ���� ���
        if (potionsToDestroy != null)   
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

    public bool CheckBoard(bool _takeAction)    //��ġ�ϴ� ������ �ִ��� Ȯ��, ���� ����
    {
        bool hasMatched = false;

        List<Bead> potionsToRemove = new();   //������ ����

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
                //���� ��尡 ��� �������� Ȯ��
                if (potionBoard[x, y].isUsable)
                {
                    Bead potion = potionBoard[x, y].potion.GetComponent<Bead>();    //���� ���� ����

                    if (/*potion.isMatched == false && */potion != null)  //������ ��ġ�ϴ��� Ȯ��
                    {
                        MatchResult matchedPotions = IsConnected(potion);   //��ġ�� ����(����Ǿ� ����)

                        if (matchedPotions.connectedPotions.Count >= 3) //3�� �̻� ��ġ
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
            foreach (Bead potionToRemove in potionsToRemove)    //������ ����
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
        //������ �����ϰ� �ش� ��ġ�� ���带 �����
        foreach (Bead potion in _potionsToRemove)
        {
            //x, y �ε����� �ӽ÷� ����
            int _xIndex = potion.xIndex;
            int _yIndex = potion.yIndex;

            //Destoy potion
            Destroy(potion.gameObject);

            //�� ���� �����
            potionBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].potion == null)
                {
                    //Debug.Log($"��ġ X :{x} Y : {y} �� �����, �ٽ� ä����� �մϴ�.");
                    RefillPotion(x, y);
                }
            }
        }
    }

    private void RefillPotion(int x, int y)
    {
        int yOffset = 1;

        // ���� �� ���� �ִ� ���� Null�̰� �츮�� ������ ���� �Ʒ��� �ֽ��ϴ�.
        while (y + yOffset < height && potionBoard[x, y + yOffset].potion == null)
        {
            // yOffset ����
            Debug.Log($"�� ���� ������ Null������ ���� ������ �� ���� ���� �����Ƿ� yOffset�� �߰��ϰ� �ٽ� �õ��Ͻʽÿ�. ���� �������� ������ �����ϴ�: {yOffset} 1���� �����ҷ��� �մϴ�.");
            yOffset++;
        }

        // ������ ã�� ���ϰ� ���� ���� ������ ��
        if (y + yOffset == height)
        {
            Debug.Log("������ ã�� ���ϰ� ���� �� ���� �����߽��ϴ�.");
            SpawnPotionAtTop(x);
        }
        // ���࿡ ����
        else if (y + yOffset < height && potionBoard[x, y + yOffset].potion != null)
        {
            // ���� ���� �ִ� ���� ��ü ��������
            Bead potionAbove = potionBoard[x, y + yOffset].potion.GetComponent<Bead>();

            // ������ ��ġ�� �̵�
            Debug.Log("���带 �ٽ� ä�� �� ������ ã�Ұ�, �װ��� ��ġ��: [" + x + "," + (y + yOffset) + "] ������ ������ ��ġ��: [" + x + "," + y + "]");
            // �ε��� ������Ʈ
            potionAbove.SetIndicies(x, y);
            // ���� ���� ������Ʈ
            potionBoard[x, y] = potionBoard[x, y + yOffset];
            // ���࿡�� ���� ��ġ�� null�� ����
            potionBoard[x, y + yOffset] = new Node(true, null);
        }
    }
    
    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = width - index;
        Debug.Log("������ spawn�Ϸ��µ�, �̻������� �ε����� ������ �ְ� �ʹ� if: " + index);
        // �������� ���� ���
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        // ��ǥ �����ϱ�
        newPotion.GetComponent<Bead>().SetIndicies(x, index);
        // ���� ���� ���� ��ǥ �����ϱ�
        potionBoard[x, index] = new Node(true, newPotion);
        // �ش� ��ġ�� �����̱�
        Vector3 targetPosition = new Vector3(newPotion.transform.position.x, newPotion.transform.position.y - locationToMoveTo, newPotion.transform.position.z);
        //newPotion.GetComponent<Potion>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = -1; // �ʱⰪ�� -1�� ����
        for (int y = height - 1; y >= 0; y--) // y�� ���� ���� ����
        {
            if (x >= 0 && x < potionBoard.GetLength(0) && y >= 0 && y < potionBoard.GetLength(1)) // �迭 ���� Ȯ��
            {
                if (potionBoard[x, y].potion == null)
                {
                    lowestNull = y;
                    break; // null�� ã������ �ݺ����� �����մϴ�.
                }
            }
        }
        return lowestNull;
    }

    #region ��ġ �׸�
    private MatchResult SuperMatch(MatchResult _matchedPotions) //33
    {
        //���� �Ǵ� �� ���� ��ġ
        if (_matchedPotions.direction == MatchDirection.Horizontal || _matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (Bead pot in _matchedPotions.connectedPotions)
            {
                List<Bead> extraConnentedPotions = new();
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
            foreach (Bead pot in _matchedPotions.connectedPotions)
            {
                List<Bead> extraConnentedPotions = new();
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
    #endregion


    MatchResult IsConnected(Bead potion)  //MatchResult(���� ���, ��ġ ����) �ο�
    {
        List<Bead> connectedPotions = new();  //����� ����
        //PotionType potionType = potion.potionType;  //���� ����

        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        if (connectedPotions.Count == 3)    //3�� ��ġ (Horizontal Match)
        {
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedPotions.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].beadType);

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
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedPotions.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedPotions[0].beadType);

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical
            };
        }
        else   //��ġ �׸��� ����
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.None
            };
        }
    }

    void CheckDirection(Bead pot, Vector2Int direction, List<Bead> connectedPotions)    //���� Ȯ��(pot : ����, direction : �����̴� ����, connectedPotions : ���� ����)
    {
        //PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height) //��� �ȿ� �ִ��� Ȯ��
        {
            if (potionBoard[x, y].isUsable)  //���尡 ä���� �� �ִ��� Ȯ��
            {
                Bead neighbourPotion = potionBoard[x, y].potion.GetComponent<Bead>();

                //�̿� ������ ���� �ȿ� �ְ�, ������ ������ ����
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

    #region ���� ��ȯ
    public Bead SetBeadSprite(string direction)
    {
        // �̵��� ���⿡ �ִ� ���� ������Ʈ�� ��������Ʈ ��������
        GameObject targetObject = GetTargetObjectInDirection(direction);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().beadType;

            //��������Ʈ ����
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //���� Ÿ�� ����
            targetObject.GetComponent<Bead>().beadType = Bead.Instance.target.GetComponent<Bead>().beadType;
            Bead.Instance.target.GetComponent<Bead>().beadType = targetBeadType;
        }

        return null;
    }

    private GameObject GetTargetObjectInDirection(string direction)
    {
        // �̵��� ���⿡ ���� Ž���� ���� ���� ����
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

        // �̵��� ���⿡ �ִ� ���� ������Ʈ Ž��
        RaycastHit2D hit = Physics2D.Raycast(Bead.Instance.target.transform.localPosition, directionVector);

        if (hit.collider != null && hit.collider.gameObject != Bead.Instance.target.gameObject)
        {
            GameObject targetObject = hit.collider.gameObject;
            Debug.Log(targetObject.GetComponent<Bead>().beadType);
            return targetObject;
        }
        return null;    //���� ������Ʈ�� ã�� �� ���� �� null�� ��ȯ
    }
    #endregion
}

public class MatchResult     //����� ���� ����, ���� Ȯ�� ��
{
    public List<Bead> connectedPotions;   //���� ���
    public MatchDirection direction;    //��ġ ����
}