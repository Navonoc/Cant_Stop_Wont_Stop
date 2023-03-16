using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Movement : MonoBehaviour
{
    /* *** DEBUG VARS *** */
    public bool displayFunctionCalls;
    public bool Steering;

    /* *** EDITOR VARS *** */
    public GameObject ring;
    public GameObject arrow;
    public GameObject arc;
    public GameObject inds;
    public GameObject CentL;
    public GameObject CentR;
    private Rigidbody rgd;
    private GameObject playerRoot;

    /* *** ENGINE VARS *** */
    private bool gasPedalDown;
    private float engineForceMag;
    private Vector3 engineDir;

    /* *** RESIST VARS *** */
    private float centripetalForceMag;
    private Vector3 centripetalForceDir;
    private float dragForceMag;

    /* *** ASSIST VARS *** */
    private float supplementaryForceMag;

    /* *** ENGINE VARS *** */
    public float[] peakTorque;
    public float rateOfIncrease;


    /* *** CONFIG VARS *** */
    public float ringRotationSpeed; // Average rotation speed over time
    public int engineRPM;
    public int currentGear;
    public float maxEngineForce;
    public float minEngineForce;
    public float maxArrowDist;
    public float maxCentArrowDist;
    public float minCentArrowDist;
    public float turnSpeed;
    public float dragForceScalar;
    public float centForceMax;
    private float maxVelocity;
    private float minVelocity;
    private float gradVelocity;
    private float mouseInput;
    private Vector3 currentVel;
    private bool clutchDown;
    private bool engineOff;

    /*
     * ///////  ///////  ///////
     * /////// - MAINS - ///////
     * ///////  ///////  ///////
     */
    void Start()
    {
        rgd = GetComponent<Rigidbody>();
        playerRoot = GetComponent<GameObject>();
        engineOff = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            engineOff = false;
            engineRPM = 1000;
        }
        currentVel = rgd.velocity;
        if (Input.GetKeyDown(KeyCode.L) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(Input.GetKeyDown(KeyCode.L) && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            clutchDown = true;
        }
        if(CheckGasPedal() == true)
        {
            AddEngineForce();
        }

        if (Input.GetKeyDown(KeyCode.E) && clutchDown == true)
        {
            currentGear += 1;
        }

            AddEngineForce();
        
        AddDragForce();
        
        if (Steering == true) { RotatePlayer(); }
        RotateRing(engineRPM);
        AllignIndicators();
    }
    /*
    * ///////  /////////  ///////
    * /////// - ACTIONS - ///////
    * ///////  /////////  ///////
    */
    void RotatePlayer()
    {
        rgd.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * mouseInput, 0));

    }
    void RotateRing(int current)
    {
        ring.transform.Rotate(new Vector3(RingRotation(current), 0, 0));
    }
    void AddEngineForce()
    {
        CalcRPMChange();
        rgd.AddForce(engineDir * torqueCurve1(engineRPM) * Time.deltaTime);
    }
    void AddDragForce()
    {
        rgd.AddForce(-1.0f * rgd.velocity.normalized * CalcDragForce() * Time.deltaTime);
    }
    void AllignIndicators()
    {
        arc.transform.position = ArcVect();
    }
    /*
     * ///////  ////////  ///////
     * /////// - CHECKS - ///////
     * ///////  ////////  ///////
     */
    void DebugPrints(string FuncDesc)
    {
        if(displayFunctionCalls == true)
        {
            print("Function Called: " + FuncDesc);
        }
    }
    void DebugPrints(string FuncDesc, string Result)
    {
        if (displayFunctionCalls == true)
        {
            print("Function Called: " + FuncDesc + " Result: " +Result);
        }
    }

    Vector3 ArcVect()
    {
        Vector3 Dir = transform.forward;
        Dir = inds.transform.position + Dir.normalized;
        return Dir;
    }
        bool CheckGasPedal()
    {
        if (Input.GetKeyDown(KeyCode.W)) { gasPedalDown = true; }
        if (Input.GetKey(KeyCode.W)) { gasPedalDown = true; }
        if (Input.GetKeyUp(KeyCode.W)) { gasPedalDown = false; }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { gasPedalDown = false; }
        if (Input.GetKey(KeyCode.LeftShift)) { gasPedalDown = false; }
        return gasPedalDown;
    }

    /*
     * ///////  ///////  ///////
     * /////// - CALCS - ///////
     * ///////  ///////  ///////
     */
    float RingRotation(int current)
    {
        float ringRotation = ringRotationSpeed * current * Time.deltaTime;
        DebugPrints("Ring Rotation Equations. current ring rotation", ringRotation.ToString());
        return ringRotation;
    }
    float CalcDragForce()
    {
        float dragCoefficient = 0.5f;
        float densOfAir = 1.293f;
        dragForceMag = 0.5f * densOfAir * rgd.velocity.magnitude * rgd.velocity.magnitude * dragCoefficient * Mathf.PI * 0.25f * Time.deltaTime;
        DebugPrints("Drag Force calculations", dragForceMag.ToString());
        return dragForceMag;
    }
    void CalcRPMChange()
    {
        engineRPM += Mathf.RoundToInt(rateOfIncrease);
        if(engineRPM >= 5000) { engineRPM = 5000; }
    }

    float torqueCurve1(int RPM)
    {
        return ((RPM / 5000) * peakTorque[currentGear]) * (RPM/60.0f);
    }
}
