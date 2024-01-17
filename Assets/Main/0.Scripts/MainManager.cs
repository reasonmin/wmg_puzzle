using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private List<ChapterBoard> chapterBoards;
    [SerializeField] private Image Star;

    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject MainBoardPanel;
    [SerializeField] private GameObject RuleBookPanel;
    [SerializeField] private GameObject AudioPanel;
    [SerializeField] private GameObject BeforeButton;
    [SerializeField] private GameObject StageView;
    [SerializeField] private GameObject StoreView;

    // Start is called before the first frame update
    void Start()
    {
        int starNum = 2;

        for (int i = 0; i < chapterBoards.Count; i++)
        {
            for (int j = 0; j < chapterBoards[i].stageButtons.Count; j++)
            {
                chapterBoards[i].stageButtons[j].stageNumText.text = (i + 1).ToString() + "-" + (j + 1).ToString();

                for (int k = 0; k < starNum; k++)
                {
                    chapterBoards[i].stageButtons[j].starImages[k].sprite = Star.sprite;
                }
            }
        }

        SettingPanel.SetActive(false);
        OnStageView();
    }

    private void LoadJson()
    {

    }

    private void SaveJson()
    {

    }

    public void OnSettingPanel()
    {
        ResetPanel(MainBoardPanel);
        BeforeButton.SetActive(false);
        SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void ResetPanel(GameObject Panel)
    {
        MainBoardPanel.SetActive(false);
        RuleBookPanel.SetActive(false);
        AudioPanel.SetActive(false);

        Panel.SetActive(true);
    }

    public void OnBefore()
    {
        ResetPanel(MainBoardPanel);
        BeforeButton.SetActive(false);
    }

    public void OnRuleBookPanel()
    {
        ResetPanel(RuleBookPanel);
        BeforeButton.SetActive(true);
    }

    public void OnAudioPanel()
    {
        ResetPanel(AudioPanel);
        BeforeButton.SetActive(true);
    }

    public void ResetView(GameObject View)
    {
        StageView.SetActive(false);
        StoreView.SetActive(false);

        View.SetActive(true);
    }

    public void OnStageView()
    {
        ResetView(StageView);
    }

    public void OnStoreView()
    {
        ResetView(StoreView);
    }
}
