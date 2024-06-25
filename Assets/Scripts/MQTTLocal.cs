using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;


namespace M2MqttUnity.Examples
{
    public class MQTTLocal : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;

        private Middleware middlewareReference;
        private bool isConnected = false;
        private bool reconnecting = false;

        private List<string> eventMessages = new List<string>();

        public void SetBrokerAddress(string brokerAddress)
        {
            this.brokerAddress = brokerAddress;
        }

        public void SetBrokerPort(string brokerPort)
        {
            int.TryParse(brokerPort, out this.brokerPort);
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log("[MQTT Local] Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
            isConnected = false;
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("[MQTT Local] Connected to broker on " + brokerAddress + "\n");
            isConnected = true;
        }

        public void Publish(string topic, string message)
        {
            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        public void Reconnect()
        {
            Debug.Log("[MQTT Local] Reconnecting...");
            if (!isConnected)
            {
                Connect();
            } 
            else
            {
                Disconnect();
                reconnecting = true;
            }
        }

        protected override void SubscribeTopics()
        {
            // DENM Topics
            client.Subscribe(new string[] { "vanetza/time/denm" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // VAM Topics
            client.Subscribe(new string[] { "vanetza/time/vam" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "vam_set_heading" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // CAM Topics
            //client.Subscribe(new string[] { "vanetza/out/cam" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // MAPEM Topics
            client.Subscribe(new string[] { "vanetza/out/mapem" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // SPATEM Topics
            client.Subscribe(new string[] { "vanetza/out/spatem" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "light" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "M2MQTT_Unity/test" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.LogError("[MQTT Local] CONNECTION FAILED! " + errorMessage);
            isConnected = false;
        }

        protected override void OnDisconnected()
        {
            Debug.Log("[MQTT Local] Disconnected.");
            isConnected = false;
            if (reconnecting)
            {
                reconnecting = false;
                Connect();
            }
        }

        protected override void OnConnectionLost()
        {
            Debug.LogError("[MQTT Local] CONNECTION LOST!");
            isConnected = false;
        }

        protected override void Start()
        {
            middlewareReference = GameObject.FindObjectOfType<Middleware>();
            base.Start();
            Debug.Log("[MQTT Local] Ready.");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            try {
                JObject json = JObject.Parse(msg);
                
                double latitude = 0.0; 
                double longitude = 0.0;
                float heading = 0f;

                if (topic == "vanetza/time/denm")
                {
                    latitude = json["fields"]["denm"]["management"]["eventPosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["denm"]["management"]["eventPosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Local][DENM] Latitude: " + latitude + ", Longitude: " + longitude);
                    
                    middlewareReference.SetDENMCoordinates(latitude, longitude);
                }

                if (topic == "vanetza/time/vam")
                {
                    latitude = json["fields"]["vam"]["vamParameters"]["basicContainer"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["vam"]["vamParameters"]["basicContainer"]["referencePosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Local][VAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    middlewareReference.SetVAMCoordinates(latitude, longitude);
                }

                if (topic == "vam_set_heading")
                {
                    heading = json["heading"].Value<float>();
                    //Debug.Log("[MQTT Local][MQTT HEADING] Heading: " + heading);

                    middlewareReference.SetHeading(heading);
                }

                if (topic == "vanetza/out/mapem")
                {
                    MAPEMData mapemData = JSONParser.ParseMAPEM(msg);
                    Debug.Log("[MQTT Local][MQTT] Received MAPEM message");
                    
                    middlewareReference.SetMAPEM(mapemData);
                }

                if (topic == "vanetza/out/spatem")
                {
                    Dictionary<int, int> trafficLightsData = JSONParser.ParseSPATEM(msg);
                    //Debug.Log("[MQTT Local][MQTT] Received SPATEM message");

                    middlewareReference.SetSPATEM(trafficLightsData);
                }

                if (topic == "vanetza/out/cam")
                {
                    latitude = json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Local][CAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    middlewareReference.SetCAMCoordinates(latitude, longitude);
                }

                if (topic == "light")
                {
                    middlewareReference.SetLightState(json["state"].Value<string>());
                    //Debug.Log("[MQTT Remote][MQTT] Received Light message " + json["state"].Value<string>());
                }
            } catch (Exception e) {
                Debug.Log("[MQTT Local] Failed to parse message on topic: " + topic + " msg: " + msg + ", Exception: " + e.Message);
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            //AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}
