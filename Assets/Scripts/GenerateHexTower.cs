using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateHexTower : MonoBehaviour
{
    public int height;
    public int defaultHeight;
    public float radius;
    public bool pointOnTop = true;
    public MeshFilter hexTower;
    public Mesh towerMesh;
    public Grid grid;
    public Tilemap tMap;

    [SerializeField]
    private List<Vector3Int> activeTiles;
    
    [SerializeField]
    private List<List<Vector3Int>> triNodes;   //inner list should be a set of 3 Vector3Ints, representing the points between any given set of hex tiles

    void Start()
    {
        MakeTriNodeList();
        GenerateMesh();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //Just allowing to make changes as things go
        {
            GenerateMesh();
        }
    }

    void GenerateMesh() {
        //initial hexagon
        towerMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = towerMesh;
        Vector3[] verts = new Vector3[7];   //points for the hexagon, 0 is center then top (or left if !pointOntop) and then counter clockwise 
       
        if (pointOnTop) {   //Pointy topped
            verts[0] = gameObject.transform.position;
            verts[0].z = height;    //Just incase of stupidity
            verts[1] = new Vector3(verts[0].x, verts[0].y + radius, verts[0].z);
            verts[2] = new Vector3(verts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[3] = new Vector3(verts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[4] = new Vector3(verts[0].x, verts[0].y - radius, verts[0].z);
            verts[5] = new Vector3(verts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
            verts[6] = new Vector3(verts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].z);
        }
        else {  //Flat toppped
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
           

        }
        towerMesh.SetTriangles(towerMesh.triangles, 0);
    }

    public void GetActiveTiles()
    {
        tMap.CompressBounds();    //May or may not need this... we'll see I guess
        for (int i = tMap.editorPreviewOrigin.x; i < tMap.size.x; i++)
        {
            for (int j = tMap.editorPreviewOrigin.y; j < tMap.size.y; j++)
            {
                for (int l = tMap.editorPreviewOrigin.z; l < tMap.size.z; l++)
                {
                    if (tMap.HasTile(new Vector3Int(i,j,l)))    //Really hope the grid coordinates themselves are flat...
                    {
                        activeTiles.Add(new Vector3Int(i, j, l));
                    }
                }
            }
        }
    }

    public void MakeTriNodeList() {
        if (activeTiles == null || activeTiles.Count == 0)
        {
            GetActiveTiles();
        }
        if (triNodes == null || triNodes.Count == 0)
        {
            triNodes = new List<List<Vector3Int>>();
        }

        //List<Vector3Int> neighbors = new List<Vector3Int>();    //probably not needed
        List<Vector3Int> nodes = new List<Vector3Int>();    //the individual sets of 3 to add to triNodes

        foreach (Vector3Int hexTile in activeTiles)
        {
            /*neighbors.Add(hexTile);                                             //I think I didn't need to do this... derp
            neighbors.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
            neighbors.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
            neighbors.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
            neighbors.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
            neighbors.Add(new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z));
            neighbors.Add(new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z));*/
            nodes.Clear();
            if (hexTile.y % 2 ==0)  //Even Rows
            {
                nodes.Add(hexTile);                                             //1
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //2
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //3
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //4
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //5
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //6
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
            }
            else    //Odd Rows
            {
                nodes.Add(hexTile);                                             //1
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y - 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //2
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //3
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //4
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //5
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Clear();
                nodes.Add(hexTile);                                             //6
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
            }

            //neighbors.Clear();
        }

    }

    /*public struct TriNode { //Struct to store the information about any given 3 neighbouring hex tiles, Takes the 3 heights and gives a midpoint
        public Vector3Int hex1;
        public Vector3Int hex2;
        public Vector3Int hex3;

        public Vector3 midpoint;

        public TriNode(Vector3Int hexIn1, Vector3Int hexIn2, Vector3Int hexIn3, int dHeight)    //Need to pass in defaultHeight for stupid reasons
        {
            hex1 = hexIn1;
            hex2 = hexIn2;
            hex3 = hexIn3;

            if (hex1.z == 0 && hex2.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = hex3.x;
                midpoint.y = hex3.y;
                midpoint.z = dHeight;
            }
            else if (hex1.z == 0 && hex3.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = hex2.x;
                midpoint.y = hex2.y;
                midpoint.z = dHeight;
            }
            else if (hex2.z == 0 && hex3.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = hex1.x;
                midpoint.y = hex1.y;
                midpoint.z = dHeight;
            }
            else if (hex1.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = (hex2.x + hex3.x) / 2;
                midpoint.y = (hex2.y + hex3.y) / 2;
                midpoint.z = (hex2.z + hex3.z) / 2;
            }
            else if (hex2.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = (hex1.x + hex3.x) / 2;
                midpoint.y = (hex1.y + hex3.y) / 2;
                midpoint.z = (hex1.z + hex3.z) / 2;
            }
            else if (hex3.z == 0)
            {
                midpoint = new Vector3();
                midpoint.x = (hex1.x + hex2.x) / 2;
                midpoint.y = (hex1.y + hex2.y) / 2;
                midpoint.z = (hex1.z + hex2.z) / 2;
            }
            else
            {
                midpoint = new Vector3();
                midpoint.x = (hex1.x + hex2.x + hex3.x) / 3;
                midpoint.y = (hex1.y + hex2.y + hex3.y) / 3;
                midpoint.z = (hex1.z + hex2.z + hex3.z) / 3;
            }

        }
    }*/
}
