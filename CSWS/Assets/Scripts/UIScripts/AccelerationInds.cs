using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationInds : MonoBehaviour
{
    public GameObject player;

    public GameObject AccelInd;
    public GameObject DeccelInd;
    public GameObject ShiftUp;
    public GameObject ShiftDown;

    Engine eng;
    void Start()
    {
        eng = player.GetComponent<Engine>();
    }
    // Update is called once per frame
    void Update()
    {
        if (eng.RPSIncreasing == true) { AccelInd.SetActive(true); DeccelInd.SetActive(false); }
        if (eng.RPSIncreasing == false) { DeccelInd.SetActive(true); AccelInd.SetActive(false); }
        if(eng.velocityInFwdDir <= eng.minSpeeds[eng.CurrentGear]) { ShiftDown.SetActive(true);}
        else { ShiftDown.SetActive(false); }
        if(eng.velocityInFwdDir >= eng.minSpeeds[eng.CurrentGear+1]) { ShiftUp.SetActive(true);}
        else { ShiftUp.SetActive(false); }
        
    }
}
