using MiFramework.Event;
using UnityEngine;

namespace GalEngine.Core
{
    /// <summary>
    /// 核心，用于自动读取指令并且自动发送事件
    /// 直到读取到“等待”的指令，然后等待用户发送取消等待再继续
    /// </summary>
    public class GalEngineCore
    {
        private const int MAX_ANSWER_COUNT = 4;
        private bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        private CommandParseResult runningCommands;

        // 阻塞核心状态
        private bool isWaitClick;

        private bool isWaitTime;
        private bool isWaitText;
        private bool isWaitSelect;

        private float waitTo;
        private int currentIndex = 0;
        public int CurrentIndex => currentIndex;

        private int[] cachedJumpIndex;

        public void Initialize(string path)
        {
            runningCommands = CommandParser.ParseScriptFile(path);
            cachedJumpIndex = new int[MAX_ANSWER_COUNT];
            isWaitClick = false;

            InitEventListener();
            isInitialized = true;
        }

        private void InitEventListener()
        {
            EventManager.Instance.Register<GalEngine_CancelWait>(OnCancelWait);
            EventManager.Instance.Register<GalEngine_SelectEvent>(OnSelectAnswer);
            EventManager.Instance.Register<GalEngine_SetTextCompleted>(OnSetTextCompleted);
        }

        private void OnSetTextCompleted(GalEngine_SetTextCompleted e)
        {
            isWaitText = false;
        }

        private void OnSelectAnswer(GalEngine_SelectEvent e)
        {
            isWaitSelect = false;
            JumpTo(cachedJumpIndex[e.SelectIndex]);
        }

        private void OnCancelWait(GalEngine_CancelWait e)
        {
            isWaitClick = false;
        }

        private void StartWaiting(float waitTime)
        {
            waitTo = Time.time + waitTime;
            isWaitTime = true;
        }

        private void StopWaiting()
        {
            waitTo = 0;
            isWaitTime = false;
        }

        private void UpdateWaitTimeState()
        {
            if (!isWaitTime || waitTo <= 0)
                return;

            if (Time.time > waitTo)
                isWaitTime = false;
        }

        private bool CanExecuteNextCommand()
        {
            if (runningCommands?.commands == null || runningCommands.commands.Count == 0)
                return false;

            int commandCount = runningCommands.commands.Count;
            return !isWaitClick && !isWaitTime && !isWaitText && !isWaitSelect && currentIndex < commandCount;
        }

        public void Update()
        {
            if (runningCommands?.commands == null || runningCommands.commands.Count == 0)
                return;

            UpdateWaitTimeState();

            while (CanExecuteNextCommand())
            {
                Command command = runningCommands.commands[currentIndex];

                if ((int)command.type <= 0) // 无效指令直接跳过
                {
                    currentIndex++;
                    continue;
                }

                if (HandleWaitCommand(command))
                {
                    isWaitClick = true;
                    currentIndex++;
                    break;
                }

                if (HandleWaitTimeCommand(command, out float waitTime))
                {
                    isWaitTime = true;
                    StartWaiting(waitTime);
                    currentIndex++;
                    break;
                }

                if (HandleSelectAnswerCommand(command))
                {
                    isWaitSelect = true;
                    currentIndex++;
                    break;
                }

                if (HandleJumpToCommand(command))
                    break;

                DispatchEvent(command);
                currentIndex++;
            }
        }

        private bool HandleSelectAnswerCommand(Command command)
        {
            if (command.type != CommandType.Select)
                return false;

            using (var arg = EventFactory.Spawn<GalEngine_WaitSelectEvent>())
            {
                string[] answerList = command.paramDic["anser"].Split('|');
                string[] labelList = command.paramDic["jumpTo"].Split('|');

                if (answerList == null || labelList == null)
                    return false;

                if (answerList.Length == 0 || labelList.Length == 0)
                    return false;

                // 查询表格判断对应label的index并缓存，等待用户输入
                for (int i = 0; i < labelList.Length && i < MAX_ANSWER_COUNT; i++)
                {
                    if (!runningCommands.jumpTable.TryGetValue(labelList[i], out int index))
                        return false;

                    cachedJumpIndex[i] = index;
                }
                arg.AnswerList = answerList;
                EventManager.Instance.Invoke(arg);
            }

            return true;
        }

