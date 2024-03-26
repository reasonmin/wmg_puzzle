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
        // �����̽� �ٸ� ������ ��
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ImageChange());
    }

    public IEnumerator ImageChange()
    {
        StartCoroutine(ImageChangea(0));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ImageChangea(1));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ImageChangea(2));
    }
    private IEnumerator ImageChangea(int index)
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
