using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using TextToSpeechHandler = MixedReality.Toolkit.Examples.TextToSpeechHandler;
using PressableButtonHoloLens2 = MixedReality.Toolkit.UX.PressableButton;


namespace MixedReality.Toolkit.Examples
{
    public class TextToSpeechDemo : TextToSpeechHandler
    {
        public GameObject HandMenuCanvas;
        public GameObject Countdown;
        public GameObject scriptReference;

        private bool isSpeaking = false;

        public override void Speak()
        {
            StartCoroutine(SpeakSequence());
        }

        private IEnumerator SpeakSequence()
        {
            base.message = "Welcome to the Safe AR Cross Application Demo. In this demo, we will showcase the potential interfaces available to you.";
            SpeakAndWait(base.message);
            yield return new WaitUntil(() => !isSpeaking);

            if (HandMenuCanvas != null)
            {
                HandMenuCanvas.SetActive(true);
            }
            base.message = "This is the main menu. You can enable it by extending your left hand in front of you, and looking at it.";
            SpeakAndWait(base.message);
            yield return new WaitUntil(() => !isSpeaking);

            base.message = "Here you can toggle the activation of our 2 systems. The Collision Warning System, and the Virtual Traffic Lights. You can also access the settings menu for further customization.";
            SpeakAndWait(base.message);
            yield return new WaitUntil(() => !isSpeaking);

            if (HandMenuCanvas != null)
            {
                HandMenuCanvas.SetActive(false);
            }
            base.message = "The Collision Warning System is designed to alert you when you are about to collide with a vehicle. Ready to see it in action?";
            SpeakAndWait(base.message);
            yield return new WaitUntil(() => !isSpeaking);
            
            if (Countdown != null)
            {
                Countdown.SetActive(true);
                Countdown.GetComponent<TextToSpeechHandler>().Speak();
                yield return new WaitForSeconds(3);

                if (scriptReference != null)
                {
                    scriptReference.GetComponent<CollisionWarningSystem>().DemoSystem();
                }
            }

            yield return new WaitForSeconds(8f);
            base.message = "Cool right? Now let's move on to the Virtual Traffic Lights system. The Virtual Traffic Lights system is designed to help you navigate through intersections. Ready to see it in action?";
            SpeakAndWait(base.message);
            yield return new WaitUntil(() => !isSpeaking);
            if (Countdown != null)
            {
                Countdown.SetActive(true);
                Countdown.GetComponent<TextToSpeechHandler>().Speak();
                yield return new WaitForSeconds(3);

                if (scriptReference != null)
                {
                    scriptReference.GetComponent<VirtualTrafficLights>().DemoSystem();
                }
            }
        }

        private void SpeakAndWait(string message)
        {
            isSpeaking = true;
            base.Speak();
            StartCoroutine(CheckSpeechLength(message));
        }

        private IEnumerator CheckSpeechLength(string message)
        {
            // Estimate the duration of the speech here
            float estimatedDuration = EstimateSpeechDuration(message);
            yield return new WaitForSeconds(estimatedDuration);
            isSpeaking = false;
        }

        private float EstimateSpeechDuration(string speech)
        {
            // Simple estimation: average reading speed or you could base it on the number of words
            float wordsPerMinute = 140;  // Average spoken words per minute
            int wordCount = speech.Split(' ').Length;
            return (float)wordCount / wordsPerMinute * 60;
        }
    }
}