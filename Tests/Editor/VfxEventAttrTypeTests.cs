// Unit tests for VfxEventAttrType — the per-EventAttrType descriptor table.
// Pure: no live VFX / no fixture. Covers the table's completeness, the Coerce narrowing rules, and
// the Pack/Unpack DTO round-trip (the bug-prone bit the old ToDTO/FromDTO switches hand-wrote).

using System;
using NUnit.Framework;
using UnityEngine;

namespace VfxInspector.EditorTools.Tests
{
    public class VfxEventAttrTypeTests
    {
        static readonly EventAttrType[] AllTypes = (EventAttrType[])Enum.GetValues(typeof(EventAttrType));

        [Test]
        public void EveryType_HasACompleteEntry()
        {
            foreach (var t in AllTypes)
            {
                Assert.That(VfxEventAttrType.Info.ContainsKey(t), $"missing table entry for {t}");
                var info = VfxEventAttrType.Info[t];
                Assert.That(info.Label, Is.Not.Null.And.Not.Empty, $"{t} label");
                Assert.That(info.IconName, Is.Not.Null.And.Not.Empty, $"{t} icon name");
                Assert.That(info.Default, Is.Not.Null, $"{t} default");
                Assert.That(info.Coerce, Is.Not.Null);
                Assert.That(info.Send, Is.Not.Null);
                Assert.That(info.Pack, Is.Not.Null);
                Assert.That(info.Unpack, Is.Not.Null);
            }
        }

        [Test]
        public void Coerce_FallsBackToDefault_OnTypeMismatch()
        {
            Assert.That(VfxEventAttrType.Info[EventAttrType.Float].Coerce("not a float"), Is.EqualTo(0f));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Int].Coerce(null), Is.EqualTo(0));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Uint].Coerce(-1), Is.EqualTo(0u));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Bool].Coerce(123), Is.EqualTo(false));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Vector3].Coerce("x"), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void Coerce_PassesThrough_MatchingValue()
        {
            Assert.That(VfxEventAttrType.Info[EventAttrType.Float].Coerce(3.5f), Is.EqualTo(3.5f));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Int].Coerce(42), Is.EqualTo(42));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Uint].Coerce(7u), Is.EqualTo(7u));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Bool].Coerce(true), Is.EqualTo(true));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Vector2].Coerce(new Vector2(1, 2)), Is.EqualTo(new Vector2(1, 2)));
            Assert.That(VfxEventAttrType.Info[EventAttrType.Vector4].Coerce(new Vector4(1, 2, 3, 4)), Is.EqualTo(new Vector4(1, 2, 3, 4)));
        }

        // The ToDTO/FromDTO contract: a value packs into the flat DTO buckets and unpacks back unchanged.
        static void AssertRoundTrip(EventAttrType t, object value)
        {
            var info = VfxEventAttrType.Info[t];
            var packed = info.Pack(value);
            Assert.That(info.Unpack(packed), Is.EqualTo(value), $"{t} round-trip");
        }

        [Test]
        public void PackUnpack_RoundTrips_EveryType()
        {
            AssertRoundTrip(EventAttrType.Float, 3.5f);
            AssertRoundTrip(EventAttrType.Vector2, new Vector2(1, 2));
            AssertRoundTrip(EventAttrType.Vector3, new Vector3(1, 2, 3));
            AssertRoundTrip(EventAttrType.Vector4, new Vector4(1, 2, 3, 4));
            AssertRoundTrip(EventAttrType.Bool, true);
            AssertRoundTrip(EventAttrType.Uint, 7u);
            AssertRoundTrip(EventAttrType.Int, -5);
        }

        [Test]
        public void Unpack_ClampsNegativeUintToZero()
        {
            // Uint packs through intVal; a negative DTO bucket must clamp back to 0u (matches old FromDTO).
            object v = VfxEventAttrType.Info[EventAttrType.Uint].Unpack((Vector4.zero, false, -3));
            Assert.That(v, Is.EqualTo(0u));
        }
    }
}
