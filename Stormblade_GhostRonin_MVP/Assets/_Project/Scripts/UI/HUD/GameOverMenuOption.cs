using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Data.Common;

public class GameOverMenuOption : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameOverMenuController menuController;

    [Header("Visual")]
    [SerializeField] private TMP_Text optionText;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.red;

    [Header("Selector")]
    [SerializeField] private RectTransform selectorAnchor;

    private void Awake()
    {
        if(menuController == null)
            menuController = GetComponentInParent<GameOverMenuController>();
            
        if(optionText == null)
            return;

        optionText.color = normalColor;
        optionText.rectTransform.localScale = Vector3.one;
    }

    public void OnSelect(BaseEventData eventData)
    {   
        if(optionText != null)
        {
            optionText.color = selectedColor;
            optionText.rectTransform.localScale = Vector3.one * 1.08f;
        }

        if(menuController == null)
            return;

        Debug.Log($"Opção selecionada: {gameObject.name}");

        if(selectorAnchor != null)
            menuController.UpdateSelector(selectorAnchor);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(optionText != null)
        {
            optionText.color = normalColor;
            optionText.rectTransform.localScale = Vector3.one; 
        }
    }
}
