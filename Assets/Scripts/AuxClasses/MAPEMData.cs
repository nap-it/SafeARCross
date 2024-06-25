using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MAPEMData
{
    public Dictionary<int, List<LaneCoordinates>> LaneCoordinatesDict { get; set; }
    public double LaneWidth { get; set; }
    public LaneCoordinates RefPoint { get; set; }
}
