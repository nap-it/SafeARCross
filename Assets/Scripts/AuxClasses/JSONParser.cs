using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


public class JSONParser
{
    public static MAPEMData ParseMAPEM(string jsonString)
    {
        var mapemData = new MAPEMData
        {
            LaneCoordinatesDict = new Dictionary<int, List<LaneCoordinates>>()
        };

        JObject json = JObject.Parse(jsonString);
        JArray intersections = (JArray)json["intersections"];

        foreach (var intersection in intersections)
        {
            // Parse laneWidth and refPoint
            mapemData.LaneWidth = intersection["laneWidth"].Value<double>();
            var refPoint = intersection["refPoint"];
            mapemData.RefPoint = new LaneCoordinates
            {
                Latitude = refPoint["lat"].Value<double>(),
                Longitude = refPoint["long"].Value<double>()
            };

            // Parse laneSet
            JArray laneSet = (JArray)intersection["laneSet"];
            foreach (var lane in laneSet)
            {
                int laneId = lane["laneID"].Value<int>();
                JArray nodes = (JArray)lane["nodeList"]["nodes"];

                var coordinatesList = new List<LaneCoordinates>();
                foreach (var node in nodes)
                {
                    var lat = node["delta"]["node-LatLon"]["lat"].Value<double>();
                    var lon = node["delta"]["node-LatLon"]["lon"].Value<double>();
                    coordinatesList.Add(new LaneCoordinates { Latitude = lat, Longitude = lon });
                }

                if (!mapemData.LaneCoordinatesDict.ContainsKey(laneId))
                {
                    mapemData.LaneCoordinatesDict.Add(laneId, coordinatesList);
                }
            }
        }

        return mapemData;
    }

    /*
    public static Dictionary<int, int> ParseSPATEM(string jsonString)
    {
        var signalGroupStates = new Dictionary<int, int>();

        JObject json = JObject.Parse(jsonString);
        JArray intersections = (JArray)json["intersections"];

        // Get stationID
        int stationId = json["stationID"].Value<int>();
        if (stationId != 1048660)
        {
            Debug.LogError("Station ID is not 1048660");
        }

        foreach (var intersection in intersections)
        {
            JArray states = (JArray)intersection["states"];
            foreach (var state in states)
            {
                int signalGroup = state["signalGroup"].Value<int>();
                int eventState = state["state-time-speed"][0]["eventState"].Value<int>();

                signalGroupStates[signalGroup] = eventState;
            }
        }

        return signalGroupStates;
    }*/
    
    public static Dictionary<int, int> ParseSPATEM(string jsonString)
    {
        var signalGroupStates = new Dictionary<int, int>();

        // Log the raw JSON string to check its integrity
        //Debug.Log("Received JSON string: " + jsonString);

        try
        {
            JObject json = JObject.Parse(jsonString);

            // Log successful parsing
            //Debug.Log("JSON parsed successfully");

            JArray intersections = (JArray)json["fields"]["spat"]["intersections"];

            // Get stationID
            int stationId = json["stationID"].Value<int>();
            if (stationId != 1048660)
            {
                Debug.LogError("Station ID is not 1048660");
                return signalGroupStates;
            }

            foreach (var intersection in intersections)
            {
                JArray states = (JArray)intersection["states"];
                foreach (var state in states)
                {
                    int signalGroup = state["signalGroup"].Value<int>();
                    int eventState = state["state-time-speed"][0]["eventState"].Value<int>();

                    if (signalGroup == 15) signalGroupStates[signalGroup] = eventState;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to parse JSON: " + ex.Message);
        }

        return signalGroupStates;
    }
}
