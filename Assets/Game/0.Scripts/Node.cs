using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable;   //보드가 채워질 수 있는지에 대한 여부

    public GameObject bead;

    public Node(bool _isUsable, GameObject _potion) //생성자
    {
        isUsable = _isUsable;
        bead = _potion;
    }
}
