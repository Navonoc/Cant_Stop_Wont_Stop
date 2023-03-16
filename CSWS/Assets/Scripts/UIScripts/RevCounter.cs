using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RevCounter : MonoBehaviour
{
    public GameObject Player;
    public float revCurrent;
    Engine Eng;
    Text revText;
    // Start is called before the first frame update
    void Start()
    {
        revText = GetComponent<Text>();
        Eng = Player.GetComponent<Engine>();
    }

    // Update is called once per frame
    void Update()
    {
        revCurrent = Eng.engineRPS * 60.0f;
        revText.text = "Revs: " + revCurrent.ToString();
    }
}
