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
    public class MQTTRemote : M2MqttUnityClient
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
            Debug.Log("[MQTT Remote] Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
            isConnected = false;
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("[MQTT Remote] Connected to broker on " + brokerAddress + "\n");
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
            Debug.Log("[MQTT Remote] Reconnecting...");
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
            client.Subscribe(new string[] { "denm_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // VAM Topics
            client.Subscribe(new string[] { "vam_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "vam_set_heading" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // CAM Topics
            client.Subscribe(new string[] { "cam_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "obu50/vanetza/own/cam_full" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "p1/cam_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // MAPEM Topics
            client.Subscribe(new string[] { "mapem_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            // SPATEM Topics
            client.Subscribe(new string[] { "spatem_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "light" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "M2MQTT_Unity/test" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.LogError("[MQTT Remote] CONNECTION FAILED! " + errorMessage);
            isConnected = false;
        }

        protected override void OnDisconnected()
        {
            Debug.Log("[MQTT Remote] Disconnected.");
            isConnected = false;
            if (reconnecting)
            {
                reconnecting = false;
                Connect();
            }
        }

        protected override void OnConnectionLost()
        {
            Debug.LogError("[MQTT Remote] CONNECTION LOST!");
            isConnected = false;
        }

        protected override void Start()
        {
            middlewareReference = GameObject.FindObjectOfType<Middleware>();
            base.Start();
            Debug.Log("[MQTT Remote] Ready.");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            try {
                JObject json = JObject.Parse(msg);
                
                double latitude = 0.0; 
                double longitude = 0.0;
                float heading = 0f;

                if (topic == "denm_decoded")
                {
                    latitude = json["fields"]["denm"]["management"]["eventPosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["denm"]["management"]["eventPosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Remote][DENM] Latitude: " + latitude + ", Longitude: " + longitude);
                    
                    middlewareReference.SetDENMCoordinates(latitude, longitude);
                }

                if (topic == "vam_decoded")
                {
                    latitude = json["vam"]["vamParameters"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["vam"]["vamParameters"]["referencePosition"]["longitude"].Value<double>();

                    latitude = latitude / 10000000;
                    longitude = longitude / 10000000;
                    //Debug.Log("[MQTT Remote][VAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    middlewareReference.SetVAMCoordinates(latitude, longitude);
                }

                if (topic == "vam_set_heading")
                {
                    heading = json["heading"].Value<float>();
                    //Debug.Log("[MQTT Remote][MQTT HEADING] Heading: " + heading);

                    middlewareReference.SetHeading(heading);
                }
                
                if (topic == "obu50/vanetza/own/cam_full")
                {
                    latitude = json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Remote][CAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    middlewareReference.SetCAMCoordinates(latitude, longitude);
                }

                if (topic == "p1/cam_decoded")
                {
                    latitude = json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["cam"]["camParameters"]["basicContainer"]["referencePosition"]["longitude"].Value<double>();
                    //Debug.Log("[MQTT Remote][CAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    middlewareReference.SetCAMCoordinates(latitude, longitude);
                }

                 if (topic == "mapem_decoded")
                {
                    MAPEMData mapemData = JSONParser.ParseMAPEM(msg);
                    //Debug.Log("[MQTT Remote][MQTT] Received MAPEM message");
                    
                    middlewareReference.SetMAPEM(mapemData);
                }

                if (topic == "spatem_decoded")
                {
                    Dictionary<int, int> trafficLightsData = JSONParser.ParseSPATEM(msg);
                    //Debug.Log("[MQTT Remote][MQTT] Received SPATEM message");

                    middlewareReference.SetSPATEM(trafficLightsData);
                }

                if (topic == "light")
                {
                    middlewareReference.SetLightState(json["state"].Value<string>());
                    //Debug.Log("[MQTT Remote][MQTT] Received Light message " + json["state"].Value<string>());
                }
            } catch (Exception e) {
                Debug.Log("[MQTT Remote] Failed to parse message on topic: " + topic + " msg: " + msg + ", Exception: " + e.Message);
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
