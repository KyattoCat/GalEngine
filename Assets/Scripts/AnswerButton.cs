using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton
{
    private int index;
    private Button button;
    private TextMeshProUGUI text;
    private RectTransform rootTransform;
    private Action<int> onClickButton;

    public AnswerButton(int index, RectTransform rootTransform, Action<int> onClickButton)
    {
        this.index = index;
        this.rootTransform = rootTransform;
        this.onClickButton = onClickButton;

        button = rootTransform.GetComponent<Button>();
        text = button.GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        onClickButton?.Invoke(index);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetVisible(bool visible)
    {
        rootTransform.SetActiveUI(visible);
    }
}