using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomClass : MonoBehaviour
{
    public bool randomRoom;
    private void Awake()
    {
        if(randomRoom)
            PuzzlePosition = transform.GetChild(1).position;
    }

    public bool isHallway;
    public string roomName = "";
    public Transform roomCam;
    public Vector4 md;

    public int Boss_State;

    public GameObject[] assets;

    public Vector3 PuzzlePosition { get; private set; }

    public GameObject Puzzle { get; set; }

    public MeshRenderer[] GetRenderers()
    {
        MeshRenderer[] R = new MeshRenderer[assets.Length];
		
        for(int i = 0; i < assets.Length; i++)
			R[i] = assets[i].GetComponent<MeshRenderer>();
		
		return R;
		
    }



    public Material[] GetMaterials()
    {
        MeshRenderer[] g = GetRenderers();
        List<Material> matList = new List<Material>();

        foreach(MeshRenderer x in g)
        {
            foreach(Material y in x.materials)
            {
                matList.Add(y);
            }
        }

        return matList.ToArray();

    }
    
}
