using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class FanPolicyEditorModel
{
    public string PolicyId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ScopeText { get; set; } = string.Empty;
    public string PolicyText { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public string SelectedInputSensorId { get; set; } = string.Empty;
    public string SelectedOutputControlId { get; set; } = string.Empty;
    public IReadOnlyList<SelectionOptionModel> InputSensorOptions { get; set; } = [];
    public IReadOnlyList<SelectionOptionModel> OutputControlOptions { get; set; } = [];
    public IReadOnlyList<FanCurvePointRowModel> CurvePoints { get; set; } = [];
    public string HysteresisText { get; set; } = string.Empty;
    public string RampUpText { get; set; } = string.Empty;
    public string RampDownText { get; set; } = string.Empty;
    public bool IsDirty { get; set; }
    public string DraftStateText { get; set; } = string.Empty;
    public bool CanReset { get; set; }
    public bool CanApplyMockPolicy { get; set; }
}
