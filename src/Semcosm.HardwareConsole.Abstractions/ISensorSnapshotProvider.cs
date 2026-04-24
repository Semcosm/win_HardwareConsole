using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface ISensorSnapshotProvider
{
    IReadOnlyList<SensorValue> GetCurrentSensorValues();
}
