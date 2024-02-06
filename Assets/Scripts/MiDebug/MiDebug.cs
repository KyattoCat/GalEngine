using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YHJDebug
{
    public static class MiDebug
    {
        private static StringBuilder sb = new StringBuilder();
        public static string ToDebugString(this GameObject[] objects)
        {
            sb.Clear();

            foreach (var obj in objects)
            {
                sb.Append(obj.name + "|");
            }

            return sb.ToString();
        }

        public static string ToDebugString(this in Vector3 vector)
        {
            return $"({vector.x:f4}, {vector.y:f4}, {vector.z:f4})";
        }

        public static string ToDebugString(this in Vector2 vector)
        {
            return $"({vector.x:f4}, {vector.y:f4})";
        }

        public static void Log(object obj1, object obj2)
        {
            sb.Append(obj1); sb.Append(" ");
            sb.Append(obj2);
            Log(sb.ToString());
        }
        public static void Log(object obj1, object obj2, object obj3)
        {
            sb.Append(obj1); sb.Append(" ");
            sb.Append(obj2); sb.Append(" ");
            sb.Append(obj3);
            Log(sb.ToString());
        }
        public static void Log(object obj1, object obj2, object obj3, object obj4)
        {
            sb.Append(obj1); sb.Append(" ");
            sb.Append(obj2); sb.Append(" ");
            sb.Append(obj3); sb.Append(" ");
            sb.Append(obj4);
            Log(sb.ToString());
        }
        public static void Log(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            sb.Append(obj1); sb.Append(" ");
            sb.Append(obj2); sb.Append(" ");
            sb.Append(obj3); sb.Append(" ");
            sb.Append(obj4); sb.Append(" ");
            sb.Append(obj5);
            Log(sb.ToString());
        }
        public static void Log(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
        {
            sb.Append(obj1); sb.Append(" ");
            sb.Append(obj2); sb.Append(" ");
            sb.Append(obj3); sb.Append(" ");
            sb.Append(obj4); sb.Append(" ");
            sb.Append(obj5); sb.Append(" ");
            sb.Append(obj6);
            Log(sb.ToString());
        }

        public static void Log(object msg)
        {
            sb.Clear();
            LogWithColor(msg, Color.cyan);
        }

        public static string ToHex(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        public static void LogWithColor(object msg, Color color)
        {
            UnityEngine.Debug.Log($"<color=#{color.ToHex()}>{msg}</color>");
        }

        public static Vector3[] pointTemp = new Vector3[64];

        public static void DrawLine(Vector3 startPos, Vector3 endPos, Color color) => UnityEngine.Debug.DrawLine(startPos, endPos, color);

        public static void DrawCube(Vector3 center, Vector3 aabbMin, Vector3 aabbMax, Color color)
        {
            aabbMin += center;
            aabbMax += center;

            // 底层
            pointTemp[0].x = aabbMin.x; pointTemp[0].y = aabbMin.y; pointTemp[0].z = aabbMin.z;
            pointTemp[1].x = aabbMin.x; pointTemp[1].y = aabbMin.y; pointTemp[1].z = aabbMax.z;
            pointTemp[2].x = aabbMax.x; pointTemp[2].y = aabbMin.y; pointTemp[2].z = aabbMax.z;
            pointTemp[3].x = aabbMax.x; pointTemp[3].y = aabbMin.y; pointTemp[3].z = aabbMin.z;
            // 顶层
            pointTemp[4].x = aabbMin.x; pointTemp[4].y = aabbMax.y; pointTemp[4].z = aabbMin.z;
            pointTemp[5].x = aabbMin.x; pointTemp[5].y = aabbMax.y; pointTemp[5].z = aabbMax.z;
            pointTemp[6].x = aabbMax.x; pointTemp[6].y = aabbMax.y; pointTemp[6].z = aabbMax.z;
            pointTemp[7].x = aabbMax.x; pointTemp[7].y = aabbMax.y; pointTemp[7].z = aabbMin.z;

            // 底层
            UnityEngine.Debug.DrawLine(pointTemp[0], pointTemp[1], color);
            UnityEngine.Debug.DrawLine(pointTemp[1], pointTemp[2], color);
            UnityEngine.Debug.DrawLine(pointTemp[2], pointTemp[3], color);
            UnityEngine.Debug.DrawLine(pointTemp[3], pointTemp[0], color);
            // 顶层
            UnityEngine.Debug.DrawLine(pointTemp[4], pointTemp[5], color);
            UnityEngine.Debug.DrawLine(pointTemp[5], pointTemp[6], color);
            UnityEngine.Debug.DrawLine(pointTemp[6], pointTemp[7], color);
            UnityEngine.Debug.DrawLine(pointTemp[7], pointTemp[4], color);
            // 两层连接
            UnityEngine.Debug.DrawLine(pointTemp[0], pointTemp[4], color);
            UnityEngine.Debug.DrawLine(pointTemp[1], pointTemp[5], color);
            UnityEngine.Debug.DrawLine(pointTemp[2], pointTemp[6], color);
            UnityEngine.Debug.DrawLine(pointTemp[3], pointTemp[7], color);
        }

        public static void DrawCube(Vector3 p0, Vector3 p1, Color color)
        {
            Vector3 center = (p0 + p1) / 2;
            float minX = Mathf.Min(p0.x, p1.x) - center.x;
            float minY = Mathf.Min(p0.y, p1.y) - center.y;
            float minZ = Mathf.Min(p0.z, p1.z) - center.z;
            float maxX = Mathf.Max(p0.x, p1.x) - center.x;
            float maxY = Mathf.Max(p0.y, p1.y) - center.y;
            float maxZ = Mathf.Max(p0.z, p1.z) - center.z;

            Vector3 aabbMin = new Vector3(minX, minY, minZ);
            Vector3 aabbMax = new Vector3(maxX, maxY, maxZ);

            DrawCube(center, aabbMin, aabbMax, color);
        }

        public static void DrawSector(Vector3 startPos, Vector3 direction, float distance, float angle, float sectorAngle, Color color, int arcPointAmount = 5)
        {
            if (arcPointAmount < 3) return;
            // 方向归一化确保没错
            direction = direction.normalized;
            // 获得旋转矩阵
            var halfAngle = sectorAngle / 2;
            var matrix = new Matrix4x4();

            var perAngle = sectorAngle / (arcPointAmount - 1);
            var forwardVector = direction * distance;

            var right = Vector3.Cross(Vector3.up, direction);
            var axis = Quaternion.AngleAxis(angle, -direction);
            var angleRight = axis * right;
            var normal = Vector3.Cross(direction, angleRight);

            for (int i = 0; i < arcPointAmount; i++)
            {
                var quaternion = Quaternion.AngleAxis(-halfAngle + perAngle * i, normal);
                matrix.SetTRS(startPos, quaternion, Vector3.one);
                pointTemp[i] = matrix.MultiplyVector(forwardVector) + startPos;
                UnityEngine.Debug.DrawLine(startPos, pointTemp[i], color);
                if (i > 0)
                    UnityEngine.Debug.DrawLine(pointTemp[i], pointTemp[i - 1], color);
            }
        }

        public static GameObject GetFirstPickGameObject(Vector2 position)
        {
            EventSystem eventSystem = EventSystem.current;
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = position;

            List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, uiRaycastResultCache);
            if (uiRaycastResultCache.Count > 0)
                return uiRaycastResultCache[0].gameObject;
            return null;
        }
    }

}
