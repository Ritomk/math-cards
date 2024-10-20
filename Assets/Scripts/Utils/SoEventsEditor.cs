using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Event, Inherited = true)]
public class DebugEventAttribute : Attribute
{
}

[CustomEditor(typeof(ScriptableObject), true)]
public class DebugEventEditor : Editor
{
    private bool showDebugEvents = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var targetObject = target;
        var targetType = targetObject.GetType();

        // Find all events with the [DebugEvent] attribute
        var events = targetType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(e => e.IsDefined(typeof(DebugEventAttribute), true))
            .ToArray();

        if (events.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Events", EditorStyles.boldLabel);

            showDebugEvents = EditorGUILayout.Foldout(showDebugEvents, "Show Debug Events");
            if (showDebugEvents)
            {
                foreach (var eventInfo in events)
                {
                    DisplayEventSubscribers(eventInfo, targetObject);
                }
            }
        }
    }

    private void DisplayEventSubscribers(EventInfo eventInfo, object targetObject)
    {
        EditorGUILayout.LabelField($"Event: {eventInfo.Name}", EditorStyles.boldLabel);

        var backingField = GetEventBackingField(eventInfo);
        if (backingField != null)
        {
            Delegate eventDelegate = backingField.GetValue(targetObject) as Delegate;
            Delegate[] subscribers = eventDelegate?.GetInvocationList();

            if (subscribers != null && subscribers.Length > 0)
            {
                EditorGUILayout.LabelField("Subscribers:");
                foreach (var subscriber in subscribers)
                {
                    string subscriberInfo = $" - Method: {subscriber.Method.DeclaringType}.{subscriber.Method.Name}";
                    EditorGUILayout.LabelField(subscriberInfo);
                    EditorGUILayout.LabelField($" Target: {subscriber.Target}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("No subscribers.");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Unable to access event backing field.");
        }

        EditorGUILayout.Space();
    }

    private FieldInfo GetEventBackingField(EventInfo eventInfo)
    {
        // Common naming conventions for event backing fields
        string[] possibleFieldNames = {
            eventInfo.Name,
            $"_{eventInfo.Name}",
            $"<{eventInfo.Name}>k__BackingField",
            $"add_{eventInfo.Name}",
            $"remove_{eventInfo.Name}"
        };

        BindingFlags allBindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        foreach (var fieldName in possibleFieldNames)
        {
            var field = eventInfo.DeclaringType.GetField(fieldName, allBindings);
            if (field != null && typeof(Delegate).IsAssignableFrom(field.FieldType))
            {
                return field;
            }
        }

        // As a last resort, search all fields with matching delegate types
        var fields = eventInfo.DeclaringType.GetFields(allBindings)
            .Where(f => typeof(Delegate).IsAssignableFrom(f.FieldType));

        foreach (var field in fields)
        {
            if (field.FieldType == eventInfo.EventHandlerType)
                return field;
        }

        return null;
    }
}