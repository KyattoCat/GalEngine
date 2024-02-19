using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GalEngine.Core
{
    public static class CommandParser
    {
        private static int index = 0;
        private static List<Command> tempCommands;
        private static Dictionary<string, int> tempJumpTable;
        private static bool nextContentNeedAppend = false;

        public static CommandParseResult ParseScriptFile(string filePath)
        {
            tempCommands = new List<Command>();
            tempJumpTable = new Dictionary<string, int>();

            string[] allLines = File.ReadAllLines(filePath);

            for (int i = 0; i < allLines.Length; i++)
            {
                string line = allLines[i].Trim();
                if (string.IsNullOrEmpty(line)) 
                    continue;
                ParseCommand(line);
            }

            CommandParseResult result = new CommandParseResult()
            {
                commands = tempCommands,
                jumpTable = tempJumpTable,
            };

            return result;
        }

        private static int AddCommandToList(CommandType commandType, Dictionary<string, string> paramDic)
        {
            Command command = new Command()
            {
                index = index++,
                type = commandType,
                paramDic = paramDic,
            };

            tempCommands.Add(command);

            return index;
        }

        private static void RegisterLabelToJumpTable(string label, int index)
        {
            if (tempJumpTable.ContainsKey(label))
            {
                GalEngineExternal.DebugLog($"该标签已存在{label} index={tempJumpTable[label].ToString()}");
                return;
            }

            tempJumpTable[label] = index;
        }

        private static void ParseCommand(string line)
        {
            CommandType tempCommandType = CommandType.None;
            Dictionary<string, string> tempParamDic = default;

            // 注释
            Match match = Regex.Match(line, @"(?<cmd>[^/]*)//");
            if (match.Success)
                line = match.Groups["cmd"].Value.Trim();

            if (string.IsNullOrEmpty(line))
                return;

            // [who]
            match = Regex.Match(line, @"\A\[(?<name>\w*)\]");
            if (match.Success)
            {
                tempCommandType = CommandType.SetName;
                tempParamDic = new Dictionary<string, string>() { { "name", match.Groups["name"].Value } };
                AddCommandToList(tempCommandType, tempParamDic);
                return;
            }

            // 一句话然后[p]
            match = Regex.Match(line, @"(?<content>.+)\[(?<waitCommand>p|p=\d+)\]");
            if (match.Success)
            {
                Group contentGroup = match.Groups["content"];
                Group waitCommandGroup = match.Groups["waitCommand"];
                if (contentGroup != null && waitCommandGroup != null)
                {
                    string content = contentGroup.Value;
                    string waitCommand = waitCommandGroup.Value;

                    if (!string.IsNullOrEmpty(content))
                    {
                        AddContentCommand(content);
                    }

                    string[] waitParam = waitCommand.Split('=');
                    if (waitParam.Length == 2 && int.TryParse(waitParam[1], out _))
                    {
                        tempCommandType = CommandType.WaitTime;
                        tempParamDic = new Dictionary<string, string>() { { "ms", waitParam[1] } };
                        AddCommandToList(tempCommandType, tempParamDic);
                        nextContentNeedAppend = true;
                    }
                    else
                    {
                        tempCommandType = CommandType.WaitClick;
                        tempParamDic = null;
                        AddCommandToList(tempCommandType, tempParamDic);
                    }

                    return;
                }
            }

            // 正常解析
            string[] lineParams = line.Split(' ');
            string firstStatement = lineParams[0];
            if (firstStatement[0] == '@') // 直接调用的方法
            {
                tempCommandType = CommandTypeMap.GetCommandType(firstStatement);
                tempParamDic = ParseCommandParam(lineParams);
                AddCommandToList(tempCommandType, tempParamDic);
                return;
            }
            else if (firstStatement[0] == '#') // 标签
            {
                tempCommandType = CommandType.Label;
                int index = AddCommandToList(tempCommandType, tempParamDic);
                RegisterLabelToJumpTable(firstStatement, index);
                return;
            }

            // 解析不到 当作设置纯文本了
            if (!string.IsNullOrEmpty(firstStatement))
                AddContentCommand(firstStatement);
        }

        // 统一调用该方法添加设置文本的指令
        // 内部判断是否append或者set
        private static void AddContentCommand(string content)
        {
            CommandType tempCommandType = CommandType.None;
            Dictionary<string, string> tempParamDic = new Dictionary<string, string>() { { "content", content } };

            if (nextContentNeedAppend)
            {
                tempCommandType = CommandType.AppendContent;
                AddCommandToList(tempCommandType, tempParamDic);
                nextContentNeedAppend = false;
            }
            else
            {
                tempCommandType = CommandType.SetContent;
                AddCommandToList(tempCommandType, tempParamDic);
            }
        }

        private static Dictionary<string, string> ParseCommandParam(string[] lineParams)
        {
            if (lineParams == null || lineParams.Length < 2) 
                return null;

            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            for (int i = 1; i < lineParams.Length; i++)
            {
                string[] pair = lineParams[i].Split('=');
                if (pair == null || pair.Length < 2) 
                    continue;

                // 如果出现多个等号，第一个等号后的所有字符串都视作参数
                string value = string.Empty;
                for (int j = 1; j < pair.Length; j++)
                    value += pair[j];

                paramDic[pair[0]] = value;
            }
            return paramDic;
        }
    }
}