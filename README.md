# VFX Inspector

A denser, more controllable editor for Unity's `VisualEffect` component, distributed as a UPM package.

It's a **custom inspector** (`[CustomEditor(typeof(VisualEffect))]`) that replaces Unity's stock VFX
inspector — a non-Unity assembly's editor takes precedence — and offers:

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
Select a GameObject with a `VisualEffect` component — this inspector replaces Unity's stock one. To tear a
single tab off into its own dockable window, use the component's **gear ▸ VFX Inspector ▸ \<Tab\>** menu (or
right-click a tab inside the inspector). Diagnostics: **Tools ▸ VFX Inspector ▸ Diagnose Target**.

The opt-in particle spreadsheet needs the graph instrumented with a Custom HLSL block pointing at
`Packages/com.vfxtools.vfxinspector/Readback/VfxReadback.hlsl` — see the in-panel help and the
architecture notes in `Documentation~/VfxInspector.md`.

## License
MIT — see [LICENSE.md](LICENSE.md).
