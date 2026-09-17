using UnityEngine;
using System;

public class CustceneController : MonoBehaviour
{
    [Serializable] 
    private class CutsceneStep
    {
        public GameObject screen;

        [TextArea(2, 6)]
        public string text;
    }

    [Header("Cutscene")]
    [SerializeField] private CutsceneStep[] steps;

    [Header("References")]
    [SerializeField] private TypewriterTextUI typewriter;

    private int currentStepIndex;

    private void Start()
    {
        ShowStep(0);
    }

    private void ShowStep(int index)
    {
        if(index < 0 || index >= steps.Length)
            return;

        for(int i = 0; i < steps.Length; i++)
        {
            if(steps[i].screen != null)
                steps[i].screen.SetActive(i == index);
        }

        currentStepIndex = index;

        if(typewriter != null)
            typewriter.Play(steps[index].text);
    }

    public void RequestAdvance()
    {
        if(typewriter != null && typewriter.IsRevealing)
        {
            typewriter.CompleteImmediately();
            return;
        }

        int nextStepIndex = currentStepIndex + 1;

        if (nextStepIndex >= steps.Length)
        {
            Debug.Log("CutsceneController: último quadro alcançado.");

            return;
        }
        
        ShowStep(nextStepIndex);
    }
}
