# Changelog

All notable changes to this package are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-06-16
### Added
- **Custom inspector** for the `VisualEffect` component with the full toolset (Properties / Playback /
  Debug / Renderer / All tabs, persistent transport, scene gizmos, the per-particle spreadsheet + CSV
  export). It replaces Unity's stock VFX inspector (a non-Unity assembly's `[CustomEditor]` wins).
- Per-tab **dockable popups** (tear-off): the component gear ▸ *VFX Inspector ▸ \<Tab\>* context menu, or
  right-clicking a tab inside the inspector, opens a native `PropertyEditor` filtered to that one tab.
### Changed
- **Renamed** the package id `com.vfxtools.vfxcontrol` → **`com.vfxtools.vfxinspector`**, the editor
  assembly `VfxTools.VfxControl.Editor` → **`VfxTools.VfxInspector.Editor`**, the namespace
  `VfxControl.EditorTools` → **`VfxInspector.EditorTools`**, the controller/editor/state types
  `VfxControl*` → `VfxInspector*`, and the user-facing name (display name + the *VFX Control* menu paths)
  → **VFX Inspector**. **Breaking:** consuming projects must update the package id in
  `Packages/manifest.json`.
### Removed
- The dockable `EditorWindow` host (`VfxControlWindowHost`), its `Window ▸ VFX Control` menu, and the
  `IVfxHost` host abstraction — the custom inspector + per-tab popups replace the tear-off workflow.

## [0.2.0] - 2026-06-15
### Added
- The full **VFX Control window** tool migrated into the package: Properties / Playback / Debug /
  Renderer tabs, scene-view gizmos, the per-particle readback spreadsheet (+ CSV export), and the
  EditMode test suite (under `Tests/Editor/`).
### Changed
- Asset paths repointed to the package (`Packages/com.vfxtools.vfxcontrol/...`): the USS stylesheet,
  the `Readback/VfxReadback.hlsl` include, and the test `.vfx` fixtures.
- Editor assembly is `VfxTools.VfxControl.Editor` (namespace `VfxControl.EditorTools`).
### Removed
- The 0.1.0 inspector-priority probe (the custom-inspector variant moves to a separate branch).

## [0.1.0] - 2026-06-15
### Added
- Initial UPM package scaffold (`com.vfxtools.vfxcontrol`).
- Inspector-priority **probe**: a minimal `[CustomEditor(typeof(VisualEffect))]` plus a
  *Tools ▸ VFX Control ▸ Probe: Diagnose VisualEffect Editor* menu to confirm this package's editor
  overrides the stock VFX inspector when installed via UPM.
