using System;

namespace GalEngine.Core
{
    /// <summary>
    /// 需要调用外部的接口
    /// </summary>
    public static class GalEngineExternal
    {
        // 加载资源的委托 从外部注册
        public static Func<string, UnityEngine.Object> LoadAsset;
        // 输出错误信息
        public static Action<string> DebugLog;

        static GalEngineExternal()
        {
#if UNITY_EDITOR
            LoadAsset = UnityEngine.Resources.Load;
            DebugLog = UnityEngine.Debug.Log;
#endif
        }
    }
}