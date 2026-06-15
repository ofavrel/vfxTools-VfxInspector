// VFX Control — custom Inspector host for VisualEffect.
//
// Hosts the shared VfxControl controller inside the Inspector instead of a window. Wins over the VFX
// package's stock inspector (non-Unity assembly takes precedence — see Documentation~/VfxControl.md).
// Drives the edited set from Editor.targets (no scene-selection tracking) and routes gizmos via the
// controller's scene-GUI hook.
//
// Per-tab popups (tear-off): either the component context menu (gear ▸ / right-click) "VFX Control ▸
// <Tab>" entries or right-clicking a tab inside the inspector open Unity's native dockable PropertyEditor
// (EditorUtility.OpenPropertyEditor) filtered to one tab. The chosen tab is handed to the freshly created
// inspector via a static "pending solo tab" (consumed in OnEnable), then the controller's solo machinery
// (IsSolo → pin _tab + hide the tab strip) does the rest.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace VfxControl.EditorTools
{
    [CustomEditor(typeof(VisualEffect))]
    [CanEditMultipleObjects]
    public sealed class VfxControlInspector : Editor
    {
        private VfxControl _ctrl;
        private VisualElement _root;
        private string _soloTab; // non-null in a per-tab popup; null for the normal (full) inspector

        // Read by the controller: the root it builds into + the pinned tab of a per-tab popup (null = full).
        internal VisualElement Root => _root;
        internal string SoloTab => _soloTab;
        // Right-click a tab in the inspector → open it as a dockable popup pinned to that tab (label unused).
        internal void OpenSolo(string tabId, string label) => OpenTabPopup(target, tabId);

        private void OnEnable()
        {
            // Consume the hand-off from a "VFX Control ▸ <Tab>" context command, if this editor is the one
            // it just opened. A normal main-Inspector editor sees null here → full inspector.
            _soloTab = s_pendingSoloTab;
            s_pendingSoloTab = null;
        }

        public override VisualElement CreateInspectorGUI()
        {
            _ctrl?.Disable();                    // guard against a re-bind creating a second controller
            _root = new VisualElement();
            _ctrl = new VfxControl(this);
            _ctrl.Enable();
            _ctrl.SetTargets(GetTargets());      // fixed targets, not scene selection
            _ctrl.Rebuild();
            return _root;
        }

        private void OnDisable()
        {
            _ctrl?.Disable();
            _ctrl = null;
        }

        private List<VisualEffect> GetTargets()
        {
            var list = new List<VisualEffect>();
            foreach (var t in targets)
                if (t is VisualEffect ve) list.Add(ve);
            return list;
        }

        // ---- per-tab dockable popups (component context menu) ----------------------------------------
        // The next VfxControlInspector created after one of these consumes it (see OnEnable).
        private static string s_pendingSoloTab;

        [MenuItem("CONTEXT/VisualEffect/VFX Control/Properties")] private static void OpenProps(MenuCommand c) => OpenTab(c, "props");
        [MenuItem("CONTEXT/VisualEffect/VFX Control/Playback")]   private static void OpenPlay(MenuCommand c) => OpenTab(c, "play");
        [MenuItem("CONTEXT/VisualEffect/VFX Control/Renderer")]   private static void OpenRender(MenuCommand c) => OpenTab(c, "render");
        [MenuItem("CONTEXT/VisualEffect/VFX Control/Debug")]      private static void OpenDebug(MenuCommand c) => OpenTab(c, "debug");

        private static void OpenTab(MenuCommand command, string tab) => OpenTabPopup(command?.context, tab);

        private static void OpenTabPopup(Object obj, string tab)
        {
            if (obj == null) return;
            s_pendingSoloTab = tab;
            EditorUtility.OpenPropertyEditor(obj); // native dockable PropertyEditor → our inspector, pinned to `tab`
        }

        // ---- diagnostics ----------------------------------------------------------------------------
        // Logs exactly where exposed-property enumeration succeeds or fails for the selected/target VFX.
        [MenuItem("Tools/VFX Control/Diagnose Target")]
        private static void Diagnose()
        {
            var go = Selection.activeGameObject;
            var ve = go != null ? go.GetComponent<VisualEffect>() : Selection.activeObject as VisualEffect;
            var asset = ve != null ? ve.visualEffectAsset : Selection.activeObject as VisualEffectAsset;

            Debug.Log($"[VFX Control] Diagnose — component={(ve != null ? ve.name : "null")}, " +
                      $"persistent={(ve != null && EditorUtility.IsPersistent(ve))}, " +
                      $"asset={(asset != null ? asset.name : "null")}");
            Debug.Log($"[VFX Control] Binding: {VfxGraphReflection.DescribeBindingState()}");

            VfxGraphReflection.Verbose = true;
            try
            {
                var ps = VfxGraphReflection.GetExposedParameters(asset);
                Debug.Log($"[VFX Control] Enumerated {ps.Count} parameter(s): " +
                          string.Join(", ", ps.Select(p => $"{p.Name}[{p.SheetType}/{p.RealType}] cat='{p.Category}'")));

                var evts = VfxGraphReflection.GetEventNames(asset);
                Debug.Log($"[VFX Control] Custom events ({evts.Count}): {string.Join(", ", evts)}");

                var customs = VfxGraphReflection.GetCustomAttributes(asset);
                Debug.Log($"[VFX Control] Custom attributes ({customs.Count}): " +
                          string.Join(", ", customs.Select(c => $"{c.name}#{c.type}")));
            }
            finally { VfxGraphReflection.Verbose = false; }
        }
    }
}
