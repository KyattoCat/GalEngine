using GalEngine.Core;
using MiFramework.Event;

public class GalEngine_SetContentEvent : EventArguments
{
    public string Content { get; set; }

    public override void Clear()
    {
        Content = string.Empty;
    }
}


public class GalEngine_SetNameEvent : EventArguments
{
    public string Name { get; set; }
    public override void Clear()
    {
        Name = string.Empty;
    }
}

public class GalEngine_SetMidEvent : EventArguments
{
    public string Content { get; set; }
    public override void Clear()
    {
        Content = string.Empty;
    }
}


public class GalEngine_WaitClickEvent : EventArguments
{
    public override void Clear()
    {

    }
}

public class GalEngine_WaitSecondEvent : EventArguments
{
    public int WaitTime { get; set; }

    public override void Clear()
    {
        WaitTime = 0;
    }
}

public class GalEngine_CancelWait : EventArguments
{
    public override void Clear()
    {

    }
}

public class GalEngine_WaitSelectEvent : EventArguments
{
    public string[] AnswerList { get; set; }

    public override void Clear()
    {
        
    }
}

public class GalEngine_SelectEvent : EventArguments
{
    public int SelectIndex { get; set; }

    public override void Clear()
    {
        SelectIndex = 0;
    }
}

/// <summary>
/// 文本效果播放完毕
/// </summary>
public class GalEngine_SetTextCompleted : EventArguments
{
    public override void Clear()
    {

    }
}

public class GalEngine_AppendContentEvent : EventArguments
{
    public string Content { get; set; }

    public override void Clear()
    {
        Content = string.Empty;
    }
}

public class GalEngine_CreateEvent : EventArguments
{
    public int ID { get; set; }
    public GalEngineSupportObject ObjectType { get; set; }

    public override void Clear()
    {
        ID = 0;
    }
}

public class GalEngine_SetAnchorEvent : EventArguments
{
    public int ID { get; set; }

    public TBCLREnum Anchor { get; set; }

    public override void Clear()
    {
        ID = 0;
        Anchor = 0;
    }
}

public class GalEngine_SetPivotEvent : EventArguments
{
    public int ID { get; set; }

    public TBCLREnum Pivot { get; set; }

    public override void Clear()
    {
        ID = 0;
        Pivot = 0;
    }
}

public class GalEngine_SetPosEvent : EventArguments
{
    public int ID { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public override void Clear()
    {
        ID = 0;
        X = 0;
        Y = 0;
    }
}

public class GalEngine_SetImageEvent : EventArguments
{
    public int ID { get; set; }
    public string FilePath { get; set; }

    public override void Clear()
    {
        ID = 0;
        FilePath = string.Empty;
    }
}