using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public bool Debug;
    public Transform sphere;
    public float xOffset;
    public float yOffset;
    public float zOffset;
    public bool usePresets;
    public Vector3[] manual_Sets1;

    private int currentPreset;
    private Vector3 currentView;

    // Start is called before the first frame update
    void Start()
    {
        if (usePresets == true)
        {
            currentPreset = 0;
            RefreshView(manual_Sets1[currentPreset]);
        }
        else
        {
            xOffset = transform.position.x - sphere.position.x;
            yOffset = transform.position.y - sphere.position.y;
            zOffset = transform.position.z - sphere.position.z;
            RefreshView(new Vector3(xOffset, yOffset, zOffset));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && usePresets == true)
        {
            if (Debug == true) { print("CurrentView:_" + currentPreset); }
            if (currentPreset == (manual_Sets1.Length - 1))
            {
                if (Debug == true) { print("FirstView"); }
                currentPreset = 0;
                RefreshView(manual_Sets1[currentPreset]);
            }
            else
            {
                currentPreset += 1;
                if (Debug == true) { print("View_" + currentPreset); }
                RefreshView(manual_Sets1[currentPreset]);
            }
        }
        transform.position = new Vector3(sphere.position.x + currentView.x, sphere.position.y + currentView.y, sphere.position.z + currentView.z);
        transform.LookAt(sphere.position);
    }
    void RefreshView(Vector3 View)
    {
        currentView = View;
    }
}
