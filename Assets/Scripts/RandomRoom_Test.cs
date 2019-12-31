using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MyDetails
{
    public Vector3 ObjectMiddle;
    public float Xlength;
    public float Zlength;
}

sealed class Zone
{

    public Vector3 FirstPoint { get; }
    public Vector3 SecondPoint { get; }
    private Vector3 Middle { get; }
    public int Sector { get; }

    public Zone(MyDetails ObjParams, int section)
    {
        Sector = section;
        Middle = ObjParams.ObjectMiddle;
        Vector3 tempPoint = RandomRoom_Test.gridMultiplier[Sector];

        float tempX = Middle.x + ObjParams.Xlength * tempPoint.x;
        float tempZ = Middle.z + ObjParams.Zlength * tempPoint.z;
        FirstPoint = new Vector3(tempX, Middle.y, tempZ);

        tempPoint = RandomRoom_Test.gridMultiplier[(int)tempPoint.y];
        tempX = Middle.x + ObjParams.Xlength * tempPoint.x;
        tempZ = Middle.z + ObjParams.Zlength * tempPoint.z;
        SecondPoint = new Vector3(tempX, Middle.y, tempZ);
    }

    public Vector3 DeployPoint()
    {
        float returnX = Random.Range(FirstPoint.x, SecondPoint.x);
        float returnZ = Random.Range(FirstPoint.z, SecondPoint.z);
        return new Vector3(returnX, Middle.y, returnZ);
    }

}

public class RandomRoom_Test : MonoBehaviour
{

    public static Dictionary<int, Vector3> gridMultiplier;

    static RandomRoom_Test()
    {
        float div3 = 1f / 6f;

        gridMultiplier = new Dictionary<int, Vector3>()
        {
            { 1, new Vector3(-0.5f, 2, div3)}, //3-1
            { 2, new Vector3(-div3, 6, 0.5f)}, //1-4
            { 3, new Vector3(div3, 11, 0.5f)}, //2-5
            { 4, new Vector3(-div3, 1, -div3)}, //7-3
            { 5, new Vector3(-div3, 12, div3)}, //11-12
            { 6, new Vector3(div3, 9, div3)}, //4-8
            { 7, new Vector3(-0.5f, 10, -div3)}, //6-9
            { 8, new Vector3(div3, 4, -0.5f)}, //10-7
            { 9, new Vector3(0.5f, 8, -div3)}, //8-10
            { 10, new Vector3(-div3, 0, -0.5f)},
            { 11, new Vector3(0.5f, 0, div3)},
            { 12, new Vector3(div3, 0, -div3)}
        };

    }

    float lengthX;
    float lengthZ;

    Vector3 middle;

    public static GameObject[] Room_Objects = new GameObject[1];

    Zone[] zones = new Zone[9];

    MyDetails details;

    void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        lengthX = mr.bounds.size.x;
        lengthZ = mr.bounds.size.z;
        Debug.Log(lengthX);
        middle = transform.position;

        details = new MyDetails
        {
            ObjectMiddle = middle,
            Xlength = lengthX,
            Zlength = lengthZ
        };
 
    }

    private void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            zones[i] = new Zone(details, i + 1);
        }

        foreach (Zone z in zones)
        {

            GameObject inst_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            inst_obj.transform.position = z.DeployPoint();
            inst_obj.transform.LookAt(middle);

            /*
            //This code tests the positions of the exterior: it places all primary (spheres) and secondary (cubes) points as detailed in the graphic.
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            a.transform.position = z.FirstPoint;
            a.name = z.Sector.ToString(); 
            GameObject a2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a2.transform.position = z.SecondPoint;
            a2.name = z.Sector.ToString();
            */
            
        }

    }

}
