using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eskill : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] sprites;

    void Update()
    {
        // 스페이스 바를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ImageChange());
    }

    private IEnumerator ImageChange()
    {
        _image.gameObject.SetActive(true);

        foreach (var sprite in sprites)
        {
            _image.sprite = sprite;
            yield return new WaitForSeconds(0.2f);
        }

        _image.gameObject.SetActive(false);
    }
}
