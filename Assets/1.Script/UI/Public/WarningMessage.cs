using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        Color newColor = new();
        newColor.a = 0;
        _Text.color = newColor;
    }

    public void Act()
    {
        StartCoroutine(Acting());
    }

    private IEnumerator Acting()
    {
        Color newColor = new();
        newColor.a = 1;
        _Text.color = newColor;

        yield return new WaitForSeconds(1f);

        while (_Text.color.a > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            newColor.a -= Time.deltaTime;
            _Text.color = newColor;
        };

        gameObject.SetActive(false);
    }
}
