using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YHJDebug;

public class MiDebugMono : MonoBehaviour
{
    [Space(10)]
    public bool checkCurrentClickElement;

    [Header("Camera Follow")]
    [Space(20)]
    public bool enableSceneCameraFollowGameCamera;

    private void Update()
    {
        CheckCurrentClickElement();
        SceneCameraFollowGameCamera();
    }

    private void CheckCurrentClickElement()
    {
        if (!checkCurrentClickElement) return;
        // UI阻挡
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = MiDebug.GetFirstPickGameObject(Input.mousePosition);
            if (go != null)
            {
                Transform tf = go.transform;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("CurrentClickElement: ");
                while (tf != null)
                {
                    stringBuilder.Append(tf.name);
                    if (tf.parent != null)
                        stringBuilder.Append("->");
                    tf = tf.parent;
                }
                MiDebug.LogWithColor(stringBuilder.ToString(), Color.yellow);
            }
        }
    }

    private void SceneCameraFollowGameCamera()
    {
        if (enableSceneCameraFollowGameCamera)
        {
#if UNITY_EDITOR
            Transform mainCameraTransform = Camera.main.transform;
            SceneView.lastActiveSceneView.pivot = mainCameraTransform.position;
            SceneView.lastActiveSceneView.rotation = mainCameraTransform.rotation;
#endif
        }
    }
}