using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevBarScript : MonoBehaviour
{
    public Engine eng;
    Image Bar;
    // Start is called before the first frame update
    void Start()
    {
        Bar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Bar.fillAmount = CalcFillAmount();
    }

    float CalcFillAmount()
    {
        if(eng.engineStarting == true)
        {
            return 1 - (eng.StartTimer/ eng.config.engSpinUpTime);
        }

        if(eng.engineRPS <= eng.config.MinRPS) { return 0; }
        return eng.engineRPS / (eng.config.MaxRPS - eng.config.MinRPS);
    }
}
