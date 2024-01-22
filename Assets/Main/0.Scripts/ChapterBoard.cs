using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterBoard : MonoBehaviour
{
    public List<StageButton> stageButtons;
    public RectTransform rTransform;
    private void Update()
    {
        SceneChange.instance.y = rTransform.anchoredPosition.y;
    }

    private void Start()
    {
        if(SceneChange.instance.y > 0)
        {
            rTransform.anchoredPosition = new Vector2(0, SceneChange.instance.y);
        }
    }
}
