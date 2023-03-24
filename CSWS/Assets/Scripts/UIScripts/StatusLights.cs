using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusLights : MonoBehaviour
{
    public GameObject player;

    public GameObject engLight;
    public GameObject stallLight;

    Engine eng;
    void Start()
    {
        eng = player.GetComponent<Engine>();
    }
    // Update is called once per frame
    void Update()
    {
        if(eng.EngineOn == true) { engLight.SetActive(true); }
        else { engLight.SetActive(false); }
        if (eng.Stalled == true) { stallLight.SetActive(true); }
        else { stallLight.SetActive(false); }
    }
}
