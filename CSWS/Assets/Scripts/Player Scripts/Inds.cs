using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Inds : MonoBehaviour
{
    public Engine eng;
    public GameObject ring;
    public GameObject[] Arms;

    public float rockets;
    public float rocketsMax;

    public float Bullets;
    public float BulletsMax;


    public float ringRotationSpeed;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rockets > rocketsMax) { rockets = rocketsMax; }
        if(Bullets > BulletsMax) { Bullets = BulletsMax; }
        RotateRing(Mathf.RoundToInt(eng.velocityInFwdDir));
        bool rocketsAvailabe;
        bool Ammo;
        if (rockets > 0) { rocketsAvailabe = true; } else { rocketsAvailabe = false; }
        if(Bullets > 0) { Ammo = true; } else { Ammo = false; }
        setArms(rocketsAvailabe, Ammo);
    }
    void RotateRing(int current)
    {
        ring.transform.Rotate(new Vector3(RingRotation(current), 0, 0));
    }
    float RingRotation(int current)
    {
        float ringRotation = ringRotationSpeed * current * Time.deltaTime;

        return ringRotation;
    }

    void setArms(bool Rockets, bool Ammo)
    {
        if(Rockets == true || Ammo == true)
        {
            Arms[0].SetActive(false);
            Arms[1].SetActive(true);
        }
        else
        {
            Arms[1].SetActive(false);
            Arms[0].SetActive(true);
        }
        if(Rockets == true)
        {
            Arms[2].SetActive(true);
        }
        else
        {
            Arms[2].SetActive(false);
        }
        if(Ammo == true)
        {
            Arms[3].SetActive(true);
        }
        else
        {
            Arms[3].SetActive(false);
        }
    }

    void updateArm(int arm, bool setState)
    {
        if (Arms[arm].activeSelf == setState) { return; }
        else 
        {
            Arms[arm].SetActive(true);
        }
    }

}
