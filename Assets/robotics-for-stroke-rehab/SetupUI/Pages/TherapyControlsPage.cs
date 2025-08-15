using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TherapyControlsPage : MonoBehaviour
{
    public TextMeshProUGUI IsAssistingText;
    public TextMeshProUGUI AssistanceLevelText;

    [SerializeField] private GameObject BeginButton;
    [SerializeField] private GameObject StopButton;
    [SerializeField] private GameObject ClearScreenButton;
    [SerializeField] private GameObject GenerateSegmentsButton;
    [SerializeField] private GameObject ClearSegmentsButton;
    [SerializeField] private GameObject PullToStartButton;
    [SerializeField] private GameObject LoopingToggle;
    

    public void ShowTextForAssisting()
    {
        IsAssistingText.color = Color.green;
        IsAssistingText.text = "Assisting";
    }

    public void ShowTextForNotAssisting()
    {
        IsAssistingText.color = Color.red;
        IsAssistingText.text = "Not Assisting";
    }

    public void ClearText()
    {
        IsAssistingText.text = "";
    }


    public void SetupForTrailMakerRunning()
    {
        BeginButton.SetActive(false);
        StopButton.SetActive(true);
    }

    public void SetupForTrailMakerEnd()
    {
        StopButton.SetActive(false);
    }

    public void SetupForClearScreen(bool canClear)
    {
        ClearScreenButton.SetActive(canClear);
    }

    public void SetupForGenerateSegments(bool canGenerate)
    {
        GenerateSegmentsButton.SetActive(canGenerate);
        LoopingToggle.SetActive(canGenerate);
    }

    public void SetupForClearSegments(bool canClear)
    {
        ClearSegmentsButton.SetActive(canClear);
    }

    public void SetupForCanBeginTrailmaker(bool canBegin)
    {
        BeginButton.SetActive(canBegin);
    }

    public void SetupForPullingToFirstTargetToggle(bool isPulling)
    {
        PullToStartButton.SetActive(isPulling);
    }

    public void UpdateAssistanceLevel(float level)
    {
        AssistanceLevelText.text = level.ToString(("#.##"));
    }
}
