using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomClass : MonoBehaviour
{
    public bool isHallway;
    public string roomName = "";
    public Transform roomCam;
    public Vector4 md;

    public GameObject[] assets;
    
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
