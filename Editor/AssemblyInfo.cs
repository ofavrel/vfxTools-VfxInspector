// Exposes the tool's `internal` types (VfxGraphReflection, VfxPropertySheet, VfxInspectorState, …)
// to the EditMode test assembly only. No effect on production behavior.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VfxTools.VfxInspector.Editor.Tests")]
