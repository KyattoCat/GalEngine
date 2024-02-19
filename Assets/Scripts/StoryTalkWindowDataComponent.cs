using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryTalkWindowDataComponent : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI BottomText;
    public TextMeshProUGUI MiddleText;
    public Button ClickNext;
    public RectTransform AnswerList;

    public RawImage Background1;
    public RawImage Background2;
    public RectTransform ObjectRoot;

    public RectTransform Arrow;
}
