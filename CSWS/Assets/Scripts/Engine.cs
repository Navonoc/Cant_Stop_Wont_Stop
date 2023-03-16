using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    /* *** Check Vars *** */
    public float EngineStallRPS;
    public float stallTimer; // this is the time after which a stalled engine can be restarted

    public float[] RPMIncrease;
    public float[] StartForce;
    public float[] dropRate;

    /* *** Accumulator VARS *** */
    float accelTime; // this is the amount of time the throttle has been held down for

    /* *** Debug VARS *** */
    public bool Keylogging;

    /* *** State VARS *** */
    bool EngineOn;
    bool InGear;
    bool Throttle;
    bool Stalled;
    bool Steering;
    float outForce; //this is the force output with each turn of the engine
    float CurrentTimer;
    
    Rigidbody rgd;

    /* *** Rate Of Change VARS *** */
    //float[] DropRate; // this will be the rate at which the engine rpm drops when above 1000 RPM for each gear
    
    // these variables will be used to organise the rpm drop caused by dropping car into gear




    // Start is called before the first frame update
    void Start()
    {
        StallEngine();
        rgd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PrimaryKeyPresses();
        if (EngineOn == true) { EngineKeyPresses(); } //if the engine is on check for key presses
        
        if (EngineOn == true && engineRPS <= EngineStallRPS) //if the engine is on and the rps drops too low the engine stalls
        {StallEngine();}

        if (Stalled == true) //if the engine is stalled reduce timer
        {
            CurrentTimer -= Time.deltaTime; if (CurrentTimer <= 0) { CurrentTimer = 0; Stalled = false; }
            //if time is up unstall engine
        } 
        if(Steering == true) { RotatePlayer(); }
    }
    void PrimaryKeyPresses()
    {
        if (EngineOn == false && Stalled == false && Input.GetKeyDown(KeyCode.I)) //if the engine is off and not stalled and ignition is pressed
        { StartEngine(); }
        if (Input.GetKeyDown(KeyCode.L)) //if the player presses L change the cursor lock state.
        {
            KeyPressed("L", 0);
            if (Cursor.lockState == CursorLockMode.None) { Cursor.lockState = CursorLockMode.Locked; Steering = true; print("Steering True"); }
            else if(Cursor.lockState == CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.None; Steering = false; print("Steering True"); }
        }

    }
    void EngineKeyPresses() //this is for all of our button press checks that only happen when the engine is on.
    {
        /* *** Clutch And Gear Change *** */
        if (Input.GetKey(KeyCode.LeftShift)) //clutch down
        {
            KeyPressed("L_Shift", 1);
            InGear = false;
        }
        if(InGear == false && Input.GetKeyDown(KeyCode.E)) //currentGear increase
        {
            KeyPressed("L_Shift+E", 0);
            CurrentGear += 1;
            if(CurrentGear >= 6) { CurrentGear = 6; }
        }
        if (InGear == false && Input.GetKeyDown(KeyCode.Q)) //currentGear decrease
        {
            KeyPressed("L_Shift+Q", 0);
            CurrentGear -= 1;
            if (CurrentGear <= 1) { CurrentGear = 1; }
        }
        if (InGear == false && Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.E)) //Put car in neutral
        {
            CurrentGear = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) //clutch up
        {
            KeyPressed("L_Shift", 3);
            InGear = true;
            MeshGears(CurrentGear);
        }





        /* *** Throttle *** */
        if (Input.GetKey(KeyCode.W))
        {
            KeyPressed("W", 1);
            Throttle = true;
            accelTime += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeyPressed("W", 2);
            Throttle = false;
            accelTime = 0;
        }

        RPSUpdate();
        EngineForce();
        AddDragForce();
    }
    void EngineForce()
    {
        rgd.AddForce(rgd.transform.forward * engineRPS * StartForce[CurrentGear] * forceScalar);
    }
    void RotatePlayer()
    {
        rgd.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), 0));
    }
    void RPSUpdate()
    {
        if (Throttle == true) { engineRPS += RPMIncrease[CurrentGear] * accelTime*accelTime * Time.deltaTime; }
        else { engineRPS -= dropRate[CurrentGear] * Time.deltaTime; }
        if (engineRPS >= 95) { engineRPS = 95; }
        if (engineRPS <= 17) { engineRPS = 17; }
    }

    void MeshGears(int gear)
    {
        if(gear == 0) { }
        if(gear == 1) { }
    }
    void AddDragForce()
    {
        float dragCoefficient = 0.5f;
        float densOfAir = 1.293f;
        float dragForceMag = 0.5f * densOfAir * rgd.velocity.magnitude * rgd.velocity.magnitude * dragCoefficient * Mathf.PI * 0.25f;
        rgd.AddForce(-1.0f * rgd.velocity.normalized * dragForceMag);
    }
    void StallEngine()
    {
        CurrentGear = 0;
        engineRPS = 0;
        Stalled = true;
        CurrentTimer = stallTimer;
    }

    /* *** Rare Use Functions *** */
    void StartEngine()
    {
        CurrentGear = 0;
        EngineOn = true;
        engineRPS = 17.0f;
        print("Engine_Started");
    }

    void StopEngine()
    {

    }




    //Debugging

    void KeyPressed(String button, int Type)
    {
        string action = "";
        if (Type == 0) { action = "Pressed"; }
        if (Type == 1) { action = "Held"; }
        if (Type == 2) { action = "Released"; }
        
        if (Keylogging == true) { print("Button_" + button + "_" + action); }
    }

}
