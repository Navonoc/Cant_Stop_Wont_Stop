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
    public Vector3[] Gears;

    /* *** CONFIG VARS *** */
    public float ringRotationSpeed; // Average rotation speed over time
    public int currentGear;
    public float maxEngineForce;
    public float minEngineForce;
    public float maxArrowDist;
    public float maxCentArrowDist;
    public float minCentArrowDist;
    public float turnSpeed;
    public float dragForceScalar;
    public float centForceScalar;
    public float centForceMax;
    private float maxVelocity;
    private float minVelocity;
    private float gradVelocity;
    private float mouseInput;
    private Vector3 currentVel;

    /*
     * ///////  ///////  ///////
     * /////// - MAINS - ///////
     * ///////  ///////  ///////
     */
    void Start()
    {
        currentGear = 0;
        maxVelocity = Gears[currentGear].x;
        minVelocity = Gears[currentGear].y;
        CentL.transform.position = inds.transform.position + inds.transform.right * -minCentArrowDist;
        CentR.transform.position = inds.transform.position + inds.transform.right * minCentArrowDist;
        gradVelocity = Gears[currentGear].z;
        rgd = GetComponent<Rigidbody>();
        playerRoot = GetComponent<GameObject>();
    }

    void Update()
    {
        currentVel = rgd.velocity;
        if (Input.GetKeyDown(KeyCode.L) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(Input.GetKeyDown(KeyCode.L) && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if(CheckGasPedal() == true)
        {
            AddEngineForce();
        }
        else
        {
            CalcEngineForce(minEngineForce);
        }
        AddDragForce();
        mouseInput = Input.GetAxis("Horizontal");
        AddCentripetalForce();
        if (Steering == true) { RotatePlayer(); }
        RotateRing();
        AllignIndicators();
    }
    /*
    * ///////  /////////  ///////
    * /////// - ACTIONS - ///////
    * ///////  /////////  ///////
    */
    void AddCentripetalForce()
    {
        rgd.AddForce(CalcCentripetalForce());
    }
    void RotatePlayer()
    {
        rgd.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * mouseInput, 0));

    }
    void RotateRing()
    {
        ring.transform.Rotate(new Vector3(RingRotation(engineForceMag), 0, 0));
    }
    void AddEngineForce()
    {
        CalcEngineForce(maxEngineForce);
        rgd.AddForce(engineDir * engineForceMag * ((maxVelocity - rgd.velocity.magnitude)/maxVelocity-minVelocity) * Time.deltaTime);
    }
    void AddDragForce()
    {
        rgd.AddForce(-1.0f * rgd.velocity.normalized * CalcDragForce() * dragForceScalar * Time.deltaTime);
    }
    void AllignIndicators()
    {
        arrow.transform.position = ArrowVect();
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
    Vector3 ArrowVect()
    {
        Vector3 Dir = new Vector3(rgd.velocity.x,0,rgd.velocity.z);
        float Dist = maxArrowDist * (rgd.velocity.magnitude / (maxVelocity - minVelocity));
        Dir = inds.transform.position + Dir.normalized * Dist;
        return Dir;
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
    float RingRotation(float current)
    {
        float ringRotation = ringRotationSpeed * current * Time.deltaTime;
        DebugPrints("Ring Rotation Equations. current ring rotation", ringRotation.ToString());
        return ringRotation;
    }
    void CalcSupplementaryForce()
    {
        DebugPrints("Supplementary Force calculations");
    }
    float CalcDragForce()
    {
        float dragCoefficient = 0.5f;
        float densOfAir = 1.293f;
        dragForceMag = 0.5f * densOfAir * rgd.velocity.magnitude * rgd.velocity.magnitude * dragCoefficient * Mathf.PI * 0.25f * Time.deltaTime;
        DebugPrints("Drag Force calculations", dragForceMag.ToString());
        return dragForceMag;
    }
    Vector3 CalcCentripetalForce()
    {
        float currentAngle = Vector3.Dot(centripetalForceDir, currentVel);
        DebugPrints("Centripetal Force calculations");
        centripetalForceDir = rgd.transform.right * Mathf.Clamp(Vector3.Dot(centripetalForceDir, currentVel), -1.0f, 1.0f);
        centripetalForceMag = Vector3.Dot(centripetalForceDir, currentVel) * centForceScalar * Time.deltaTime;
        if(currentAngle <= 0)
        {
            //Left
            CentL.transform.position = inds.transform.position + centripetalForceDir * currentAngle;
        }
        else if(currentAngle >= 0)
        {
            //Right
            CentR.transform.position = inds.transform.position + centripetalForceDir * currentAngle;
        }
        return centripetalForceDir * centripetalForceMag;
    }
    void CalcEngineForce(float magnitude)
    {
        engineDir = transform.forward;
        engineForceMag = magnitude;
        DebugPrints("Engine Force calculationsS", engineDir.ToString());
    }
}
