using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjectDatas/EventTriggerData")]
public class EventTriggerData : ScriptableObject
{
    [Serializable]
    public class JsonData
    {
        public bool used;
        public Vector2 targetPosition;

        public static implicit operator JsonData(EventTriggerData data)
        {
            return new JsonData {
                used = data.used,
                targetPosition = data.targetPosition
            };
        }
    }

    public bool used;
    public Vector2 targetPosition;

    public void Init()
    {
        used = false;
        targetPosition = Vector2.zero;
    }
    public void Init(JsonData eventTriggerData)
    {
        used = eventTriggerData.used;
        targetPosition = eventTriggerData.targetPosition;
    }


#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            used = false;
            targetPosition = Vector2.zero;
        }
    }
#endif
}
