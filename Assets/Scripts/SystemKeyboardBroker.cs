﻿// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;


namespace M2MqttUnity.Examples
{
    /// <summary>
    /// An example script that delegates keyboard API access either to the WinRT <c>MixedRealityKeyboard</c> class
    /// or Unity's <see cref="TouchScreenKeyboard"/> class depending on the platform.
    /// </summary>
    /// <remarks>
    /// <para>Note that like Unity's TouchScreenKeyboard API, this script only supports WSA, iOS, and Android.</para>
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/System Keyboard Broker")]
    public class SystemKeyboardBroker : MonoBehaviour
    {
#if WINDOWS_UWP
        private WindowsMRKeyboard wmrKeyboard;
#elif UNITY_IOS || UNITY_ANDROID
        private TouchScreenKeyboard touchscreenKeyboard;
#endif

        [SerializeField]
        private TextMeshPro debugMessage = null;
        public TextMeshProUGUI brokerIP;
        public GameObject scriptReference;
        public string brokerType;
        private MQTTLocal localMqttClient;
        private MQTTRemote remoteMqttClient;


#pragma warning disable 0414
        [SerializeField]
        private KeyboardPreview mixedRealityKeyboardPreview = null;
#pragma warning restore 0414


        /// <summary>
        /// Opens a platform specific keyboard.
        /// </summary>
        public void OpenSystemKeyboard()
        {
#if WINDOWS_UWP
            wmrKeyboard.ShowKeyboard(wmrKeyboard.Text, false);
#elif UNITY_IOS || UNITY_ANDROID
            touchscreenKeyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false);
#endif
        }

        #region MonoBehaviour Implementation

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            // Initially hide the preview.
            if (mixedRealityKeyboardPreview != null)
            {
                mixedRealityKeyboardPreview.gameObject.SetActive(false);
            }
            
            // Script references is an empty game object that has the MQTTLowLevel script attached to it
            if (brokerType == "local")
            {
                localMqttClient = scriptReference.GetComponent<MQTTLocal>();
                // Set the brokerIP text to the brokerAddress and brokerPort
                brokerIP.text = localMqttClient.brokerAddress + ":" + localMqttClient.brokerPort.ToString();
            }
            else if (brokerType == "remote")
            {
                remoteMqttClient = scriptReference.GetComponent<MQTTRemote>();
                // Set the brokerIP text to the brokerAddress and brokerPort
                brokerIP.text = remoteMqttClient.brokerAddress + ":" + remoteMqttClient.brokerPort.ToString();
            }
            brokerIP.text = localMqttClient.brokerAddress + ":" + localMqttClient.brokerPort.ToString();

#if WINDOWS_UWP
            // Windows mixed reality keyboard initialization goes here
            wmrKeyboard = gameObject.AddComponent<WindowsMRKeyboard>();
            if (wmrKeyboard.OnShowKeyboard != null)
            {
                wmrKeyboard.OnShowKeyboard.AddListener(() =>
                {
                    if (mixedRealityKeyboardPreview != null)
                    {
                        mixedRealityKeyboardPreview.gameObject.SetActive(true);
                    }
                });
            }

            if (wmrKeyboard.OnHideKeyboard != null)
            {
                wmrKeyboard.OnHideKeyboard.AddListener(() =>
                {
                    if (mixedRealityKeyboardPreview != null)
                    {
                        mixedRealityKeyboardPreview.gameObject.SetActive(false);
                    }
                });
            }
#elif UNITY_IOS || UNITY_ANDROID
            // non-Windows mixed reality keyboard initialization goes here
#else       
            Debug.Log("Keyboard not supported on this platform.");
            if (debugMessage != null) debugMessage.text = "Keyboard not supported on this platform.";
#endif
            
        }

        
#if WINDOWS_UWP
        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            // Windows mixed reality keyboard update goes here
            if (wmrKeyboard.Visible)
            {
                if (debugMessage != null)
                {
                    debugMessage.text = "Typing: " + wmrKeyboard.Text;
                }

                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.Text = wmrKeyboard.Text;
                    mixedRealityKeyboardPreview.CaretIndex = wmrKeyboard.CaretIndex;

                    if (brokerIP != null)
                    {
                        brokerIP.text = wmrKeyboard.Text;
                        // if text has ":" then split it and assign to brokerAddress and brokerPort
                        string[] brokerAddressPort = wmrKeyboard.Text.Split(':');

                        if (brokerType == "local")
                        {
                            localMqttClient.brokerAddress = brokerAddressPort[0];
                            localMqttClient.brokerPort = int.Parse(brokerAddressPort[1]);
                        }
                        else if (brokerType == "remote")
                        {
                            remoteMqttClient.brokerAddress = brokerAddressPort[0];
                            remoteMqttClient.brokerPort = int.Parse(brokerAddressPort[1]);
                        }
                    }
                }
            }
            else
            {
                var keyboardText = wmrKeyboard.Text;

                if (string.IsNullOrEmpty(keyboardText))
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "Open keyboard to type text.";
                    }
                }
                else
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "Typed: " + keyboardText;
                    }
                }
                
                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.Text = string.Empty;
                    mixedRealityKeyboardPreview.CaretIndex = 0;
                }
            }
        }
#elif UNITY_IOS || UNITY_ANDROID
        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            // non-Windows mixed reality keyboard initialization goes here
            // for non-Windows mixed reality keyboards just use Unity's default
            // touch screen keyboard.
            if (touchscreenKeyboard != null)
            {
                string KeyboardText = touchscreenKeyboard.text;
                if (TouchScreenKeyboard.visible)
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "typing... " + KeyboardText;
                    }
                }
                else
                {
                    if (debugMessage != null)
                    {
                        debugMessage.text = "typed " + KeyboardText;
                    }

                    touchscreenKeyboard = null;
                }
            }
        }
#endif

        #endregion MonoBehaviour Implementation
    }
}
#pragma warning restore CS1591