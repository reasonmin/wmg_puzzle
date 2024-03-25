using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject[] ItemButton;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("bronze") == "true")
            ItemButton[0].SetActive(true);
        else
            ItemButton[0].SetActive(false);

        if (PlayerPrefs.GetString("silver") == "true")
            ItemButton[1].SetActive(true);
        else
            ItemButton[1].SetActive(false);

        if (PlayerPrefs.GetString("silver") == "true")
            ItemButton[2].SetActive(true);
        else
            ItemButton[2].SetActive(false);
    }

}
