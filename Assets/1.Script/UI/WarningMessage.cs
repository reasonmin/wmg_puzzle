using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    [SerializeField] private Sentence sentence;

    private void Awake()
    {
        switch (PlayerDataManager.Instance.playerData.language)
        {
            case LanguageType.English:
                _Text.text = sentence.English;
                break;
            case LanguageType.Korean:
                _Text.text = sentence.Korean;
                break;
        }

        Color newColor = new();
        newColor.a = 0;
        _Text.color = newColor;

        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        Color newColor = new();
        newColor.a = 0;
        _Text.color = newColor;

        StartCoroutine(Acting());
    }

    public void Act()
    {
        Color newColor = new();
        newColor.a = 0;
        _Text.color = newColor;

        StartCoroutine(Acting());
    }

    private IEnumerator Acting()
    {
        Color newColor = new();
        newColor.a = 1;
        _Text.color = newColor;

        Vector3 vec = new Vector3(1, 1, 1);

        while (Vector3.Distance(transform.localScale, vec) > 0.1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, vec, Time.deltaTime * 10);
        };

        yield return new WaitForSeconds(1f);

        while (_Text.color.a > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            newColor.a -= Time.deltaTime;
            _Text.color = newColor;
        };

        Destroy(gameObject);
    }
}
