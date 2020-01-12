using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateHexTower : MonoBehaviour
{
    public int height;
    public int floorHieght;
    public float radius;
    //public bool pointOnTop = true;
    public MeshFilter hexTower;
    public Mesh towerMesh;
    public Grid grid;
    public Tilemap tMap;


    [SerializeField]
    private List<Vector3Int> activeTiles;
    
    [SerializeField]
    private List<List<Vector3Int>> triNodes;   //inner list should be a set of 3 Vector3Ints, representing the points between any given set of hex tiles

    private Dictionary<Vector3Int, List<Vector3>> midpointDict;

    [SerializeField]
    private List<Vector3> nodeList; //Trying a different approach for now in making the nodes...


    void Start()
    {
        //MakeTriNodeList();
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
        //hexagon tops
        towerMesh = new Mesh();
        
        GetComponent<MeshFilter>().mesh = towerMesh;
        
        List<Vector3> vertList = new List<Vector3>();   //List of verticies for the Mesh
        
        
        if (activeTiles == null || activeTiles.Count == 0)
        {
            GetActiveTiles();
        }
        towerMesh.subMeshCount = activeTiles.Count + 7; //Each hexagonal 'tower' has 7 flat planes, each made as a submesh here 

        //GenerateMidPoints();

        foreach (Vector3Int hexTile in activeTiles)
        {
            //Mesh topMesh = new Mesh();
            Vector3[] hexVerts = new Vector3[7];   //points for the hexagon, 0 is center then top (or left if !pointOntop) and then counter clockwise 

            //if (pointOnTop)
            //{   //Pointy topped
                hexVerts[0] = tMap.GetCellCenterWorld(hexTile);
                hexVerts[0].z = hexTile.z;    
                hexVerts[1] = new Vector3(hexVerts[0].x, hexVerts[0].y + radius, hexVerts[0].z);
                hexVerts[2] = new Vector3(hexVerts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[3] = new Vector3(hexVerts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[4] = new Vector3(hexVerts[0].x, hexVerts[0].y - radius, hexVerts[0].z);
                hexVerts[5] = new Vector3(hexVerts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[6] = new Vector3(hexVerts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
            /*}
            else
            {  //Flat toppped
                verts[0] = tMap.GetCellCenterWorld(hexTile);
                //verts[0].z = height;    //Just incase of stupidity
                verts[1] = new Vector3(verts[0].x - radius, verts[0].y, verts[0].z);
                verts[2] = new Vector3(verts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
                verts[3] = new Vector3(verts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
                verts[4] = new Vector3(verts[0].x + radius, verts[0].y, verts[0].z);
                verts[5] = new Vector3(verts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
                verts[6] = new Vector3(verts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), verts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), verts[0].z);
            }
            */


            for (int i = 0; i < hexVerts.Length; i++)
            {
                vertList.Add(hexVerts[i]);
            }

            //towerMesh.Clear();
            //towerMesh.vertices = verts;
            //towerMesh.triangles = hexTriangles;
            towerMesh.SetVertices(vertList);
           


        //skirt


            // Mesh skirtMesh = new Mesh();
            Vector3[] skirtQuadVerts = new Vector3[4];  //Each flap of the skirt
           
            List<Vector3> skirtVerts = GetSurroundingMidpoints(hexTile);    //Since I fixed GetSurroundingMidpoints to actually return midpoints, this MIGHT be broken now...
                                                                            // should have a count of 6

            
            for (int i = 1; i <  7; i++)
            {
                skirtQuadVerts[0] = hexVerts[i];
                skirtQuadVerts[1] = skirtVerts[i-1];
                
                if (i==6)
                {

                    skirtQuadVerts[2] = skirtVerts[0];

                    skirtQuadVerts[3] = hexVerts[1];
                }
                else
                {
                    skirtQuadVerts[2] = skirtVerts[i];

                    skirtQuadVerts[3] = hexVerts[i + 1];
                }
                

                
                for (int j = 0; j < 4; j++)
                {
                    if (j == 1 || j == 2)
                    {
                        vertList.Add(skirtQuadVerts[j]);
                    }

                }

                towerMesh.SetVertices(vertList);
                //towerMesh.SetTriangles(triangleList.ToArray(), i);


            }

               /* if (activeTiles.IndexOf(hexTile) == 0)
                {

                    foreach (var item in vertList)
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = item;
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        sphere.name = "Sphere " + vertList.IndexOf(item);
                    }
                }*/
        }

        

        SetTriangles();
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

    private void SetTriangles() {   //Let's try putting all the triangles in their own function for readability
        List<int> triangleList = new List<int>();   //List of Triangles for the Mesh
        int index = 0;
        foreach (var hexTile in activeTiles)
        {
            int[] triangles = new int[] {
                index, index + 2, index + 1,    //Start hexagon
                index, index + 3, index + 2,
                index, index + 4, index + 3,
                index, index + 5, index + 4,
                index, index + 6, index + 5,
                index, index + 1, index + 6,    //End hexagon
                index + 1, index + 13, index + 14,  //Skirt Flap 1
                index + 1, index + 14, index + 6,
                index + 2, index + 11, index+ 12,   //Skirt Flap 2
                index + 2, index + 12, index +1,
                index + 3, index + 9, index + 10,  //Skirt Flap 3
                index + 3, index + 10, index + 2,
                index + 4, index + 7, index +8,   //Skirt Flap 4
                index + 4, index + 8, index + 3,
                index + 5, index + 17, index + 18,  //Skirt Flap 5
                index + 5, index + 18, index + 4,
                index + 6, index + 15, index + 16,  //Skirt Flap 6
                index + 6, index + 16, index + 5
            };

            for (int i = 0; i < triangles.Length; i++)
            {
                triangleList.Add(triangles[i]);
            }
            towerMesh.SetTriangles(triangleList.ToArray(), activeTiles.IndexOf(hexTile));

            index += 19;
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
            nodes = new List<Vector3Int>();
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
                nodes.Add(hexTile);                                             //2
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //3
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //4
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //5
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
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
                nodes.Add(hexTile);                                             //2
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //3
                nodes.Add(new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //4
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
                nodes.Add(hexTile);                                             //5
                nodes.Add(new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z));
                nodes.Add(new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z));
                if (!triNodes.Contains(nodes))
                {
                    triNodes.Add(nodes);
                }
                nodes = new List<Vector3Int>();
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

    public void GenerateMidPoints()
    {
        midpointDict = new Dictionary<Vector3Int, List<Vector3>>();

        foreach (List<Vector3Int> triNode in triNodes)
        {
            Vector3 midpoint = new Vector3();
            Vector3Int hex1i = triNode.ToArray()[0];    //3 temp variables because can't convert directly between Vector3 and Vector3Int
            Vector3Int hex2i = triNode.ToArray()[1];
            Vector3Int hex3i = triNode.ToArray()[2];

            Vector3 hex1 = tMap.GetCellCenterWorld(hex1i);
            Vector3 hex2 = tMap.GetCellCenterWorld(hex2i);
            Vector3 hex3 = tMap.GetCellCenterWorld(hex3i);

            midpoint.x = (hex1.x + hex2.x + hex3.x) / 3;
            midpoint.y = (hex1.y + hex2.y + hex3.y) / 3;
            midpoint.z = (hex1.z + hex2.z + hex3.z) / 3;

            /*if (hex1.z == 0 && hex2.z == 0 && hex3.z == 0)
            {

            }
            if (hex1.z == 0 && hex2.z == 0)
            {           
                midpoint.x = hex3.x;
                midpoint.y = hex3.y;
                midpoint.z = defaultHeight;
            }
            else if (hex1.z == 0 && hex3.z == 0)
            {            
                midpoint.x = hex2.x;
                midpoint.y = hex2.y;
                midpoint.z = defaultHeight;
            }
            else if (hex2.z == 0 && hex3.z == 0)
            {             
                midpoint.x = hex1.x;
                midpoint.y = hex1.y;
                midpoint.z = defaultHeight;
            }
            else if (hex1.z == 0)
            {              
                midpoint.x = (hex2.x + hex3.x) / 2;
                midpoint.y = (hex2.y + hex3.y) / 2;
                midpoint.z = (hex2.z + hex3.z) / 2;
            }
            else if (hex2.z == 0)
            {               
                midpoint.x = (hex1.x + hex3.x) / 2;
                midpoint.y = (hex1.y + hex3.y) / 2;
                midpoint.z = (hex1.z + hex3.z) / 2;
            }
            else if (hex3.z == 0)
            {            
                midpoint.x = (hex1.x + hex2.x) / 2;
                midpoint.y = (hex1.y + hex2.y) / 2;
                midpoint.z = (hex1.z + hex2.z) / 2;
            }
            else
            {
                midpoint.y = (hex1.y + hex2.y + hex3.y) / 3;
                midpoint.z = (hex1.z + hex2.z + hex3.z) / 3;
            }*/

            foreach (Vector3Int hexTile in triNode)
            {        
                if (!midpointDict.ContainsKey(hexTile))
                {
                    midpointDict.Add(hexTile, new List<Vector3>());
                }
                if (!midpointDict[hexTile].Contains(midpoint))
                {
                    midpointDict[hexTile].Add(midpoint);
                }
            }

        }
    }

    public List<Vector3> GetSurroundingMidpoints(Vector3Int hexTile)    //Looks at the hexTiles' neighbors then returns 6 midpoints around the hexTile
    {
        List<Vector3> nodes = new List<Vector3>();
        List<Vector3Int> intNodes = GetNeighbors(hexTile);  //The grid coordinates of the neighbors, if neighbors don't exist, then there are fake ones with defaultHeight
        Vector3 worldCoordHexTile = tMap.GetCellCenterWorld(hexTile); //Need to do this to not lose hexTile's z value
        worldCoordHexTile.z = hexTile.z;
        //nodes.Add(worldCoordHexTile);     //Not sure why I was adding this...
        foreach (Vector3Int intNode in intNodes)        //Getting the center of the tiles in World coordinates (and making it Vector3s instead of Vector3Ints while we're at it)
        {
            Vector3 node = tMap.GetCellCenterWorld(intNode);
            node.z = intNode.z; //GetCellCenterWorld() always returns 0 for z, so putting it back here
            nodes.Add(node);
        }
        List<Vector3> midpoints = new List<Vector3>();
        Vector3[] nodeArray = nodes.ToArray();  //should have length of 6

        Vector3[] midpoint = new Vector3[6];
                            //All the midpoints should align to the grid intersections, so just taking the hexagon definition code from earlier for the x,y coordinates of the midpoints,
                            //Then I just need to line them up right and to work with my previous code (thus them being out of order), then determind the z value
        midpoint[4] = new Vector3(worldCoordHexTile.x, worldCoordHexTile.y + 0.5f, 0); 
        midpoint[3] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
        midpoint[2] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
        midpoint[1] = new Vector3(worldCoordHexTile.x, worldCoordHexTile.y - 0.5f, 0);
        midpoint[0] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
        midpoint[5] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);


        for (int i = 0; i < 6; i++) //Setting the z values
        {
            if (i==5)
            {
                if (nodeArray[5].z == floorHieght)
                {
                    if (nodeArray[0].z == floorHieght)  //no neighbors in this direction
                    {
                        midpoint[i].z = floorHieght;
                    }
                    else                                //1 neighbor
                    {
                        midpoint[i].z = (worldCoordHexTile.z + nodeArray[0].z) / 2;
                    }
                }
                else
                {
                    if (nodeArray[0].z == floorHieght)  //1 neighbor
                    {
                        midpoint[i].z = (worldCoordHexTile.z + nodeArray[5].z) / 2;
                    }
                    else                                //2 neighbors
                    {
                        midpoint[i].z = (nodeArray[5].z + nodeArray[0].z + worldCoordHexTile.z) / 3;
                    }
                }
                
            }
            else
            {
                if (nodeArray[i].z == floorHieght)
                {
                    if (nodeArray[i+1].z == floorHieght)  //no neighbors in this direction
                    {
                        midpoint[i].z = floorHieght;
                    }
                    else                                //1 neighbor
                    {
                        midpoint[i].z = (worldCoordHexTile.z + nodeArray[i+1].z) / 2;
                    }
                }
                else
                {
                    if (nodeArray[i+1].z == floorHieght)  //1 neighbor
                    {
                        midpoint[i].z = (worldCoordHexTile.z + nodeArray[i].z) / 2;
                    }
                    else                                //2 neighbors
                    {
                        midpoint[i].z = (nodeArray[i].z + nodeArray[i+1].z + worldCoordHexTile.z) / 3;
                    }
                }
            }
        }







        /*for (int i = 0; i < nodeArray.Length; i++)
        {
            if (i==5)
            {
                midpoint = Vector3.Lerp(nodeArray[5], nodeArray[0], 0.5f);  //1st the midpoint between the centerpoints of the neighbors, %5 so it aligns correctly to the 6 neighbors 0-5
            }
            else
            {
                midpoint = Vector3.Lerp(nodeArray[i], nodeArray[i + 1], 0.5f);  //1st the midpoint between the centerpoints of the neighbors, %5 so it aligns correctly to the 6 neighbors 0-5
            }
           
            midpoint = Vector3.Lerp(worldCoordHexTile, midpoint, 0.5f);            //Then the midpoint between the previous midpoint and the tile center, which SHOULD yeild the point between the 3
            midpoint.z = nodeArray[i % 5].z + nodeArray[(i + 1) % 5].z / 2; //Trying this because the Z values where not looking right....

            midpoints.Add(midpoint);
        }*/
        midpoints.AddRange(midpoint);   //ugly, but I can clean it up later, 1st priority is to get things working, so not changing other code right now

        return midpoints;
    }

    public List<Vector3Int> GetNeighbors(Vector3Int hexTile)    //Makes a list of neighbors around the hexTile, making one with a z of defaultHeight if one doesn't actually exist.
                                                                //Order of neighbors: 0 = right, then go around clockwise, then a secondary loop starting at 6
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        if (hexTile.y % 2 == 0) //Even Rows
        {
            Vector3Int neighbor = new Vector3Int(hexTile.x + 1,hexTile.y,hexTile.z);    //0
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)  //Checking if there's a tile at the x, y location of neighbor because this is the only way I can think of doing it at the moment
            {                                   //PLEASE don't have heights beyond -100 to 100!!!
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //1
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z);    //2
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z);    //3
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z);    //4
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z);    //5
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
        }
        else                    //Odd Rows
        {
            Vector3Int neighbor = new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z);    //0
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)  //Checking if there's a tile at the x, y location of neighbor because this is the only way I can think of doing it at the moment
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x + 1, hexTile.y - 1, hexTile.z);    //1
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x + 1, neighbor.y - 1, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //2
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z);    //3
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z);    //4
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);
            neighbor = new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z);    //5
            neighbor.z = floorHieght;
            for (int i = -100; i < 100; i++)
            {
                if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                {
                    neighbor.z = i;
                    break;
                }
            }
            neighbors.Add(neighbor);

        }

        return neighbors;
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
