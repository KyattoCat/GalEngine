using System.Collections.Generic;

namespace GalEngine.Core
{
    public enum CommandType
    {
        Label = -1,
        None = 0,
        SetName,
        SetContent,
        AppendContent,
        SetMid,
        WaitClick,
        WaitTime,
        Create,
        Anchor,
        Pivot,
        Pos,
        Scale,
        SetImage,
        Select,
        JumpTo,
        Exit,
    }

    public static class CommandTypeMap
    {
        public static Dictionary<string, CommandType> STRING_TO_ENUM = new Dictionary<string, CommandType>()
        {
            { "@setName",           CommandType.SetName             },
            { "@setContent",        CommandType.SetContent          },
            { "@appendContent",     CommandType.AppendContent       },
            { "@setMid",            CommandType.SetMid              },
            { "@waitClick",         CommandType.WaitClick           },
            { "@waitTime",          CommandType.WaitTime            },
            { "@create",            CommandType.Create              },
            { "@anchor",            CommandType.Anchor              },
            { "@pivot",             CommandType.Pivot               },
            { "@pos",               CommandType.Pos                 },
            { "@scale",             CommandType.Scale               },
            { "@setImage",          CommandType.SetImage            },
            { "@select",            CommandType.Select              },
            { "@jumpTo",            CommandType.JumpTo              },
            { "@exit",              CommandType.Exit                },
        };

        public static CommandType GetCommandType(string str)
        {
            if (STRING_TO_ENUM.ContainsKey(str))
            {
                return STRING_TO_ENUM[str];
            }

            GalEngineExternal.DebugLog($"不存在{str}的映射");
            return CommandType.None;
        }
    }
}