        private bool HandleJumpToCommand(Command command)
        {
            if (command.type != CommandType.JumpTo)
                return false;

            string labelName = command.paramDic["label"];
            if (!runningCommands.jumpTable.TryGetValue(labelName, out int index))
                return false;

            JumpTo(index);
            return true;
        }

        private void JumpTo(int index)
        {
            currentIndex = index;
        }

        private bool HandleWaitCommand(Command command)
        {
            if (command.type != CommandType.WaitClick)
                return false;

            EventManager.Instance.Invoke<GalEngine_WaitClickEvent>();
            return true;
        }

        private bool HandleWaitTimeCommand(Command command, out float waitTime)
        {
            waitTime = 0;

            if (command.type != CommandType.WaitTime)
                return false;

            if (!int.TryParse(command.paramDic["ms"], out int ms))
                return false;

            waitTime = ms / 1000.0f;
            return true;
        }

        private void DispatchEvent(Command command)
        {
            switch (command.type)
            {
                case CommandType.SetName:
                    using (var arg = EventFactory.Spawn<GalEngine_SetNameEvent>())
                    {
                        arg.Name = command.paramDic["name"];
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                case CommandType.SetContent:
                    using (var arg = EventFactory.Spawn<GalEngine_SetContentEvent>())
                    {
                        arg.Content = command.paramDic["content"];
                        EventManager.Instance.Invoke(arg);
                    }
                    isWaitText = true;
                    break;

                case CommandType.SetMid:
                    using (var arg = EventFactory.Spawn<GalEngine_SetMidEvent>())
                    {
                        arg.Content = command.paramDic["content"];
                        EventManager.Instance.Invoke(arg);
                    }
                    isWaitText = true;
                    break;

                case CommandType.AppendContent:
                    using (var arg = EventFactory.Spawn<GalEngine_AppendContentEvent>())
                    {
                        arg.Content = command.paramDic["content"];
                        EventManager.Instance.Invoke(arg);
                    }
                    isWaitText = true;
                    break;

                case CommandType.Create:
                    using (var arg = EventFactory.Spawn<GalEngine_CreateEvent>())
                    {
                        arg.ID = int.Parse(command.paramDic["id"]);
                        arg.ObjectType = GalEngineSupportObjectMap.Get(command.paramDic["type"]);
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                case CommandType.Anchor:
                    using (var arg = EventFactory.Spawn<GalEngine_SetAnchorEvent>())
                    {
                        arg.ID = int.Parse(command.paramDic["id"]);
                        arg.Anchor = TBCLRMap.Get(command.paramDic["type"]);
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                case CommandType.Pivot:
                    using (var arg = EventFactory.Spawn<GalEngine_SetPivotEvent>())
                    {
                        arg.ID = int.Parse(command.paramDic["id"]);
                        arg.Pivot = TBCLRMap.Get(command.paramDic["type"]);
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                case CommandType.Pos:
                    using (var arg = EventFactory.Spawn<GalEngine_SetPosEvent>())
                    {
                        arg.ID = int.Parse(command.paramDic["id"]);
                        arg.X = float.Parse(command.paramDic["x"]);
                        arg.Y = float.Parse(command.paramDic["y"]);
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                case CommandType.SetImage:
                    using (var arg = EventFactory.Spawn<GalEngine_SetImageEvent>())
                    {
                        arg.ID = int.Parse(command.paramDic["id"]);
                        arg.FilePath = command.paramDic["file"];
                        EventManager.Instance.Invoke(arg);
                    }
                    break;

                default: 
                    break;
            }
        }
    }
}