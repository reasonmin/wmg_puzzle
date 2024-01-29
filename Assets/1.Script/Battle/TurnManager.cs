using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Turned());
    }

    public IEnumerator Turned()
    {
        while (true)
        {
            yield return StartCoroutine(Enemy.Instance.StartTurn());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
