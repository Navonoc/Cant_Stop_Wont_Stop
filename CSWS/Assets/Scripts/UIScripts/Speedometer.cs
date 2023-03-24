using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public float minRot; //this is the rotation the arrow sits at at the current gears minimum speed
    public float maxRot; //this is the rotation the arrow sits at at the current gears max speed
    public GameObject Player;
    public GameObject Speedo;
    Engine engine;
    public RectTransform arrow;

    // Start is called before the first frame update
    void Start()
    {
        
        engine = Player.GetComponent<Engine>();
        arrow = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedRange = (engine.MaxV() - engine.MinV());

        float speedCurrent = (engine.velocityInFwdDir - engine.MinV());
        //print("speedcurrent " + speedCurrent);
        //print(speedCurrent / speedRange);
        if (engine.CurrentGear == 0) { arrow.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.eulerAngles.y, minRot); /*print("no gear set Minimum");*/ }
        else
        {

            if ((speedCurrent / speedRange) >= 1) { arrow.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.eulerAngles.y, maxRot); }
            else {; arrow.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.eulerAngles.y, 90.0f * (1.0f - (speedCurrent / speedRange))); }
        }
    }


}
