#if USING_MRTK
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI;

namespace SimplifyXR
{
    /// <summary>
    /// Custom Editor for ListenForManipulationEvent
    /// </summary>
    [CustomEditor(typeof(ListenForManipulationEvent), true)]
    internal class ListenForManipulationEventDrawer : Editor
    {
        ListenForManipulationEvent directive;
        public override void OnInspectorGUI()
        {
            ListenForManipulationEvent directive = (ListenForManipulationEvent)target;

            directive.GameObjectWithComponent = (GameObject)EditorGUILayout.ObjectField("Object to listen to", directive.GameObjectWithComponent, typeof(GameObject), true);

            if (directive.GameObjectWithComponent != null)
            {
                List<Component> components = directive.GameObjectWithComponent.GetComponents(typeof(Component)).ToList();
                components.RemoveAll(x => IsRemovedType(x));

                string[] componentNames = new string[components.Count];
                for (int i = 0; i < components.Count; i++)
                {
                    componentNames[i] = components[i].ToString();
                }

                int index = 0;
                if (components.Contains(directive.ComponentWithEvent))
                    index = components.IndexOf(directive.ComponentWithEvent);

                index = EditorGUILayout.Popup("Component", index, componentNames);
                directive.ComponentWithEvent = components[index];

                //Reflect out unityEvents
                List<FieldInfo> fields = directive.ComponentWithEvent.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();

                fields.RemoveAll(x => (!x.FieldType.IsSubclassOf(typeof(UnityEvent<ManipulationEventData>))));

                if (fields.Count > 0)
                {
                    List<string> fieldNames = new List<string>();
                    fields.ForEach(delegate (FieldInfo x) { fieldNames.Add(x.Name); });

                    int eventIndex = 0;

                    FieldInfo fi = fields.FirstOrDefault(x => x.Name == directive.eventFieldInfoName);
                    if (fi != null)
                        eventIndex = fields.IndexOf(fi);

                    EditorGUILayout.LabelField("Can only listen to Events with ManipulationEventData");

                    eventIndex = EditorGUILayout.Popup("Event", eventIndex, fieldNames.ToArray());
                    directive.eventFieldInfoName = fields[eventIndex].Name;
                }
                else
                {
                    EditorGUILayout.LabelField("This component has no events which pass ManipulationEventData");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Select A GameObject");
                directive.ComponentWithEvent = null;
            }
        }

        bool IsRemovedType(Component c)
        {
            List<Type> typesToCheck = new List<Type>();
            typesToCheck.Add(typeof(Collider));
            typesToCheck.Add(typeof(Renderer));
            typesToCheck.Add(typeof(Transform));
            typesToCheck.Add(typeof(MeshFilter));
            foreach (Type t in typesToCheck)
            {
                if (c.GetType().IsSubclassOf(t) || c.GetType() == t)
                    return true;
            }
            return false;
        }
    }
}
#endif