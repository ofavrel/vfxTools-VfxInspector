# VFX Control

A denser, more controllable editor for Unity's `VisualEffect` component, distributed as a UPM package.

It's a **dockable `EditorWindow`** (not a `[CustomEditor]`, to avoid conflicting with the VFX package's
own inspector) that follows the selected scene Visual Effect and offers:

- **Properties** — categorized, struct-aware exposed parameters with typed controls, favorites, modified
  markers, copy/paste, constrain-proportions lock, and a category enable gate.
- **Playback** — a persistent transport (scrub/play/step/loop/Rate) + Duration, Seed/Reseed, Initial
  Event, and a Send-Event panel with a `VFXEventAttribute` payload editor.
- **Debug** — live statistics (incl. CPU/GPU time + attribute memory), per-system capacity bars, texture
  usage, a Show Bounds visualizer, and an opt-in **per-particle attribute spreadsheet** (GraphicsBuffer
  readback) with multi-system support, a scene overlay, Alt-click framing, and **CSV export**.
- **Renderer** — the sibling `VFXRenderer` settings (Probes + Additional).
- **Scene gizmos** — custom edit handles for spaceable struct properties (Position/Direction/Box/
  Cone/Sphere/Circle/Torus/Line/Plane/Transform).

## Requirements
- Unity **6000.0+**
- `com.unity.visualeffectgraph` installed in the project

## Install
**Package Manager ▸ + ▸ Add package from disk…** → select this package's `package.json` (or **Add package
from git URL…** with the repo URL).

## Usage
Open **Window ▸ VFX Control**, then select a GameObject with a `VisualEffect` in the scene.

The opt-in particle spreadsheet needs the graph instrumented with a Custom HLSL block pointing at
`Packages/com.vfxtools.vfxinspector/Readback/VfxReadback.hlsl` — see the in-panel help and the
architecture notes in `Documentation~/VfxInspector.md`.

## License
MIT — see [LICENSE.md](LICENSE.md).
