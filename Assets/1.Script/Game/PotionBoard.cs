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


    //���� ������ ���� ����
    public GameObject beadPrefab;
    

    public GameObject potionParent;

    [HideInInspector] public List<GameObject> potionsToDestroy = new();    //���� �ı�

    //������ ���� ���ϱ�
    private float spacingX;
    private float spacingY;

    //����
    public Node[,] beadBoard; //���� ����(2���� �迭)

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

    void InitializeBoard()  //���� ����
    {
        DestroyPotions();
        beadBoard = new Node[width, height];

        //���� ����(���� �ϴ� -> ���� ���)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * ((imageSizeX/100f)), y * ((imageSizeY/100f))); //������ ������ ��ġ

                GameObject bead = Instantiate(beadPrefab, position, Quaternion.identity); //���� ����
                bead.transform.SetParent(potionParent.transform);   //potionParent�ȿ� ����

                if (beadBoard != null)
                {
                    beadBoard[x, y] = new Node(true, bead); //������ ���忡 ��ġ

                }
                potionsToDestroy.Add(bead);
            }
        }

        // �θ��� ���� ��ġ ����
        potionParent.transform.position = new Vector2(-((width * (imageSizeX / 100f)) / 2), -((height * (imageSizeY / 120f)) / 2));

        if (CheckBoard(false))
        {
            Debug.Log("��ġ�ϴ� �׸��� �����ϴ�, ���带 �ٽ� ����ϴ�.");
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

    public bool CheckBoard(bool _takeAction)    //��ġ�ϴ� ������ �ִ��� Ȯ��, ���� ����(_takeAction = true : ����, false : ���� �� ��)
    {
        bool hasMatched = false;

        List<Bead> beadsToRemove = new();   //������ ����

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
                //���� ��尡 ��� �������� Ȯ��
                if (beadBoard != null)
                {
                    if (beadBoard[x, y].isUsable)
                    {
                        /*
                        Bead bead = beadBoard[x, y].bead.GetComponent<Bead_BG>().bead.GetComponent<Bead>();
                        if (bead != null)
                        {
                            if (bead.isMatched == false)  //������ ��ġ�ϴ��� Ȯ��
                            {
                                MatchResult matchedBeads = IsConnected(bead);   //��ġ�� ����(����Ǿ� ����)

                                if (matchedBeads.connectedBeads.Count >= 3) //3�� �̻� ��ġ
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
            foreach (Bead beadToRemove in beadsToRemove)    //������ ����
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

    #region ����, �缳ġ
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
            beadBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (beadBoard[x, y].bead == null)
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
        while (y + yOffset < height && beadBoard[x, y + yOffset].bead == null)
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
        else if (y + yOffset < height && beadBoard[x, y + yOffset].bead != null)
        {
            // ���� ���� �ִ� ���� ��ü ��������
            Bead potionAbove = beadBoard[x, y + yOffset].bead.GetComponent<Bead>();

            // ������ ��ġ�� �̵�
            Debug.Log("���带 �ٽ� ä�� �� ������ ã�Ұ�, �װ��� ��ġ��: [" + x + "," + (y + yOffset) + "] ������ ������ ��ġ��: [" + x + "," + y + "]");
            // �ε��� ������Ʈ
            potionAbove.SetIndicies(x, y);
            // ���� ���� ������Ʈ
            beadBoard[x, y] = beadBoard[x, y + yOffset];
            // ���࿡�� ���� ��ġ�� null�� ����
            beadBoard[x, y + yOffset] = new Node(true, null);
        }
    }
    
    
    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = width - index;
        Debug.Log("������ spawn�Ϸ��µ�, �̻������� �ε����� ������ �ְ� �ʹ� if: " + index);
        //�������� ���� ���
        GameObject newPotion = Instantiate(beadPrefab, new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);

        //��ǥ �����ϱ�
        newPotion.GetComponent<Bead>().SetIndicies(x, index);
        //���� ���� ���� ��ǥ �����ϱ�
        beadBoard[x, index] = new Node(true, newPotion);
        //�ش� ��ġ�� �����̱�
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = -1; // �ʱⰪ�� -1�� ����
        for (int y = height - 1; y >= 0; y--) // y�� ���� ���� ����
        {
            if (x >= 0 && x < beadBoard.GetLength(0) && y >= 0 && y < beadBoard.GetLength(1)) // �迭 ���� Ȯ��
            {
                if (beadBoard[x, y].bead == null)
                {
                    lowestNull = y;
                    break; // null�� ã������ �ݺ����� �����մϴ�.
                }
            }
        }
        return lowestNull;
    }
    #endregion

    private MatchResult SuperMatch(MatchResult _matchedBeads) //33
    {
        //���� �Ǵ� �� ���� ��ġ
        if (_matchedBeads.direction == MatchDirection.Horizontal || _matchedBeads.direction == MatchDirection.LongHorizontal)
        {
            foreach (Bead pot in _matchedBeads.connectedBeads)
            {
                List<Bead> extraConnentedBeads = new();
                //Check up
                CheckDirection(pot, new Vector2Int(0, 1), extraConnentedBeads);
                //Check down
                CheckDirection(pot, new Vector2Int(0, -1), extraConnentedBeads);

                if (extraConnentedBeads.Count >= 2)   //2�� �̻� ��ġ
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

        //���� Ȥ�� �� ���� ��ġ
        else if (_matchedBeads.direction == MatchDirection.Vertical || _matchedBeads.direction == MatchDirection.LongVertical)
        {
            foreach (Bead pot in _matchedBeads.connectedBeads)
            {
                List<Bead> extraConnentedPotions = new();
                //check right
                CheckDirection(pot, new Vector2Int(1, 0), extraConnentedPotions);
                //check left
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnentedPotions);

                if (extraConnentedPotions.Count >= 2)   //2�� �̻� ��ġ
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
        return null;    //������ ��ġ���� ���� ��
    }

    MatchResult IsConnected(Bead bead)  //MatchResult(���� ���, ��ġ ����) �ο�
    {
        /*
        List<Bead> connectedBeads = new();  //����� ����
        BeadType beadType = bead.beadType;  //���� ����

        connectedBeads.Add(bead);

        //check right
        CheckDirection(bead, new Vector2Int(1, 0), connectedBeads);
        //check left
        CheckDirection(bead, new Vector2Int(-1, 0), connectedBeads);

        if (connectedBeads.Count == 3)    //3�� ��ġ (Horizontal Match)
        {
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedBeads.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.LongHorizontal
            };
        }
        //����� ���� ����
        connectedBeads.Clear();
        //�ʱ� ������ �ٽ� �߰�
        connectedBeads.Add(bead);

        //check up
        CheckDirection(bead, new Vector2Int(0, 1), connectedBeads);
        //check down
        CheckDirection(bead, new Vector2Int(0, -1), connectedBeads);

        if (connectedBeads.Count == 3)    //3�� ��ġ (Horizontal Match)
        {
            Debug.Log("��ġ ����� ���� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedBeads.Count > 3)    //3�� �ʰ� ��ġ (Long horizontal Match)
        {
            Debug.Log("����� ����� ��ġ�� �ְ� �˴ϴ�. ����� ���� : " + connectedBeads[0].beadType);

            return new MatchResult
            {
                connectedBeads = connectedBeads,
                direction = MatchDirection.LongVertical
            };
        }
        else   //��ġ �׸��� ����
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
        while (x >= 0 && x < width && y >= 0 && y < height) // ��� �ȿ� �ִ��� Ȯ��
        {
            if (!beadBoard[x, y].isUsable) // ���尡 ä���� �� ���� ���
                break;

            
            Bead neighbourBead = beadBoard[x, y].bead.GetComponent<Bead_BG>().bead.GetComponent<Bead>();

            // �̿� ������ ���� �ȿ� �ְ�, ������ ������ ����
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


    #region ���� ��ȯ
    public Bead SetBeadSprite(Vector2 directionVector)
    {
        
        // �̵��� ���⿡ �ִ� ���� ������Ʈ�� ��������Ʈ ��������
        GameObject targetObject = GetTargetObjectInDirection(directionVector);

        if (targetObject != null)
        {
            Sprite targetSprite = targetObject.GetComponent<SpriteRenderer>().sprite;
            BeadType targetBeadType = targetObject.GetComponent<Bead>().type;

            //��������Ʈ ����
            targetObject.GetComponent<SpriteRenderer>().sprite = Bead.Instance.target.GetComponent<SpriteRenderer>().sprite;
            Bead.Instance.target.GetComponent<SpriteRenderer>().sprite = targetSprite;

            //���� Ÿ�� ����
            targetObject.GetComponent<Bead>().type = Bead.Instance.target.GetComponent<Bead>().type;
            Bead.Instance.target.GetComponent<Bead>().type = targetBeadType;
        }
        
        return null;
    }

    private GameObject GetTargetObjectInDirection(Vector2 directionVector)
    {
        // �̵��� ���⿡ �ִ� ���� ������Ʈ Ž��
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

public class MatchResult     //����� ���� ����, ���� Ȯ�� ��
{
    public List<Bead> connectedBeads;   //���� ���
    public MatchDirection direction;    //��ġ ����
}