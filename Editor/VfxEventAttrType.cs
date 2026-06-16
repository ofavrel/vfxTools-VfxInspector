// VFX Inspector — the event-attribute type contract (one descriptor per EventAttrType).
//
// A single source of truth for the per-type knowledge the Send-Event payload editor needs:
// display label + blackboard icon name, the zero/default value, how to read a typed value out of
// the boxed `object` model (Coerce), how to push it onto a VFXEventAttribute (Send), and how to
// pack/unpack it through the flat SessionState DTO buckets (Pack/Unpack). Replaces the parallel
// switches that used to spell this out in VfxInspector.Events.cs (DefaultAttrValue/AttrTypeLabel/
// AttrTypeIcon/SendEventToAll/ToDTO/FromDTO). The pure members (everything but Send) are
// unit-testable without a live VFX — Send is a thin VFXEventAttribute runtime-API delegate.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace VfxInspector.EditorTools
{
    // Payload attribute types, in dropdown order (Float, V2, V3, V4, Bool, Uint, Int). The ordinal
    // doubles as the CustomAttributeUtility.Signature index (see VfxGraphReflection.SignatureIndex).
    internal enum EventAttrType { Float, Vector2, Vector3, Vector4, Bool, Uint, Int }

    internal static class VfxEventAttrType
    {
        internal readonly struct EventAttrTypeInfo
        {
            public readonly string Label;     // short display label ("Vec 3")
            public readonly string IconName;  // VFX blackboard type-icon file stem ("Vector3"/"Integer"/…)
            public readonly object Default;    // zero/default value of the right boxed type
            public readonly Func<object, object> Coerce;   // read a typed value out of the boxed model
            public readonly Action<VFXEventAttribute, string, object> Send; // push onto a VFXEventAttribute
            public readonly Func<object, (Vector4 vec, bool boolVal, int intVal)> Pack;   // -> DTO buckets
            public readonly Func<(Vector4 vec, bool boolVal, int intVal), object> Unpack; // <- DTO buckets

            public EventAttrTypeInfo(string label, string iconName, object def,
                Func<object, object> coerce, Action<VFXEventAttribute, string, object> send,
                Func<object, (Vector4 vec, bool boolVal, int intVal)> pack,
                Func<(Vector4 vec, bool boolVal, int intVal), object> unpack)
            {
                Label = label; IconName = iconName; Default = def;
                Coerce = coerce; Send = send; Pack = pack; Unpack = unpack;
            }
        }

        // Coercers — the canonical "read X out of the boxed object, else its default" used by the
        // value editor, Send and Pack so the narrowing rules live in exactly one place.
        private static bool AsBool(object v) => v is bool b && b;
        private static int AsInt(object v) => v is int i ? i : 0;
        private static uint AsUint(object v) => v is uint u ? u : 0u;
        private static float AsFloat(object v) => v is float f ? f : 0f;
        private static Vector2 AsVec2(object v) => v is Vector2 x ? x : Vector2.zero;
        private static Vector3 AsVec3(object v) => v is Vector3 x ? x : Vector3.zero;
        private static Vector4 AsVec4(object v) => v is Vector4 x ? x : Vector4.zero;

        internal static readonly IReadOnlyDictionary<EventAttrType, EventAttrTypeInfo> Info =
            new Dictionary<EventAttrType, EventAttrTypeInfo>
        {
            [EventAttrType.Float] = new EventAttrTypeInfo("Float", "Float", 0f,
                v => AsFloat(v), (a, n, v) => a.SetFloat(n, AsFloat(v)),
                v => (new Vector4(AsFloat(v), 0, 0, 0), false, 0), d => d.vec.x),

            [EventAttrType.Vector2] = new EventAttrTypeInfo("Vec 2", "Vector2", Vector2.zero,
                v => AsVec2(v), (a, n, v) => a.SetVector2(n, AsVec2(v)),
                v => { var x = AsVec2(v); return (new Vector4(x.x, x.y, 0, 0), false, 0); },
                d => new Vector2(d.vec.x, d.vec.y)),

            [EventAttrType.Vector3] = new EventAttrTypeInfo("Vec 3", "Vector3", Vector3.zero,
                v => AsVec3(v), (a, n, v) => a.SetVector3(n, AsVec3(v)),
                v => { var x = AsVec3(v); return (new Vector4(x.x, x.y, x.z, 0), false, 0); },
                d => new Vector3(d.vec.x, d.vec.y, d.vec.z)),

            [EventAttrType.Vector4] = new EventAttrTypeInfo("Vec 4", "Vector4", Vector4.zero,
                v => AsVec4(v), (a, n, v) => a.SetVector4(n, AsVec4(v)),
                v => (AsVec4(v), false, 0), d => d.vec),

            [EventAttrType.Bool] = new EventAttrTypeInfo("Bool", "Boolean", true,
                v => AsBool(v), (a, n, v) => a.SetBool(n, AsBool(v)),
                v => (Vector4.zero, AsBool(v), 0), d => d.boolVal),

            [EventAttrType.Uint] = new EventAttrTypeInfo("Uint", "Integer", 0u,
                v => AsUint(v), (a, n, v) => a.SetUint(n, AsUint(v)),
                v => (Vector4.zero, false, (int)AsUint(v)), d => (uint)Mathf.Max(0, d.intVal)),

            [EventAttrType.Int] = new EventAttrTypeInfo("Int", "Integer", 0,
                v => AsInt(v), (a, n, v) => a.SetInt(n, AsInt(v)),
                v => (Vector4.zero, false, AsInt(v)), d => d.intVal),
        };
    }
}
