using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTurningCone : MonoBehaviour
{
    public float angle;
    public float radius;
    public GameObject Player;
    public Mesh arc;

    Engine eng;


    // Start is called before the first frame update
    void Start()
    {
        eng = Player.GetComponent<Engine>();
        Vector2[] UVs = new Vector2[4]; ;
        int[] Tris = new int[6];

        arc = new Mesh();
        GetComponent<MeshFilter>().mesh = arc;


        UpdateVerts(0.25f*Mathf.PI);
        Tris[0] = 0;
        Tris[1] = 1;
        Tris[2] = 2;
        Tris[3] = 0;
        Tris[4] = 2;
        Tris[5] = 3;

        arc.uv = UVs;
        arc.triangles = Tris;
    }

    void Update()
    {
        UpdateVerts(RecalcAngle());
        UpdateLocation();
    }

    void UpdateLocation()
    {
        transform.position = Player.transform.position;
        transform.rotation = Quaternion.LookRotation(Player.GetComponent<Rigidbody>().velocity, Player.transform.up);
    }
    void UpdateVerts(float ang)
    {
        Vector3[] Verts = new Vector3[4];
        Verts[0] = new Vector3(0, 0, 0);
        float xOffset = Mathf.Sin(ang) * radius;
        float zOffset = Mathf.Cos(ang) * radius;
        Verts[1] = new Vector3(-xOffset, 0, zOffset);
        Verts[2] = new Vector3(0, 0, zOffset);
        Verts[3] = new Vector3(xOffset, 0, zOffset);
        arc.vertices = Verts;
    }
    float RecalcAngle()
    {
        float H = Player.GetComponent<Rigidbody>().velocity.magnitude;
        float C = eng.config.MaxDeltaP;
        float Dots = Mathf.Pow(H, 2) - Mathf.Pow(C, 2);
        float A = Mathf.Sqrt(Dots);
        print("H: " + H + "Dots: " + Dots + "A: " + A + "Ratio: " + A / H + "arcCos: " + Mathf.Acos(A / H));
        return Mathf.Acos(A / H);
    }
}
