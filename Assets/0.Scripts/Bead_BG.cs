using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bead_BG : MonoBehaviour
{
    public static Bead_BG Instance;

    public Sprite[] beadSprite;

    public GameObject bead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, beadSprite.Length);

        bead.GetComponent<SpriteRenderer>().sprite = beadSprite[randomIndex];
        bead.GetComponent<Bead>().beadType = (BeadType)randomIndex;
    }
}
