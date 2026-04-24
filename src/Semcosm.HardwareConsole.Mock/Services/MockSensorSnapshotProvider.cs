using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockSensorSnapshotProvider : ISensorSnapshotProvider
{
    public IReadOnlyList<SensorValue> GetCurrentSensorValues() => MockHardwareData.CurrentSensorValues;
}
