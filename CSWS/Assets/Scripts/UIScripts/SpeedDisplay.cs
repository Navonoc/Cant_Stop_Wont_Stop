using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    public GameObject Player;
    public Config config;

    public GameObject MPH;
    public GameObject KPH;

    public GameObject[] Hundreds;
    public GameObject[] Tens;
    public GameObject[] Ones;
    float DisplayV;
    Engine Eng;

    // Start is called before the first frame update
    void Start()
    {
        Eng = Player.GetComponent<Engine>();
        UpdateReadout("0"[0], "0"[0], "0"[0]);
        if (config.DisplaySpeedInMPH){MPH.SetActive(true); KPH.SetActive(false);}
        else { MPH.SetActive(false); KPH.SetActive(true); }
    }

    // Update is called once per frame
    void Update()
    {
        //DisplayV = ConvertV(Eng.velocityInFwdDir);
        DisplayV = ConvertV(Player.GetComponent<Rigidbody>().velocity.magnitude);
        string str = DisplayV.ToString();
        
        if (str.Length == 1)
        {
            UpdateReadout(str[0]);
        }
        if (str.Length == 2)
        {
            UpdateReadout(str[0], str[1]);
        }
        if (str.Length == 3)
        {
            UpdateReadout(str[0], str[1], str[2]);
        }
    }

    void UpdateReadout(char firstPos, char secondPos, char thirdPos)
    {
        for(int i = 0; i < Hundreds.Length; i++)
        {
            if(i == int.Parse(firstPos.ToString()))
            {
                Hundreds[i].SetActive(true);
            }
            else
            {
                Hundreds[i].SetActive(false);
            }
        }
        for (int j = 0; j < Tens.Length; j++)
        {
            if (j == int.Parse(secondPos.ToString()))
            {
                Tens[j].SetActive(true);
            }
            else
            {
                Tens[j].SetActive(false);
            }
        }
        for (int k = 0; k < Ones.Length; k++)
        {
            if (k == int.Parse(thirdPos.ToString()))
            {
                Ones[k].SetActive(true);
            }
            else
            {
                Ones[k].SetActive(false);
            }
        }
    }
    void UpdateReadout(char secondPos, char thirdPos)
    {
        for (int i = 0; i < (Hundreds.Length - 1); i++)
        {
            if (i == 0)
            {
                Hundreds[i].SetActive(true);
            }
            else
            {
                Hundreds[i].SetActive(false);
            }
        }
        for (int j = 0; j < Tens.Length; j++)
        {
            if (j == int.Parse(secondPos.ToString()))
                {
                Tens[j].SetActive(true);
            }
            else
            {
                Tens[j].SetActive(false);
            }
        }
        for (int k = 0; k < Ones.Length; k++)
        {
            if (k == int.Parse(thirdPos.ToString()))
            {
                Ones[k].SetActive(true);
            }
            else
            {
                Ones[k].SetActive(false);
            }
        }
    }
    void UpdateReadout(char thirdPos)
    {
        for (int i = 0; i < (Hundreds.Length-1); i++)
        {
            if (i == 0)
            {
                Hundreds[i].SetActive(true);
            }
            else
            {
                Hundreds[i].SetActive(false);
            }
        }
        for (int j = 0; j < (Tens.Length-1); j++)
        {
            if (j == 0)
            {
                Tens[j].SetActive(true);
            }
            else
            {
                Tens[j].SetActive(false);
            }
        }
        for (int k = 0; k < (Ones.Length - 1); k++)
        {
            if (k == int.Parse(thirdPos.ToString()))
            {
                Ones[k].SetActive(true);
            }
            else
            {
                Ones[k].SetActive(false);
            }
        }
    }
    float ConvertV(float V)
    {
        if (config.DisplaySpeedInMPH){return Mathf.RoundToInt(V * 2.23694f);}
        else {return Mathf.RoundToInt(V * 3.60000f);}
    }
}

