using System;
using System.Diagnostics;
using UnityEngine;

namespace Hlight.Serialization.InspectorSerializationTool
{
    /// <summary>
    /// Draws Serialize/Deserialize controls under the decorated member so its value can be
    /// copied out of, or pasted into, the Inspector. Editor-only: the attribute is not emitted
    /// in player builds.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class ShowInspectorSerializationToolAttribute : PropertyAttribute
    {
    }
}
