# PR2 Additions Integration Checklist

Source compared:
- Current branch: compare/current-code-20260511 (HEAD)
- PR branch snapshot: pr-2 (commit 49cf11e7aec10d951ba22f2fd8728ea867a0ec47)
- File scope: RackBuilderMod/RackBuilderCore.cs

## Verified PR-only additions

### Types only in PR
- [ ] RackColorChoice
- [ ] RackColorData
- [ ] RackColorEntry

### Fields only in PR
- [ ] private bool _rackColorsLoaded;
- [ ] private bool _inputSystemHotkeyWarningLogged;
- [ ] private int _selectedRackColorIndex;
- [ ] private bool _bulkEditMode;
- [ ] private int _cableColorIndex;
- [ ] private bool _bulkDragSelecting;
- [ ] private bool _bulkDragTargetSelectState;
- [ ] public readonly string Name;
- [ ] public readonly Color Color;

Note: Name and Color are data fields for PR color-choice data type, not top-level mod state.

### Methods only in PR

#### Rack color system
- [ ] bool ApplyRackColor(Rack rack, Color color)
- [ ] bool TryGetRackColor(Rack rack, out Color color)
- [ ] bool TryGetSavedRackColor(Rack rack, out Color color)
- [ ] Color GetConfiguredRackColor()
- [ ] Color GetRackPreviewColor(Rack rack)
- [ ] RackColorChoice GetSelectedRackColorChoice()
- [ ] void CycleRackColorChoice()
- [ ] void EnsureRackColorsLoaded()
- [ ] void RestoreSavedRackColors()
- [ ] void SaveRackColors()
- [ ] string GetRackColorsFilePath()
- [ ] Color GetHoverColor(Color baseColor)
- [ ] Color GetReadableTextColor(Color bg)
- [ ] float Clamp01(float value)
- [ ] void AddColorChannelRow(string channelName, float value, Action<float> onChanged)

#### Bulk rack selection and actions
- [ ] void AddBulkSelectableServerRow(int anchor, int size, string text, Color baseColor)
- [ ] void SetBulkServerSelection(int anchor, bool selected)
- [ ] void ShowBulkRackContents()
- [ ] void BulkOpenSelectedRacks()
- [ ] void BulkRemoveSelectedRacks()
- [ ] void BulkRemoveAllEquipment()
- [ ] void BulkSetRackColor()
- [ ] int FindFreeSlot(Rack rack, int size)

#### Input and mode helpers
- [ ] bool IsCtrlHeld()
- [ ] bool IsRackManagerTogglePressed()

#### Cable color/perf helpers
- [ ] RackColorChoice GetSelectedCableColor()
- [ ] void CycleCableColorChoice()
- [ ] void PreWarmCableClips()

#### Shared method with signature drift
- [ ] bool CreateCable(CablePositions cablePositions, CableLink startPort, CableLink endPort, IEnumerable<Transform> waypoints, CableLink.TypeOfLink startType, CableLink.TypeOfLink endType, string serverId = "")

Current HEAD signature to preserve while integrating:
- bool CreateCable(CablePositions cablePositions, CableLink startPort, CableLink endPort, IEnumerable<Transform> waypoints, CableLink.TypeOfLink startType, CableLink.TypeOfLink endType, string serverId = "", int customerId = -1)

## Clean integration policy

- Keep current robust cleanup pipeline intact:
  - RemoveCableIdEverywhere
  - ReconcileSelectedRackUsageState
  - pending-removal filtering and deferred refresh behavior
- Keep current low-overhead frame behavior intact:
  - conditional modal enforcement and empty-set short-circuits
- Port PR additions by subsystem, then adapt call sites to existing method contracts.
- Never replace current cleanup/perf logic with PR variants without explicit parity checks.

## Integration order

- [ ] Add PR color data types and minimal state fields.
- [ ] Add rack color load/save and color selection helpers.
- [ ] Add bulk selection UI helpers and actions.
- [ ] Add cable color helpers and clip prewarm helper.
- [ ] Adapt PR call sites to current CreateCable signature (retain customerId support).
- [ ] Compile and resolve errors.
- [ ] Validate behavior: bulk remove counters, ghost cable cleanup, rack color persistence, bulk-edit flows, idle overhead.
- [ ] Produce final comparison artifact for PR review.