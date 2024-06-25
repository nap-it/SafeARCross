using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Middleware : MonoBehaviour
{
    private CollisionWarningSystem cwsReference;
    private VirtualTrafficLights vtlReference;
    private double denmLatitude = 0.0;
    private double denmLongitude = 0.0;
    private double vamLatitude = 0.0;
    private double vamLongitude = 0.0;
    private float mqttHeading = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cwsReference = GameObject.FindObjectOfType<CollisionWarningSystem>();
        vtlReference = GameObject.FindObjectOfType<VirtualTrafficLights>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDENMCoordinates(double latitude, double longitude)
    {
        denmLatitude = latitude;
        denmLongitude = longitude;

        // Set values for AR systems
        cwsReference.SetDENMCoordinates(latitude, longitude);
        cwsReference.DisplayWarningSystem();
    }

    public void SetVAMCoordinates(double latitude, double longitude)
    {
        vamLatitude = latitude;
        vamLongitude = longitude;

        // Set values for AR systems
        cwsReference.SetVAMCoordinates(latitude, longitude);
        vtlReference.SetVAMCoordinates(latitude, longitude);
    }

    public void SetCAMCoordinates(double latitude, double longitude)
    {
        // Set values for AR systems
        cwsReference.SetCAMCoordinates(latitude, longitude);
    }

    public void SetHeading(float heading)
    {
        mqttHeading = heading;
        cwsReference.SetHeading(heading);
    }

    public void SetLight(string value)
    {
        vtlReference.SetLight(value);
    }

    public void SetMAPEM(MAPEMData mapemData)
    {
        // Set values for AR systems
        vtlReference.SetMAPEM(mapemData);
    }

    public void SetSPATEM(Dictionary<int, int> trafficLightsData)
    {
        // Set values for AR systems
        vtlReference.SetSPATEM(trafficLightsData);
    }

    public void SetLightState(string value)
    {
        // Set values for AR systems
        vtlReference.SetLight(value);
    }
}
