using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupRoutine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject calibrateDialog = GameObject.Find("CalibrateOptionsDialog");
        calibrateDialog.SetActive(false);
        GameObject manualCalibrateDialog = GameObject.Find("ManualCalibrateDialog");
        manualCalibrateDialog.SetActive(false);
        GameObject settingsCanvas = GameObject.Find("SettingsCanvas");
        settingsCanvas.SetActive(false);
        GameObject handMenu = GameObject.Find("HandMenu/MenuContent-Canvas");
        handMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
