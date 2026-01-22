using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Modifier Choice Panel")]
    [SerializeField] private GameObject modifierPanel;
    [SerializeField] private Button[] optionButtons;          // 3 botones
    [SerializeField] private TMP_Text[] optionTitleTexts;     // 3 títulos
    [SerializeField] private TMP_Text[] optionDescTexts;      // 3 descripciones

    private Action<int> _onOptionSelected;

    private void Awake()
    {
        Instance = this;
        modifierPanel.SetActive(false);
    }

    public void ShowModifierChoices(ModifierOffer[] offers, System.Action<int> onOptionSelected)
    {
        _onOptionSelected = onOptionSelected;
        modifierPanel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            optionTitleTexts[i].text = offers[i].Title;
            optionDescTexts[i].text = offers[i].Description;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() =>
            {
                modifierPanel.SetActive(false);
                _onOptionSelected?.Invoke(index);
            });
        }
    }

}
