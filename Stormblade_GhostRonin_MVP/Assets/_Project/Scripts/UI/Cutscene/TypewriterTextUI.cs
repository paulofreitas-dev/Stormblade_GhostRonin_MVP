using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class TypewriterTextUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text targetText;

    [Header("Typewriter Settings")]
    [SerializeField] private float letterDelay = 0.06f;

    [Header("Test")]
    [SerializeField] private bool playOnStart;

    public bool IsRevealing {get; private set;}

    public event Action OnRevealFinished;

    private Coroutine revealRoutine;
    private int totalCharacters;

    private void Awake()
    {
        if(targetText == null)
            targetText = GetComponent<TMP_Text>();

        letterDelay = Mathf.Max(0f, letterDelay);

        if(targetText != null)
            targetText.maxVisibleCharacters = 0;
    }

    private void Start()
    {
        if(playOnStart && targetText != null)
            Play(targetText.text);
    }

    public void Play(string content)
    {
        if(targetText == null)
            return;

        if(revealRoutine != null)
            StopCoroutine(revealRoutine);

        targetText.text = content ?? string.Empty;

        targetText.maxVisibleCharacters = 0;

        targetText.ForceMeshUpdate();

        totalCharacters = targetText.textInfo.characterCount;

        IsRevealing = true;

        revealRoutine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        for(int i = 0; i <= totalCharacters; i++)
        {
            targetText.maxVisibleCharacters = i;

            if(i < totalCharacters)
                yield return new WaitForSecondsRealtime(letterDelay);
        }

        revealRoutine = null;
        IsRevealing = false;

        OnRevealFinished?.Invoke();
    }

    public void CompleteImmediately()
    {
        if(!IsRevealing || targetText == null)
            return;

        if(revealRoutine != null)
            StopCoroutine(revealRoutine);

        revealRoutine = null;

        targetText.maxVisibleCharacters = totalCharacters;

        IsRevealing = false;

        OnRevealFinished?.Invoke();
    }
}
