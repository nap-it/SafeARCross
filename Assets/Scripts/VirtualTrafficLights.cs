using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualTrafficLights : MonoBehaviour
{
    private bool enabled = true;
    private MAPEMData mapemData;
    private Dictionary<int, int> spatemData;
    public GameObject trafficLightPrefab;
    public GameObject greenLight;
    public GameObject redLight;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private bool manualControl = false;
    public bool debug = false;


    // Start is called before the first frame update
    void Start()
    {
        TurnOffLights();
        //SetVAMCoordinates(40.634825, -8.660323); // LaneID -1
        //SetVAMCoordinates(40.630419, -8.654414); // LaneID 1
        //SetVAMCoordinates(40.630389, -8.654323); // LaneID 2
        //SetVAMCoordinates(40.630676, -8.653668); // LaneID 7
        //trafficLightPrefab.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Don't send the messages if it's not enabled
        if (!enabled || manualControl) return;
        if (spatemData == null) return;
        
        foreach (KeyValuePair<int, int> trafficLight in spatemData)
        {
            Debug.Log("Traffic Light ID: " + trafficLight.Key + " State: " + trafficLight.Value);
            if (trafficLight.Key == 15)
            {
                Debug.Log("Traffic Light ID: " + trafficLight.Key + " State: " + trafficLight.Value);
                if (trafficLight.Value == 3)
                {
                    TurnOnRedLight();
                }
                else if (trafficLight.Value == 5)
                {
                    TurnOnGreenLight();
                }            
            }
        }

        /*
        if (mapemData != null)
        {
            int laneID = FindLaneId(mapemData, vamLatitude, vamLongitude);
            Debug.Log("Lane ID: " + laneID);
            if (laneID != -1)
            {
                trafficLightPrefab.SetActive(true);
                
                if (spatemData != null)
                {
                    if (spatemData[laneID] == 3)
                    {
                        TurnOnRedLight();
                    }

                    if (spatemData[laneID] == 6)
                    {
                        TurnOnGreenLight();
                    }
                }
                else
                {
                    TurnOffLights();
                }
            }
            else 
            {
                trafficLightPrefab.SetActive(false);
            }
        }
        */
    }

    public void SystemToggle()
    {
        enabled = !enabled;
        if (!enabled) trafficLightPrefab.SetActive(false);
        Debug.Log("Warning System Enabled: " + enabled);
    }

    public void SetLight(string value)
    {
        if (value == "red")
        {
            TurnOnRedLight();
        } 
        else if (value == "green")
        {
            TurnOnGreenLight();
        } 
        else if (value == "off")
        {
            TurnOffLights();
        } 
        else if (value == "no")
        {
            trafficLightPrefab.SetActive(false);
            manualControl = false;
        } 
        else if (value == "yes")
        {
            Debug.Log("Manual control enabled");
            trafficLightPrefab.SetActive(true);
            manualControl = true;
        }
    }

    public void DemoSystem()
    {
        trafficLightPrefab.SetActive(true);
        TurnOnRedLight();
        StartCoroutine(WaitAndActivate());
    }

    public IEnumerator WaitAndActivate()
    {
        yield return new WaitForSeconds(5);
        TurnOnGreenLight();
        yield return new WaitForSeconds(5);
        trafficLightPrefab.SetActive(false);
    }

    public static int FindLaneId(MAPEMData mapemData, double currentLat, double currentLon)
    {
        foreach (var lane in mapemData.LaneCoordinatesDict)
        {
            for (int i = 0; i < lane.Value.Count - 1; i++)
            {
                if (IsPointWithinLane(currentLat, currentLon, lane.Value[i], lane.Value[i + 1], mapemData.LaneWidth))
                {
                    return lane.Key; // Return the lane ID if the point is near the line
                }
            }
        }

        return -1; // Return -1 if the point is not near any lane
    }

    private static bool IsPointWithinLane(double pointLat, double pointLon, LaneCoordinates start, LaneCoordinates end, double laneWidth)
    {
        // Calculate the bounding box for each line segment
        var boundingBox = CalculateBoundingBox(start, end, laneWidth);

        // Check if the point is within the bounding box
        return pointLat >= boundingBox.Item1.Latitude && pointLat <= boundingBox.Item2.Latitude &&
               pointLon >= boundingBox.Item1.Longitude && pointLon <= boundingBox.Item2.Longitude;
    }

    private static Tuple<LaneCoordinates, LaneCoordinates> CalculateBoundingBox(LaneCoordinates start, LaneCoordinates end, double laneWidth)
    {
        double latDiff = end.Latitude - start.Latitude;
        double lonDiff = end.Longitude - start.Longitude;

        // Calculate offset for latitude and longitude based on lane width
        // This is a simplistic approach and works for small distances. 
        // For larger distances or more accuracy, consider using a geospatial library to account for Earth's curvature and coordinate system
        double latOffset = (laneWidth / 111111) * Math.Sqrt(1 / (1 + (lonDiff / latDiff) * (lonDiff / latDiff))); // 111111 is an approximate value for meters per degree latitude
        double lonOffset = (laneWidth / 111111) * Math.Sqrt(1 / (1 + (latDiff / lonDiff) * (latDiff / lonDiff))); // Adjust for longitude, varies with latitude

        // Create bounding box coordinates
        LaneCoordinates minCoords = new LaneCoordinates
        {
            Latitude = Math.Min(start.Latitude, end.Latitude) - latOffset,
            Longitude = Math.Min(start.Longitude, end.Longitude) - lonOffset
        };

        LaneCoordinates maxCoords = new LaneCoordinates
        {
            Latitude = Math.Max(start.Latitude, end.Latitude) + latOffset,
            Longitude = Math.Max(start.Longitude, end.Longitude) + lonOffset
        };

        return new Tuple<LaneCoordinates, LaneCoordinates>(minCoords, maxCoords);
    }

    public void SetSPATEM (Dictionary<int, int> data)
    {
        if (debug) PrintSPATEMData(data);
        spatemData = data;
    }

    public void SetMAPEM(MAPEMData data)
    {   
        // Check if MAPEM data has changed
        if (mapemData != null)
        {
            if (mapemData.RefPoint.Latitude != data.RefPoint.Latitude ||
                mapemData.RefPoint.Longitude != data.RefPoint.Longitude)
            {
                Debug.Log("RefPoint changed");
                if (debug) PrintMAPEMLanes(data);
                mapemData = data;
            } 
        // If MAPEM data has not been set yet
        } else {
            Debug.Log("First time setting MAPEM lanes");
            if (debug) PrintMAPEMLanes(data);
            mapemData = data;
        }   
    }

    public void SetVAMCoordinates(double latitude, double longitude)
    {
        vamLatitude = latitude;
        vamLongitude = longitude;
    }

    private void TurnOnGreenLight()
    {
        greenLight.SetActive(true);
        redLight.SetActive(false);
    }

    private void TurnOnRedLight()
    {
        greenLight.SetActive(false);
        redLight.SetActive(true);
    }

    private void TurnOffLights()
    {
        greenLight.SetActive(false);
        redLight.SetActive(false);
    }

    private void PrintSPATEMData(Dictionary<int, int> spatemData)
    {
        Debug.Log("Printing SPATEM Data");
        foreach (KeyValuePair<int, int> trafficLight in spatemData)
        {
            Debug.Log("Traffic Light ID: " + trafficLight.Key + " State: " + trafficLight.Value);
        }
    }

    private void PrintMAPEMLanes(MAPEMData mapemData)
    {
        foreach (KeyValuePair<int, List<LaneCoordinates>> lane in mapemData.LaneCoordinatesDict)
        {
            string laneInfo = "Lane ID: " + lane.Key + "\nCoordinates: ";
            foreach (LaneCoordinates coords in lane.Value)
            {
                laneInfo += String.Format("({0}, {1}), ", coords.Latitude, coords.Longitude);
            }
            Debug.Log(laneInfo);
        }

        Debug.Log("Lane Width: " + mapemData.LaneWidth);
        Debug.Log("Reference Point: (" + mapemData.RefPoint.Latitude + ", " + mapemData.RefPoint.Longitude + ")");
    }
}
