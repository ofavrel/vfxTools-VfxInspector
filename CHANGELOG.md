# Changelog

All notable changes to this package are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-06-15
### Added
- The full **VFX Control window** tool migrated into the package: Properties / Playback / Debug /
  Renderer tabs, scene-view gizmos, the per-particle readback spreadsheet (+ CSV export), and the
  EditMode test suite (under `Tests/Editor/`).
### Changed
- Asset paths repointed to the package (`Packages/com.vfxtools.vfxinspector/...`): the USS stylesheet,
  the `Readback/VfxReadback.hlsl` include, and the test `.vfx` fixtures.
- Editor assembly is `VfxTools.VfxInspector.Editor` (namespace `VfxInspector.EditorTools` unchanged).
### Removed
- The 0.1.0 inspector-priority probe (the custom-inspector variant moves to a separate branch).

## [0.1.0] - 2026-06-15
### Added
- Initial UPM package scaffold (`com.vfxtools.vfxinspector`).
- Inspector-priority **probe**: a minimal `[CustomEditor(typeof(VisualEffect))]` plus a
  *Tools ▸ VFX Control ▸ Probe: Diagnose VisualEffect Editor* menu to confirm this package's editor
  overrides the stock VFX inspector when installed via UPM.
