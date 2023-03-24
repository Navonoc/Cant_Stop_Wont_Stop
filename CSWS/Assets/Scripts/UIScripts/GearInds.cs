using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearInds: MonoBehaviour
{
    public GameObject Player;

    Engine eng;
    public GameObject[] Gear;
    // Start is called before the first frame update
    void Start()
    {
        eng = Player.GetComponent<Engine>();
    }

    // Update is called once per frame
    void Update()
    {
        SetGear(eng.CurrentGear);
    }

    void SetGear(int GearCur)
    {
        for(int i = 0; i < Gear.Length; i++)
        {
            if(i == GearCur)
            {
                Gear[i].SetActive(true);
            }
            else
            {
                Gear[i].SetActive(false);
            }
        }
    }
}
