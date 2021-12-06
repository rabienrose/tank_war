using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class GridSensorComponentCustom : GridSensorComponent
{
    protected override GridSensorBase[] GetGridSensors()
    {
        return new GridSensorBase[] { new GridSensorCustom(SensorName, CellScale,GridSize,DetectableTags,CompressionType,gameObject)};
    }
}