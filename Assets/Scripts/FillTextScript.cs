using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Slider = MixedReality.Toolkit.UX.Slider;
using TMPro;


public class FillTextScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    private GameObject scriptReference;
    // Start is called before the first frame update
    void Start()
    {
        scriptReference = GameObject.Find("Scripts");
        slider.Value = (int)scriptReference.GetComponent<CollisionWarningSystem>().GetHeading();
    }

    // Update is called once per frame
    void Update()
    {
        // Set the text to the value of the slider with .1f precision
        text.text = slider.Value.ToString("F1") + "º";
        if (scriptReference != null)
        {
            scriptReference.GetComponent<CollisionWarningSystem>().SetHeading(slider.Value);
        }
    }

    public void SetSliderValue()
    {
        scriptReference = GameObject.Find("Scripts");
        slider.Value = (int)scriptReference.GetComponent<CollisionWarningSystem>().GetHeading();
        Debug.Log("Slider Value: " + (int)scriptReference.GetComponent<CollisionWarningSystem>().GetHeading());
        scriptReference.GetComponent<CollisionWarningSystem>().SetHeading(slider.Value);
    }
}
