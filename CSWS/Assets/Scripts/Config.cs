using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    /*ENGINE BEHAVIOUR BOOLS*/
    public bool DragFroceActive; //Bool for drag Force Func
    public bool EngineStalling; //Bool for wether the engine can stall currently
    public bool ResetRPSOnGearChange; //Bool for wether RPS values are reset to minimum upon gear change

    /*INPUT CONTROL BOOLS*/
    public bool CursorLockEnable; //Bool for wether cursor can be locked
    public bool MouseSteering; //Bool for enabling mouse input to affect player rot
    public bool KeyboardSteering; //Bool for enabling Keyboard input to affect player rot
    public bool ClutchNeeded; //Bool for wether clutch is required for changing gears
    public bool ConstThrottle; //Bool for weither Throttle is always set to a constant value
    public bool DisplaySpeedInMPH; //Stupid measurement

    /*DEBUG BOOLS*/
    public bool KeyLogging; //controls key press messages that get sent to console
    public bool StatusUpdates; //controls readout messages that get sent to console

    /*Additional Variables*/
    /// these variables are developer variables that would rarely be changed but maybe. ///
    public float MaxRPS;
    public float MinRPS;
    public float EngineStallRPS;
    public float MaxAccelTime;
    public float engSpinUpTime;
    public float MinVPadding;
    public float MaxDeltaP;

    public float dragCoefficient = 0.5f;
    public float densOfAir = 1.293f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
