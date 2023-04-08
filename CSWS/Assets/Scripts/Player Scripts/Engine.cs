using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering;

public class Engine : MonoBehaviour
{
    /* *** Primary VARS *** */
    public float engineRPS;
    public int CurrentGear;
    public float turnSpeed;
    public float forceScalar;
    public float velocityInFwdDir;
    public bool RPSIncreasing;

    /* *** Check Vars *** */
    public float EngineStallRPS;
    public float stallTimer; // this is the time after which a stalled engine can be restarted

    public float[] RPMIncrease;
    public float[] StartForce;
    public float[] dropRate;
    public float[] minSpeeds;

    /* *** Accumulator VARS *** */
    float accelTime; // this is the amount of time the throttle has been held down for

    /* *** Debug VARS *** */
    public bool Keylogging;

    /* *** State VARS *** */
    public bool EngineOn;
    bool InGear;
    bool Throttle;
    public bool Stalled;
    bool Slipping;
    bool Steering;
    float outForce; //this is the force output with each turn of the engine
    float CurrentTimer;
    float playerRotation; //the rotation change of the player for the current frame
    int NewGear; //used to hold the value of the next gear to be used before clutch is released.
    public bool engineStarting; //this is used to indicate state during which engine is starting but not active
    public float StartTimer; //this is used during the ignition phase to control how long it takes for the engine to start 
    Rigidbody rgd;
    public Config config;

    /* *** Rate Of Change VARS *** */
    // this will be the rate at which the engine rpm drops when above 1000 RPM for each gear

    // these variables will be used to organise the rpm drop caused by dropping car into gear




    
    void Start()
    {
        rgd = GetComponent<Rigidbody>();
        StopEngine();
        InGear = false;
    }
    void Update()
    {
        PrimaryKeyPresses(/*INPUT FUNCTIONS (Primary)*/); 
        if (EngineOn == true)
        {
            EngineKeyPresses(/*INPUT FUNCTIONS (Primary)*/);
            EngineActions(/*OUTPUT FUNCTIONS (Primary*/);
        }
        if (Stalled == true)
        {
            StallTimer(/*CALCULATION FUNCTIONS (Changes)*/);
        }
        WorldActions(/*OUTPUT FUNCTIONS (Primary*/);
    }


    /// /// Input Functions (Primary) /// ///
    /// /// /// /// /// //// /// /// /// ///
    /// below are highest level functions for capturing player input
    #region WIP
    void PrimaryKeyPresses(/*Primary Key presses are checked for regardless of engine status*/)
    {
        if (EngineOn == false && Stalled == false) { CheckIgnition(/*INPUT FUNCTIONS SECONDARY*/); }
        RotatePlayer(/*OUTPUT FUNCTIONS (REAL)*/);
    }/*checks for ignition Inputs if engine meets start criteria*//* updates player rotation*/

    void EngineKeyPresses(/*Engine Key Presses are only checked when engine is on*/)
    {
        Clutch(); /*INPUT FUNCTIONS SECONDARY*/
        ThrottleOn(); /*INPUT FUNCTIONS SECONDARY*/

        UpdateVelFwdDir(); /*CALCULATION FUNCTIONS*/



    }/*Checks for CLUTCH and THROTTLE INPUTS*//*Updates velocityFwdDir*/
    #endregion
    #region Finialised

    #endregion


    /// /// Input Functions (Secondary) /// ///
    /// /// /// /// /// //// /// /// /// /// ///
    /// below are secondary functions checking for specific inputs
    #region WIP

    void CheckIgnition()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            engineStarting = false;
            StartEngine(/*OUTPUT FUNCTIONS (Engine)*/);

