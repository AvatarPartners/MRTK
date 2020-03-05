#if USING_MRTK
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;
using Microsoft.MixedReality.Toolkit.UI;

namespace SimplifyXR
{
    /// <summary>
    /// Sends the manipulation event data from a unity event
    /// </summary>
    [DirectiveCategory(DirectiveCategories.Initiator, DirectiveSubCategory.Events)]
    public class ListenForManipulationEvent : Initiator
    {
        public GameObject GameObjectWithComponent;
        public Component ComponentWithEvent;
        public string eventFieldInfoName;
        public UnityEvent theEvent;

        public override List<KnobKeywords> ReceiveKeywords()
        {
            return new List<KnobKeywords>();
        }

        public override List<KnobKeywords> SendKeywords()
        {
            return new List<KnobKeywords>(){
                new KnobKeywords("EventData", typeof(ManipulationEventData)),
            };
        }

        protected new void Awake()
        {
            base.Awake();
            if (string.IsNullOrEmpty(eventFieldInfoName))
            {
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "No event selected on {0}. Ensure this event passes ManipulationEventData.", SimplifyXRDebug.Args(gameObject.name));
                return;
            }

            //Register for the event
            List<FieldInfo> fields = ComponentWithEvent.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            fields.RemoveAll(x => !x.Name.Equals(eventFieldInfoName));

            if (fields.Count == 0)
            {
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "No event selected on {0}. Ensure this event passes ManipulationEventData.", SimplifyXRDebug.Args(gameObject.name));
                return;
            }

            Type theEventType = fields[0].FieldType;

            if (theEventType.GetGenericArguments().Length == 0)
            {
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.Warning, "Can only listen to events with ManipulationEventData", SimplifyXRDebug.Args());
            }
            else if (theEventType.GetGenericArguments().Length == 1)
            {
                Type[] types = theEventType.GetGenericArguments();
                if (types.Length == 1)
                {
                    UnityEvent<ManipulationEventData> singleEventInstance = (UnityEvent<ManipulationEventData>)((fields[0].GetValue(ComponentWithEvent)));
                    singleEventInstance.AddListener(SingleTypeInitiate);
                }
            }
        }
        protected new void OnDestroy()
        {
            List<FieldInfo> fields = ComponentWithEvent.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            fields.RemoveAll(x => !x.Name.Equals(eventFieldInfoName));
            ((UnityEvent<ManipulationEventData>)fields[0].GetValue(ComponentWithEvent)).RemoveListener(SingleTypeInitiate);
            base.OnDestroy();
        }
        public void SingleTypeInitiate(ManipulationEventData data)
        {
            SendData(data);
            base.Initiate();
        }
        void SendData(ManipulationEventData data)
        {
            var thisData = new List<object> { data };
            var thisKeywords = new List<string> { "EventData" };
            AddPassableData(thisKeywords, thisData);
        }
    }
}
#endif