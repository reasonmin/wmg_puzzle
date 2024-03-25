using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject[] ItemButton;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("bronze") == "True")
            ItemButton[0].SetActive(true);
        else
            ItemButton[0].SetActive(false);

        if (PlayerPrefs.GetString("silver") == "True")
            ItemButton[1].SetActive(true);
        else
            ItemButton[1].SetActive(false);

        if (PlayerPrefs.GetString("gold") == "True")
            ItemButton[2].SetActive(true);
        else
            ItemButton[2].SetActive(false);
    }

    public void OnBronze()
    {
        if (BoardManager.Instance.isPlay == true)
        {
            ItemButton[0].SetActive(false);
            StartCoroutine(UseBronzeItem());
        }
    }

    [HideInInspector] public Collider2D target = null; //���� ���� ����

    public IEnumerator UseBronzeItem()
    {
        BoardManager.Instance.isPlay = false;
        bool isT = true;

        while (isT)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GetHit2D().collider != null)
                {
                    target = GetHit2D().collider;
                    target.GetComponent<Bead>().SetBead((int)target.GetComponent<Bead>().Type, SpecialBT.Five);
                    isT = false;
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BoardManager.Instance.isPlay = true;
    }

    RaycastHit2D GetHit2D()
    {
        // ���� ���콺 ��ġ�� ��ũ�� ��ǥ�κ��� ���̷� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ����ĳ��Ʈ�� ���� ���̿� �浹�� ��ü�� ���� ���� ����
        return Physics2D.Raycast(ray.origin, ray.direction);
    }
}