            KeyPressed("I", 0);/*DEBUG FUNCTIONS*/
            StatusUpdate("EngineStarting");/*DEBUG FUNCTIONS*/
        }
        if (Input.GetKey(KeyCode.I))
        {
            StartEngine(); 
            KeyPressed("I", 1);
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            engineStarting = false;
        } 
    }/*Checks if the ignition key has been pressed and starts engine*/
    void MouseSteering()/*Config Var >> */{ if (config.MouseSteering){

        playerRotation = Input.GetAxis("Horizontal");
        //print(CalcRequiredVelChange());
        if (Input.GetKeyDown(KeyCode.L)) //if the player presses L change the cursor lock state.
        {
            KeyPressed("L", 0);/*DEBUG FUNCTIONS*/
            ChangeCursorLock(/*OUTPUT FUNCTIONS (REAL)*/);
        }
    }} /*Checks for MOUSE INPUT and Checks if the LOCKSTATE/STEERING Key has been pressed*/
    void KeyboardSteering()/*Config Var >> */{if (config.KeyboardSteering){

            if (Input.GetKey(KeyCode.LeftArrow)) { playerRotation = -1.0f; KeyPressed("LArrow", 1); }//TurnLeft
            if (Input.GetKey(KeyCode.RightArrow)) { playerRotation = 1.0f; KeyPressed("RArrow", 1); }//TurnRight
            if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) { playerRotation = 0.0f; KeyPressed("LArrow+RArrow", 1); }//SET 0
            if (Input.GetKeyDown(KeyCode.L)) //if the player presses L change the steering Value.
            {
                KeyPressed("L", 0);
                SetSteering(/*OUTPUT FUNCTIONS (Engine)*/);
            }
    }}/*Checks for KEYBOARD INPUT and Checks if the STEERING Key has been pressed*/
    void ThrottleOn()/*Config Var >> */{if(config.ConstThrottle == false)
    {
        if (Input.GetKey(KeyCode.W) && InGear == true)
        {
            KeyPressed("W", 1); /*DEBUG FUNCTIONS*/
            Throttle = true;
            AccelTimer(Time.deltaTime); /*CALCULATION FUNCTIONS (Changes)*/
        }
        else 
        {
            Throttle = false;
            AccelTimer(/*CALCULATION FUNCTIONS (Changes)*/);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeyPressed("W", 2); /*DEBUG FUNCTIONS*/
            StatusUpdate("Throttle off"); /*DEBUG FUNCTIONS*/
            Throttle = false;
            AccelTimer(/*CALCULATION FUNCTIONS (Changes)*/);
        }
    }else{/*alternative behaviour if config var is true*/Throttle = true;
    }}/*Checks if the Throttle Key is been pressed*/
    void Clutch()/*Config Var >> */{if(config.ClutchNeeded){

        if (Input.GetKeyDown(KeyCode.LeftShift)) //clutch down
        {
            KeyPressed("L_Shift", 1); /*DEBUG FUNCTIONS*/
            Throttle = false;
            NewGear = CurrentGear;
            AccelTimer();
            InGear = false;
        }
        if(InGear == false && Input.GetKey(KeyCode.E)) //currentGear increase
        {
            KeyPressed("L_Shift+E", 0); /*DEBUG FUNCTIONS*/
            NewGear = CurrentGear + 1;
            if(NewGear >= 5) { NewGear = 5; }
        }
        if (InGear == false && Input.GetKey(KeyCode.Q)) //currentGear decrease
        {
            KeyPressed("L_Shift+Q", 0); /*DEBUG FUNCTIONS*/
            NewGear = CurrentGear - 1;
            if (NewGear <= 1) { NewGear = 1; }
        }
        if (InGear == false && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E)) //Put car in neutral
        {
            NewGear = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) //clutch up
        {
            KeyPressed("L_Shift", 3); /*DEBUG FUNCTIONS*/
            if (NewGear == CurrentGear) { InGear = true; }
            else { MeshGears(NewGear); } /*OUTPUT FUNCTIONS (Engine)*/
        }
    }else{/*alternative behaviour if config var is false*/
        if(Input.GetKeyDown(KeyCode.E)){CurrentGear += 1; if (CurrentGear >= 5) {CurrentGear = 5;} }
        if(Input.GetKeyDown(KeyCode.Q)){CurrentGear -= 1; if (CurrentGear <= 1) {CurrentGear = 1;} }
        MeshGears(CurrentGear); /*OUTPUT FUNCTIONS (Engine)*/
    }}/*Checks Clutch Key and gear change keys to take engine in and out of gear and change gear*/
    #endregion
    #region Finialised

    #endregion

    
    /// /// Output Functions (Primary) /// ///
    /// /// /// /// /// //// /// /// /// ///
    /// below are highest level functions for calling Action Functions
    #region WIP
    
    void EngineActions()
    {
        RPSUpdate(/*OUTPUT FUNCTIONS (Engine)*/);//Updates our engines RPS
        EngineForce(/*OUTPUT FUNCTIONS (REAL)*/); //Adds the engines Force to our rigidbody 
    }/*calls actions that are performed only while engine is on*/
    void WorldActions()
    {
        AddDragForce(/*OUTPUT FUNCTIONS (Real)*/);
        CalcRequiredVelChange( );
    }
    #endregion
    #region Finialised

    #endregion


    /// /// Output Functions (REAL) /// ///
    /// /// /// /// /// //// /// /// /// ///
    /// below are action base functions that output a "REAL" effect into the game world

    #region WIP

    void EngineForce()
    {
        float EngForce = CalcEngOutForce(/*CALCULATION FUNCTIONS*/); 
        rgd.AddForce(rgd.transform.forward * EngForce);
        if (velocityInFwdDir > MinV() && CurrentGear != 0) { rgd.AddForce(rgd.transform.forward * CalcLerpForce((velocityInFwdDir-MinV()) / (MaxV() - MinV()))); }
    }/*calculates and adds current engine output force to rigidbody*/
    void RotatePlayer()
    {
        KeyboardSteering(/*INPUT FUNCTIONS SECONDARY*/); 
        MouseSteering(/*INPUT FUNCTIONS SECONDARY*/);
        if (Steering) { rgd.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * playerRotation, 0)); }
    }/*checks for rotation player INPUTS then updates players rotation if STEERING = true*/
    void ChangeCursorLock()/*Config Var >> */{if (config.CursorLockEnable){
        
        if (Cursor.lockState == CursorLockMode.None) { Cursor.lockState = CursorLockMode.Locked; SetSteering(true);}/*OUTPUT FUNCTIONS (Engine)*/
        else if (Cursor.lockState == CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.None; SetSteering(false);}/*OUTPUT FUNCTIONS (Engine)*/
    }else{/*alternative behaviour if config var is false*/SetSteering(); KeyPressed("L", 0);
    }}/*Changes the Cursor Lock State and Steering value*/
    void AddDragForce()/*Config Var >> */{if(config.DragFroceActive){

        float dragForceMag = CalcDragForce(/*CALCULATION FUNCTIONS*/); 
        rgd.AddForce(-1.0f * rgd.velocity.normalized * dragForceMag);
    }}/*calculates and then adds drag force to our rigidbody*/
    #endregion
    #region Finialised

    #endregion

    /// /// Output Functions (Engine) /// ///
    /// /// /// /// /// //// /// /// /// ///
    /// below are action based functions that output a "SIM" effect to the "ENGINE" variables

    #region WIP
    void RPSUpdate()
    {
        if (Throttle == true) { engineRPS += RpmIncrease() * Time.deltaTime; }
        else { engineRPS -= dropRate[CurrentGear] * Time.deltaTime; RpmIncrease(); RPSIncreasing = false; }
        if (engineRPS >= config.MaxRPS) { engineRPS = config.MaxRPS; }
        if (CurrentGear == 0 && engineRPS <= config.MinRPS) { engineRPS = config.MinRPS; }
        if (engineRPS <= config.EngineStallRPS) { StallEngine(); }
    }
    void StartEngine()
    {

        if (engineStarting == true)
        {
            if (StartTimer >= 0)
            {
                StartTimer -= Time.deltaTime;
                StatusUpdate("EngineStarting Progress: " + StartTimer);/*DEBUG FUNCTIONS*/
            }
            else
            {
                CurrentGear = 0;
                MeshGears(CurrentGear);
                EngineOn = true;
                engineStarting = false;

                StatusUpdate("Engine_Started"); /*DEBUG FUNCTIONS*/
            }
        }
        else
        {
            StartTimer = config.engSpinUpTime; 
            engineStarting = true;
        }
    }/*sets engine values to natural start values*/
    void StopEngine()
    {
        CurrentGear = 0;
        engineRPS = 0;
        EngineOn = false;
    }/*turns of the engine without stalling*//*WIP*/
    void SetSteering()
    {
        Steering = !Steering;
        StatusUpdate("Steering = " + Steering); /*Debug functions*/
    }/*toggles STEERING value equal to !STEERING*/
        /// SubFunctions ///
        /// these functions use preset values on the primary function to produce a result
        void SetSteering(bool Val)
        {
            Steering = Val;
            StatusUpdate("Steering = " + Steering); /*Debug functions*/
        }/*sets STEERING to the value VAL it is given*/
    void MeshGears(int gear)/*Config Var >> */{if (config.ResetRPSOnGearChange){
            if (gear == 0) { CurrentGear = gear; engineRPS = config.MinRPS; }
            if (gear == 1) { CurrentGear = gear; engineRPS = config.MinRPS; }
            if (gear == 2) { CurrentGear = gear; engineRPS = config.MinRPS; }
            if (gear == 3) { CurrentGear = gear; engineRPS = config.MinRPS; }
            if (gear == 4) { CurrentGear = gear; engineRPS = config.MinRPS; }
            if (gear == 5) { CurrentGear = gear; engineRPS = config.MinRPS; }
            InGear = true;
        }else{/*alternative behaviour if config var is false*/CurrentGear = gear; InGear = true;
            }
        }/*resets rps to minimum and sets current gear to value GEAR it is given*/
    void StallEngine()/*Config Var >> */{if (config.EngineStalling){
            CurrentGear = 0;
            engineRPS = 0;
            EngineOn = false;
            Stalled = true;
            CurrentTimer = stallTimer;
    }else{/*alternative behaviour if config var is false*/if (engineRPS <= 0.0f) {engineRPS = 0.0f;} 
    }}/*turns engine off*//*sets stall value to true*//*sets current timer to stall timer*/
    #endregion
    #region Finialised

    #endregion

    /// /// Calculation Functions (Velocity)/// ///
    /// /// /// /// /// //// /// /// ///
    /// below are action based functions that output a "REAL" effect into the game world
    #region WIP
    float VelDifference(/*result summary inside*/)
    {
        float VelDif = velocityInFwdDir - ExpectedV();/*CALCULATION FUNCTIONS*/
        if(Mathf.RoundToInt(VelDif) == 0) /*returns zero value for perfect speed*/{ return 0.0f; }
        if(VelDif >= 0.1f) /*returns Positive One for faster than expected*/{return VelDif / (MaxV()-ExpectedV()); }/*CALCULATION FUNCTIONS*/
        else/*returns negative One for slower than expected*/{return VelDif/(MinV()-ExpectedV()); }/*CALCULATION FUNCTIONS*/

        ///Result summary
        ///(Result > 1): current velocity is HIGHER than MaxV() for this gear
        ///(1 > Result > 0): current velocity is HIGHER than ExpectedV() but NOT HIGHER than MaxV()
        ///(Result = 0): current velocity is EQUAL to ExpectedV()
        ///(0 > Result > -1): current velocity is LOWER than ExpectedV() but not LOWER than MinV()
        ///(-1 > Result): current velocity is LOWER than MinV() for this gear
    }/*returns the current difference between the expected velocity and the current velocity as percent negative or positive*/
    float ExpectedV(float RPS, int Gear)// returns the expected velocity in Fwd Dir based on given Values RPS and GEAR
    {
        if(Gear == 0) { return 0; }
        return Mathf.Sqrt((RPS * StartForce[Gear]) / (0.5f*config.densOfAir*config.dragCoefficient*0.25f*Mathf.PI));
    }
        /*SubFunctions*/
        /// <summary>
        /// these functions use preset values on the primary function to produce a result
        /// </summary>
        float ExpectedV(/*OVERLOAD fUNCTION*/)
        {
            return Mathf.Sqrt((engineRPS * StartForce[CurrentGear]) / (0.25f));
        }/*returns the expected velocity in Fwd Dir using the current engine Values*/
        public float MaxV()
        {
            return ExpectedV(config.MaxRPS, CurrentGear); /*Parent Func*/ 
        }/*returns the max velocity of the current gear.*/
        public float MinV() 
        {
            return ExpectedV(config.MinRPS, CurrentGear); /*Parent Func*/
        }/*returns the min velocity of the current gear.*/
    float UpdateVelFwdDir()
    {
        velocityInFwdDir = Vector3.Dot(rgd.transform.forward,rgd.velocity);
        StatusUpdate("velFwd: " + velocityInFwdDir); /*DEBUG FUNCTIONS*/
        return velocityInFwdDir;
    }/*calculates current velocityInFwdDir updates our public Float Value and return it as a float value when called.*/
    #endregion
    #region Finialised

    #endregion

    /// /// Calculation Functions (Forces) /// ///
    /// /// /// /// /// //// /// /// ///
    /// below are action based functions that output a "REAL" effect into the game world
    #region WIP
    float CalcEngOutForce()
    {
        return engineRPS * StartForce[CurrentGear] * forceScalar;
    }/*returns the current magnitude of output force generated by the engine*/
    float CalcDragForce()
    {
        return 0.5f * config.densOfAir * rgd.velocity.magnitude * rgd.velocity.magnitude * config.dragCoefficient * Mathf.PI * 0.25f;
    }/*returns the current magnitude of drag force generated from velocity values && config values(dragCoefficient & densOfAir)*/
    float CalcRequiredVelChange()
    {
        float val = Vector3.Dot(rgd.velocity, rgd.transform.right);
        if (val <= config.MaxDeltaP) {rgd.AddForce(val * -rgd.transform.right, ForceMode.Impulse);}
        return val;
    }/*returns the required change in velocity required for a  perfect turn*/
    float CalcLerpForce(float VDif)
    {
        float LerpRaw = (2 * Mathf.Pow(VDif, 3) - 3 * Mathf.Pow(VDif, 2) + 1) * accelTime;
        //print("Vdif: " + VDif + "Lerp Val: " + LerpRaw);
        return LerpRaw;
    }
    #endregion
    #region Finialised

    #endregion


    /// /// Calculation Functions (Changes) /// ///
    /// /// /// /// /// //// /// /// ///
    #region WIP
    float RpmIncrease()
    {
        float increase;
        //print(MinV() - config.MinVPadding);
        if (velocityInFwdDir >= (MinV()-config.MinVPadding)) { increase = RPMIncrease[CurrentGear] * IncreasePercent((engineRPS-config.MinRPS)/(config.MaxRPS - config.MinRPS)); RPSIncreasing = true; }
        else { increase = 0; }

        return increase;
    }
    float IncreasePercent(float Val)
    {
        float result = 1 - (MathF.Pow(2 * (Val - 0.5f), 2));
        if(result < 0.1f) result = 0.1f;
        return result;
    }
    void StallTimer()
    {
        CurrentTimer -= Time.deltaTime; if (CurrentTimer <= 0) { CurrentTimer = 0; Stalled = false; }
    }/*Reduces current timer over time and sets stalled to false once time is up*/
    void AccelTimer(float delta)
    {
        accelTime += delta;
        if(accelTime >= config.MaxAccelTime) { accelTime = config.MaxAccelTime; }
    }/*increases accel time by given Value DELTA*//*Clamps Value to be less than config value(MaxAccelTime)*/
        /// SubFunctions ///
        void AccelTimer(/*OVERLOAD FUNCTION*/)
        {
            accelTime = 0;
        }/*resets accel time to 0*/
    #endregion
    #region Finialised

    #endregion

    #region DUBUG Functions
    /// /// DUBUG Functions /// ///
    /// /// /// /// ///
    /// below are action based functions that control messages sent to the console
    void StatusUpdate(string UpdateMessage)/*Config Var >> */{if(config.StatusUpdates)
    {
        print(UpdateMessage);
    }}/*prints engine status messages*/
    void KeyPressed(String button, int Type)/*Config Var >> */{if(config.KeyLogging)
    {

        string action = "";
        if (Type == 0) { action = "Pressed"; }
        if (Type == 1) { action = "Held"; }
        if (Type == 2) { action = "Released"; }
        
        print("Button_" + button + "_" + action);
    }}/*prints key press messages*/
    #endregion

}
