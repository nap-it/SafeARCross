using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using TextToSpeechHandler = MixedReality.Toolkit.Examples.TextToSpeechHandler;

namespace MixedReality.Toolkit.Examples
{
    public class TextToSpeechCalibration : TextToSpeechHandler
    {
        public GameObject scriptReference;
        public GameObject currentButton;
        public GameObject nextButton;

        public override void Speak()
        {
            base.Speak();

            if (scriptReference != null)
            {
                scriptReference.GetComponent<CollisionWarningSystem>().SetHeading(0);
            }
            
            StartCoroutine(WaitAndActivate());
        }

        private IEnumerator WaitAndActivate()
        {
            // Wait for 1 second
            yield return new WaitForSeconds(1);

            if (currentButton != null) {
                currentButton.SetActive(false);  // Deactivate the current button
            } else {
                yield return new WaitForSeconds(2);  // Workaround for the first trigger (the intrusction is still playing)
            }

            if (nextButton != null)
            {
                nextButton.SetActive(true);  // Activate the next button
                nextButton.GetComponent<TextToSpeechHandler>().Speak();
            }
        }
    }
}