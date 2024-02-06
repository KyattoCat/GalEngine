using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using MiFramework.Event;

namespace GalEngine.Core
{
    public class GalEngineObjectComponent : MonoBehaviour
    {
        public GalEngineSupportObject ObjectType { get; set; }
    }

    /// <summary>
    /// 管理由脚本发起的create指令创建的物体 会挂一个GalEngineObjectComponent
    /// </summary>
    public class GalEngineObjectManager
    {
        private RectTransform rootTransform;
        private Dictionary<int, GameObject> objects;

        public GalEngineObjectManager(RectTransform rootTransform)
        {
            this.rootTransform = rootTransform;
            objects = new Dictionary<int, GameObject>();
        }

        public void CreateGameObject(int id, GalEngineSupportObject objectType)
        {
            if (objects.ContainsKey(id))
                return;

            GameObject gameObject = new GameObject($"{id}_{objectType}");

            GalEngineObjectComponent galEngineObject = gameObject.AddComponent<GalEngineObjectComponent>();
            galEngineObject.ObjectType = objectType;

            gameObject.AddComponent<RectTransform>();

            AddComponentByType(gameObject, objectType);

            gameObject.transform.SetParent(rootTransform);
            objects[id] = gameObject;
        }

        private void AddComponentByType(GameObject gameObject, GalEngineSupportObject objectType)
        {
            if (objectType == GalEngineSupportObject.RawImage)
                gameObject.AddComponent<RawImage>();
            else if (objectType == GalEngineSupportObject.Image)
                gameObject.AddComponent<Image>();
        }

        private GameObject GetObjectByID(int id)
        {
            if (!objects.ContainsKey(id))
                return null;
            return objects[id];
        }

        private RectTransform GetRectTransformByID(int id)
        {
            GameObject gameObject = GetObjectByID(id);
            if (gameObject == null)
                return null;

            return gameObject.GetComponent<RectTransform>();
        }

        public void SetAnchordPosition(int id, float x, float y)
        {
            RectTransform rectTransform = GetRectTransformByID(id);
            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        public void SetAnchor(int id, TBCLREnum anchor)
        {
            RectTransform rectTransform = GetRectTransformByID(id);
            if (rectTransform == null)
                return;

            float x = 0.5f;
            float y = 0.5f;

            if ((anchor & TBCLREnum.Top) > 0)
                y = 1;
            else if ((anchor & TBCLREnum.Bottom) > 0)
                y = 0;

            if ((anchor & TBCLREnum.Left) > 0)
                x = 0;
            else if ((anchor & TBCLREnum.Right) > 0)
                x = 1;

            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(x, y);
        }

        public void SetPivot(int id, TBCLREnum pivot)
        {
            RectTransform rectTransform = GetRectTransformByID(id);
            if (rectTransform == null)
                return;

            float x = 0.5f;
            float y = 0.5f;

            if ((pivot & TBCLREnum.Top) > 0)
                y = 1;
            else if ((pivot & TBCLREnum.Bottom) > 0)
                y = 0;

            if ((pivot & TBCLREnum.Left) > 0)
                x = 0;
            else if ((pivot & TBCLREnum.Right) > 0)
                x = 1;

            rectTransform.pivot = new Vector2(x, y);
        }

        public void SetImage(int id, string filePath)
        {
            GameObject gameObject = GetObjectByID(id);
            if (gameObject == null)
                return;

            GalEngineObjectComponent galEngineObject = gameObject.GetComponent<GalEngineObjectComponent>();
            if (galEngineObject.ObjectType == GalEngineSupportObject.RawImage)
            {
                RawImage rawImage = galEngineObject.GetComponent<RawImage>();
                rawImage.texture = GalEngineExternal.LoadAsset?.Invoke(filePath) as Texture;
                rawImage.raycastTarget = false;
                rawImage.SetNativeSize();
            }
            else if (galEngineObject.ObjectType == GalEngineSupportObject.Image)
            {
                Image image = galEngineObject.GetComponent<Image>();
                image.sprite = GalEngineExternal.LoadAsset?.Invoke(filePath) as Sprite;
                image.raycastTarget = false;
                image.SetNativeSize();
            }
        }

        public void Dispose()
        {

        }
    }
}