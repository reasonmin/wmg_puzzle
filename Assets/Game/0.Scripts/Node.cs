using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable;   //���尡 ä���� �� �ִ����� ���� ����

    public GameObject bead;

    public Node(bool _isUsable, GameObject _bead) //������
    {
        isUsable = _isUsable;
        bead = _bead;
    }
}
