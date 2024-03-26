using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eskill : MonoBehaviour
{
    [SerializeField] private Image[] _image;
    [SerializeField] private Sprite[] sprites;

    void Update()
    {
        // 스페이스 바를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ImageChange());
    }

    public IEnumerator ImageChange()
    {
        int rand = Random.Range(0, 3);

        StartCoroutine(ImageChanges(rand));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ImageChanges((rand + 1) % 3));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ImageChanges((rand + 2) % 3));
    }
    private IEnumerator ImageChanges(int index)
    {
        _image[index].gameObject.SetActive(true);

        foreach (var sprite in sprites)
        {
            _image[index].sprite = sprite;
            yield return new WaitForSeconds(0.1f);
        }

        _image[index].gameObject.SetActive(false);
    }
}
