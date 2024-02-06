
using System;
using System.Collections.Generic;

namespace GalEngine.Core
{
    /// <summary>
    /// 还真不知道怎么命名这个枚举了 暂时用[T]op[B]ottom[C]enter[L]eft[R]ight代替 想到好的再换
    /// </summary>
    [Flags]
    public enum TBCLREnum : byte
    {
        Top = 1 << 0,
        Bottom = 1 << 1,
        Center = 1 << 2,
        Left = 1 << 3,
        Right = 1 << 4,
    }

    /// <summary>
    /// 从字符串转为枚举
    /// </summary>
    public static class TBCLRMap
    {
        private static Dictionary<string, TBCLREnum> map = new Dictionary<string, TBCLREnum>()
        {
            { "left",           TBCLREnum.Left                      },
            { "center",         TBCLREnum.Center                    },
            { "right",          TBCLREnum.Right                     },
            { "topleft",        TBCLREnum.Top | TBCLREnum.Left      },
            { "topcenter",      TBCLREnum.Top | TBCLREnum.Center    },
            { "topright",       TBCLREnum.Top | TBCLREnum.Right     },
            { "bottomleft",     TBCLREnum.Bottom | TBCLREnum.Left   },
            { "bottomcenter",   TBCLREnum.Bottom | TBCLREnum.Center },
            { "bottomright",    TBCLREnum.Bottom | TBCLREnum.Right  },
        };

        public static TBCLREnum Get(string key)
        {
            if (!map.ContainsKey(key))
            {
                GalEngineExternal.DebugLog($"不存在{key}的映射");
                return 0;
            }
            
            return map[key];
        }
    }

    /// <summary>
    /// 脚本支持创建的类型
    /// </summary>
    public enum GalEngineSupportObject
    {
        Image,
        RawImage,
    }

    public static class GalEngineSupportObjectMap
    {
        private static Dictionary<string, GalEngineSupportObject> map = new Dictionary<string, GalEngineSupportObject>()
        {
            { "image",      GalEngineSupportObject.Image    },
            { "rawimage",   GalEngineSupportObject.RawImage },
        };

        public static GalEngineSupportObject Get(string key)
        {
            if (!map.ContainsKey(key))
            {
                GalEngineExternal.DebugLog($"不存在{key}的映射");
                return 0;
            }

            return map[key];
        }
    }
}
