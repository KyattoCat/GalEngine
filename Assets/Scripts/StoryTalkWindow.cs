using GalEngine.Core;
using MiFramework.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryTalkWindow : MonoBehaviour
{
    private StoryTalkWindowDataComponent dataCompt;
    private List<AnswerButton> answerButtonList;
    private StoryTalkContent storyTalkContent;
    private GalEngineObjectManager objectManager;

    private void Awake()
    {
        dataCompt = GetComponent<StoryTalkWindowDataComponent>();

        storyTalkContent = new StoryTalkContent(dataCompt);
        objectManager = new GalEngineObjectManager(dataCompt.ObjectRoot);

        InitEventListener();
        InitButtonClickListener();
        InitAnswerList();
    }

    private void InitEventListener()
    {
        EventManager.Instance.Register<GalEngine_SetNameEvent>(OnSetName);
        EventManager.Instance.Register<GalEngine_SetContentEvent>(OnSetContent);
        EventManager.Instance.Register<GalEngine_SetMidEvent>(OnSetMidContent);
        EventManager.Instance.Register<GalEngine_WaitClickEvent>(OnWaitClick);
        EventManager.Instance.Register<GalEngine_WaitSelectEvent>(OnShowSelect);
        EventManager.Instance.Register<GalEngine_AppendContentEvent>(OnAppendContent);
        EventManager.Instance.Register<GalEngine_CreateEvent>(OnCreateObject);
        EventManager.Instance.Register<GalEngine_SetAnchorEvent>(OnSetObjectAnchor);
        EventManager.Instance.Register<GalEngine_SetPivotEvent>(OnSetObjectPivot);
        EventManager.Instance.Register<GalEngine_SetPosEvent>(OnSetObjectPos);
        EventManager.Instance.Register<GalEngine_SetImageEvent>(OnSetImage);
    }

    private void InitAnswerList()
    {
        answerButtonList = new List<AnswerButton>();
        for (int i = 0; i < 4; i++)
        {
            GameObject gameObject = dataCompt.AnswerList.GetChild(i).gameObject;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            AnswerButton answerButton = new AnswerButton(i, rectTransform, OnClickAnswerButton);
            answerButtonList.Add(answerButton);
        }
        HideAllAnswerButton();
    }

    private void OnShowSelect(GalEngine_WaitSelectEvent e)
    {
        int count = e.AnswerList == null ? 0 : e.AnswerList.Length;
        for (int i = 0; i < count && i < answerButtonList.Count; i++)
        {
            answerButtonList[i].SetText(e.AnswerList[i]);
            answerButtonList[i].SetVisible(true);
        }
    }

    private void HideAllAnswerButton()
    {
        foreach (var btn in answerButtonList)
            btn.SetVisible(false);
    }

    private void InitButtonClickListener()
    {
        dataCompt.ClickNext.onClick.AddListener(OnClickNext);
    }

    private void OnClickAnswerButton(int index)
    {
        using (var arg = EventFactory.Spawn<GalEngine_SelectEvent>())
        {
            arg.SelectIndex = index;
            EventManager.Instance.Invoke(arg);
        }
        HideAllAnswerButton();
    }

    private void OnClickNext()
    {
        if (StoryDataManager.isWaitClick)
        {
            EventManager.Instance.Invoke<GalEngine_CancelWait>();
            StoryDataManager.isWaitClick = false;
        }
        else if (storyTalkContent.IsTyping)
        {
            storyTalkContent.ForceEndType();
        }
    }

    private void OnSetImage(GalEngine_SetImageEvent e)
    {
        objectManager.SetImage(e.ID, e.FilePath);
    }

    private void OnSetObjectPos(GalEngine_SetPosEvent e)
    {
        objectManager.SetAnchordPosition(e.ID, e.X, e.Y);
    }

    private void OnSetObjectPivot(GalEngine_SetPivotEvent e)
    {
        objectManager.SetPivot(e.ID, e.Pivot);
    }

    private void OnSetObjectAnchor(GalEngine_SetAnchorEvent e)
    {
        objectManager.SetAnchor(e.ID, e.Anchor);
    }

    private void OnCreateObject(GalEngine_CreateEvent e)
    {
        objectManager.CreateGameObject(e.ID, e.ObjectType);
    }

    private void OnWaitClick(GalEngine_WaitClickEvent e)
    {
        StoryDataManager.isWaitClick = true;
    }

    private void OnAppendContent(GalEngine_AppendContentEvent e)
    {
        storyTalkContent.SetTextTypewriter(e.Content, StoryTalkContent.TextLocation.Bottom, typingMode: StoryTalkContent.TypingMode.Append);
    }

    private void OnSetMidContent(GalEngine_SetMidEvent e)
    {
        storyTalkContent.SetTextTypewriter(e.Content, StoryTalkContent.TextLocation.Middle);
    }

    private void OnSetContent(GalEngine_SetContentEvent e)
    {
        storyTalkContent.SetTextTypewriter(e.Content, StoryTalkContent.TextLocation.Bottom);
    }

    private void OnSetName(GalEngine_SetNameEvent e)
    {
        storyTalkContent.SetName(e.Name);
    }

    private void Update()
    {
        storyTalkContent?.Update();
        dataCompt.Arrow.SetVisible(!storyTalkContent.IsTyping);
    }
}