using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eskill : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject BoomtPrefab;
    [SerializeField] private Transform parent;

    void Update()
    {
        // 스페이스 바를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newObject = Instantiate(BoomtPrefab, parent);
            Eskillexample script = newObject.AddComponent<Eskillexample>();
            script.sprites = sprites;
            script.isRunning = true;
        }
    }
}

public class Eskillexample : MonoBehaviour
{
    public Sprite[] sprites;
    public bool isRunning = false;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0.5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites.Length > 0)
            spriteRenderer.sprite = sprites[0];
    }

    void Update()
    {
        if (!isRunning)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            currentIndex = (currentIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentIndex];
            timer = 0.2f;

            if (currentIndex == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
