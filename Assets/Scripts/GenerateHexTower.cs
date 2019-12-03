using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateHexTower : MonoBehaviour
{
    public float height;
    public float defaultHeight;
    public float radius;
    public bool pointOnTop = true;
    public MeshFilter hexTower;
    public Mesh towerMesh;
    public Grid grid;

    void Start()
    {
        GenerateMesh();
    }

    void Update()
    {
        
    }

    void GenerateMesh() {
        //initial hexagon
        towerMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = towerMesh;
        Vector3[] verts = new Vector3[7];   //points for the hexagon, 0 is center then top (or left if !pointOntop) and then counter clockwise 
       
        if (pointOnTop) {
            verts[0] = gameObject.transform.position;
            verts[0].z = height;    //Just incase of stupidity
            verts[1] = new Vector3(verts[0].x, verts[0].y + radius, verts[0].z);
            verts[2] = new Vector3(verts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[3] = new Vector3(verts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[4] = new Vector3(verts[0].x, verts[0].y - radius, verts[0].z);
            verts[5] = new Vector3(verts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[6] = new Vector3(verts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
        }
        else {
            verts[0] = gameObject.transform.position;
            verts[0].z = height;    //Just incase of stupidity
            verts[1] = new Vector3(verts[0].x - radius, verts[0].y, verts[0].z);
            verts[2] = new Vector3(verts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
            verts[3] = new Vector3(verts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
            verts[4] = new Vector3(verts[0].x + radius, verts[0].y, verts[0].z);
            verts[5] = new Vector3(verts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
            verts[6] = new Vector3(verts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
        }

        int[] hexTriangles = new int[] {   
            0,2,1,
            0,3,2,
            0,4,3,
            0,5,4,
            0,6,5,
            0,1,6
        };

        towerMesh.Clear();
        towerMesh.vertices = verts;
        towerMesh.triangles = hexTriangles;

        List<Vector3> vorts = new List<Vector3>(verts);

        //skirt
        towerMesh.subMeshCount = 7;

        Mesh skirtMesh = new Mesh();
        Vector3[] skirtVerts = new Vector3[4];
        int[] skirtTriangles;

        for (int i = 1; i < 7; i++)
        {
            skirtVerts[0] = verts[i];
            skirtVerts[1] = new Vector3(verts[i].x, verts[i].y, verts[i].z + height);
            if (i==6)
            {
                skirtVerts[2] = new Vector3(verts[1].x, verts[1].y, verts[1].z + height);
                skirtVerts[3] = verts[1];
            }
            else
            {
                skirtVerts[2] = new Vector3(verts[i + 1].x, verts[i + 1].y, verts[i + 1].z + height);
                skirtVerts[3] = verts[i + 1];
            }

            for (int j = 0; j < 4; j++)
            {
                if (j==1 || j==2)
                {
                    vorts.Add(skirtVerts[j]);
                }
               
            }

            if (i==6)   //otherwise it tries to make a triangle with 6,7,17, & 18, and that's not what we want
            {
                skirtTriangles = new int[] {
                    ((2 * (i-1)) + 8),((2 * (i-1)) + 7), i,
                    i, 1,((2 * (i-1)) + 8)
                };
            }

            else
            {
                skirtTriangles = new int[] {
                    ((2 * (i-1)) + 8),((2 * (i-1)) + 7), i,
                    i, i+1,((2 * (i-1)) + 8)
                };
            }


            towerMesh.SetVertices(vorts); 
            towerMesh.SetTriangles(skirtTriangles, i);
            


            //skirtMesh.Clear();
            //skirtMesh.vertices = skirtVerts;
            //skirtMesh.triangles = skirtTriangles;
            //CombineInstance tempCombine = new CombineInstance();
            //tempCombine.lightmapScaleOffset = Vector4.zero;
            //tempCombine.mesh = skirtMesh;
            //tempCombine.realtimeLightmapScaleOffset = Vector4.zero;
            //tempCombine.subMeshIndex = 0;
            //tempCombine.transform = Matrix4x4.zero;

            //CombineInstance[] comb = { tempCombine };

            //towerMesh.CombineMeshes(comb);

        }
        towerMesh.SetTriangles(towerMesh.triangles, 0);
    }
}
