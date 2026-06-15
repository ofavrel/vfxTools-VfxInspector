// EditMode tests for VfxInspectorState — the UI-state persistence layer.
//
// Focused on the logic unique to our code: the per-asset set serialization (EditorPrefs, '\n'
// joined/split) and the global timeline-duration clamp + loop default. The trivial SessionState
// pass-throughs (Tab/Filter/Search/Sections) are deliberately NOT tested — they'd clobber the live
// window's session state for no real coverage. Per-asset keys use a throwaway GUID and are deleted
// in TearDown; the two global keys are saved and restored so the suite never pollutes real prefs.

using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using VfxInspector.EditorTools;

namespace VfxInspector.EditorTools.Tests
{
    public class VfxInspectorStateTests
    {
        string _guid;
        float _origDuration;
        bool _origLoop;

        [SetUp]
        public void SetUp()
        {
            _guid = System.Guid.NewGuid().ToString("N"); // isolated per-asset namespace
            _origDuration = VfxInspectorState.GetTimelineDuration();
            _origLoop = VfxInspectorState.GetLoop();
        }

        [TearDown]
        public void TearDown()
        {
            // per-asset keys this fixture may have written (format mirrors VfxInspectorState)
            EditorPrefs.DeleteKey($"vfxctrl.{_guid}.favorites");
            EditorPrefs.DeleteKey($"vfxctrl.{_guid}.collapsed");
            EditorPrefs.DeleteKey($"vfxctrl.{_guid}.constrained");
            // restore the globals we touched
            VfxInspectorState.SetTimelineDuration(_origDuration);
            VfxInspectorState.SetLoop(_origLoop);
        }

        [Test]
        public void Favorites_RoundTrip_PreservesAllEntries()
        {
            var state = new VfxInspectorState(_guid);
            var saved = new HashSet<string> { "prop:Mass", "renderer:m_Priority", "play:sendevent" };
            state.SaveFavorites(saved);

            var loaded = new VfxInspectorState(_guid).LoadFavorites();
            Assert.That(loaded, Is.EquivalentTo(saved));
        }

        [Test]
        public void Collapsed_RoundTrip_HandlesStructKeysWithColons()
        {
            var state = new VfxInspectorState(_guid);
            // keys contain ':' (e.g. "struct:Bounds", "debug:live") — must survive the '\n' serialization
            var saved = new HashSet<string> { "struct:Bounds", "debug:live", "Favorites" };
            state.SaveCollapsed(saved);

            Assert.That(new VfxInspectorState(_guid).LoadCollapsed(), Is.EquivalentTo(saved));
        }

        [Test]
        public void Constrained_EmptySet_RoundTripsToEmpty()
        {
            var state = new VfxInspectorState(_guid);
            state.SaveConstrained(new HashSet<string>());
            Assert.That(new VfxInspectorState(_guid).LoadConstrained(), Is.Empty);
        }

        [Test]
        public void LoadFavorites_UntouchedAsset_IsEmpty()
        {
            // a fresh GUID has nothing stored
            Assert.That(new VfxInspectorState(_guid).LoadFavorites(), Is.Empty);
        }

        [Test]
        public void TimelineDuration_ClampsToMinimum()
        {
            VfxInspectorState.SetTimelineDuration(0.0001f);
            Assert.That(VfxInspectorState.GetTimelineDuration(), Is.EqualTo(0.1f).Within(1e-4f));
        }

        [Test]
        public void TimelineDuration_NormalValue_RoundTrips()
        {
            VfxInspectorState.SetTimelineDuration(7.5f);
            Assert.That(VfxInspectorState.GetTimelineDuration(), Is.EqualTo(7.5f).Within(1e-4f));
        }

        [Test]
        public void Loop_RoundTrips()
        {
            VfxInspectorState.SetLoop(false);
            Assert.That(VfxInspectorState.GetLoop(), Is.False);
            VfxInspectorState.SetLoop(true);
            Assert.That(VfxInspectorState.GetLoop(), Is.True);
        }
    }
}
