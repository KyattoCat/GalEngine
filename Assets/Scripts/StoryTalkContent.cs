using MiFramework.Event;
using TMPro;
using UnityEngine;

public class StoryTalkContent
{
    private StoryTalkWindowDataComponent dataCompt;
    private TextMeshProUGUI[] allText;

    public enum TextLocation
    {
        Bottom,
        Middle,
    }

    public enum TextEffect
    {
        None,
        FadeIn,
        Typewriter,
    }

    public enum TypingMode
    {
        Set,
        Append,
    }

    private TypewriterStorage typewriterStorage;

    private float fadeInStartTime;
    private float fadeInDuration;

    private string prevContent;
    private string currentContent;
    private TextEffect currentEffect;
    private TextLocation currentLocation;
    private TypingMode typingMode;
    private bool isTyping;
    public bool IsTyping => isTyping;

    public StoryTalkContent(StoryTalkWindowDataComponent compt)
    {
        this.dataCompt = compt;
        typewriterStorage = new TypewriterStorage();
        InitAllText();
    }

    private void InitAllText()
    {
        // 和TextLocation枚举一一对应
        allText = new TextMeshProUGUI[]
        {
            dataCompt.BottomText,
            dataCompt.MiddleText,
        };
    }

    private void ClearAllTextContent()
    {
        foreach (var text in allText)
            text?.SetText(string.Empty);
    }

    private TextMeshProUGUI GetCurrentLocationText()
    {
        return allText[(int)currentLocation];
    }

    public void SetTextTypewriter(string content, TextLocation textLocation, int perCharTime = 200, TypingMode typingMode = TypingMode.Set)
    {
        if (typingMode == TypingMode.Append)
            prevContent += currentContent;
        else if (typingMode == TypingMode.Set)
            prevContent = string.Empty;

        this.currentContent = content;
        this.currentLocation = textLocation;
        this.currentEffect = TextEffect.Typewriter;
        this.typingMode = typingMode;

        if (typingMode == TypingMode.Set)
            ClearAllTextContent();

        typewriterStorage.SetUp(content, Time.time, perCharTime);
        isTyping = true;
    }

    public void SetTextFadeIn(string content, TextLocation textLocation, int duration = 200)
    {
        this.currentContent = content;
        this.currentLocation = textLocation;
        this.currentEffect = TextEffect.FadeIn;

        ClearAllTextContent();

        var text = GetCurrentLocationText();
        text.alpha = 0;
        text.SetText(content);

        fadeInDuration = duration / 1000.0f;
        isTyping = true;
    }

    public void SetName(string name)
    {
        dataCompt.NameText.SetText(name);
    }

    public void ForceEndType()
    {
        if (!isTyping) 
            return;

        var text = GetCurrentLocationText();
        text.SetText(prevContent + currentContent);

        if (currentEffect == TextEffect.FadeIn)
            text.alpha = 1;

        TypeComplete();
    }

    private void TypeComplete()
    {
        isTyping = false;
        EventManager.Instance.Invoke<GalEngine_SetTextCompleted>();
    }

    public void Update()
    {
        if (!isTyping)
            return;

        var text = GetCurrentLocationText();

        if (currentEffect == TextEffect.Typewriter)
        {
            text.text = prevContent + typewriterStorage.GetString(Time.time);
        }
        else if (currentEffect == TextEffect.FadeIn)
        {
            text.alpha = Mathf.Lerp(0, 1, (Time.time - fadeInStartTime) / fadeInDuration);
        }

        // 判断是否打印完毕
        if ((currentEffect == TextEffect.Typewriter && Time.time > typewriterStorage.EndTime) ||
            (currentEffect == TextEffect.FadeIn && text.alpha >= 0.999f))
        {
            TypeComplete();
        }
    }
}
