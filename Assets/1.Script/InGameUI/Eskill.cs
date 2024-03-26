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

    private IEnumerator ImageChange()
    {
        StartCoroutine(ImageChangea());
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ImageChangeb());
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ImageChangec());
    }
    private IEnumerator ImageChangea()
    {
        _image[0].gameObject.SetActive(true);

        foreach (var sprite in sprites)
        {
            _image[0].sprite = sprite;
            yield return new WaitForSeconds(0.1f);
        }

        _image[0].gameObject.SetActive(false);
    }

    private IEnumerator ImageChangeb()
    {
        _image[1].gameObject.SetActive(true);

        foreach (var sprite in sprites)
        {
            _image[1].sprite = sprite;
            yield return new WaitForSeconds(0.1f);
        }

        _image[1].gameObject.SetActive(false);
    }

    private IEnumerator ImageChangec()
    {
        _image[2].gameObject.SetActive(true);

        foreach (var sprite in sprites)
        {
            _image[2].sprite = sprite;
            yield return new WaitForSeconds(0.1f);
        }

        _image[2].gameObject.SetActive(false);
    }
}
