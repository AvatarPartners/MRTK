#if USING_MRTK
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace SimplifyXR
{
    /// <summary>
    /// Filters incoming MRTK ManipulationData
    /// </summary>
    [DirectiveCategory(DirectiveCategories.Action, DirectiveSubCategory.Filter)]
    public class ManipulationDataFilter : Actions
    {
        ManipulationEventData data;

        public override List<KnobKeywords> ReceiveKeywords()
        {
            return new List<KnobKeywords> { new KnobKeywords("ManipulationEventData", typeof(ManipulationEventData)) };
        }

        public override List<KnobKeywords> SendKeywords()
        {
            return new List<KnobKeywords> {
                new KnobKeywords("GameObject", typeof(GameObject)),
                new KnobKeywords("PointerCentroid", typeof(Vector3)),
                new KnobKeywords("PointerVelocity", typeof(Vector3)),
                new KnobKeywords("PointerAngularVelocity", typeof(Vector3)),
                new KnobKeywords("IsNearInteraction", typeof(bool)),
            };
        }

        public override void Execute()
        {
            GetData();
            SendData();
            ThisActionCompleted();
        }

        void GetData()
        {
            var objectPassed = GetPassableData();
            if (objectPassed != null)
            {
                if (KeywordInUse == "ManipulationEventData")
                {
                    data = (ManipulationEventData)objectPassed;
                }
            }
            else
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "No object to filter on {0}", SimplifyXRDebug.Args(this));
        }

        void SendData()
        {
            var thisData = new List<object> {
                data.ManipulationSource.gameObject,
                data.PointerCentroid,
                data.PointerVelocity,
                data.PointerAngularVelocity,
                data.IsNearInteraction
            };
            var thisKeywords = new List<string> {
                "GameObject",
                "PointerCentroid",
                "PointerVelocity",
                "PointerAngularVelocity",
                "IsNearInteraction",
            };
            AddPassableData(thisKeywords, thisData);
        }
    }
}
#endif