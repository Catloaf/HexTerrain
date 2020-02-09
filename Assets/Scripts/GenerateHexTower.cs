using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateHexTower : MonoBehaviour
{
    //public int hight;
    public int floorHight;
    public int ceilHight;
    public float radius;
    public MeshFilter hexTower;
    public Mesh towerMesh;
    public Grid grid;
    public Tilemap tMap;
    public GameObject hexTerrainGameObject;
    public MeshRenderer mRender;
    public Material testMaterial;
    
    //[SerializeField]
    //private bool pointOnTop;
    [SerializeField]
    private List<Vector3Int> activeTiles;
    private int hexIterator;

    private List<Material> matList;

    public void Start()
    {
        GenerateMesh();
    }
    public void GenerateMesh() {
        activeTiles.Clear();
        //matList.Clear();
         /*foreach (Transform child in transform)  //Deleting Children (primitive Spheres used for debugging so the scene doesn't get cluttered)
        {
            Destroy(child.gameObject);
        }
       if (grid.cellSwizzle == GridLayout.CellSwizzle.XYZ) //In unity, the only difference between a hegaonal grid with points on top vs flat-topped hexagons is the cell swizzle
        {
            pointOnTop = true;
        }
        else
        {
            if (grid.cellSwizzle == GridLayout.CellSwizzle.YXZ)
            {
                pointOnTop = false;
            }
            else
            {
                Debug.LogError("THIS WILL NOT WORK UNLESS THE GRID'S CELL SWIZZLE IS EITHER IN XYZ OR YXZ!!!");
            }
        }*/

        //hexagon tops
        towerMesh = new Mesh();
        
        GetComponent<MeshFilter>().mesh = towerMesh;
        
        List<Vector3> vertList = new List<Vector3>();   //List of verticies for the Mesh
        
        
        if (activeTiles == null || activeTiles.Count == 0)
        {
            GetActiveTiles();
        }
        if (matList == null || matList.Count==0)
        {
            matList = new List<Material>();
        }

        towerMesh.subMeshCount = activeTiles.Count + 7; //Each hexagonal 'tower' has 7 flat planes, each made as a submesh here 

        hexIterator = 0;
        foreach (Vector3Int hexTile in activeTiles)
        {
            Vector3[] hexVerts = new Vector3[7];   //points for the hexagon, 0 is center then top (or left if !pointOntop) and then counter clockwise 

          //  if (pointOnTop)
          //  {   //Pointy topped
                hexVerts[0] = tMap.GetCellCenterWorld(hexTile);
                hexVerts[0].z = hexTile.z;    
                hexVerts[1] = new Vector3(hexVerts[0].x, hexVerts[0].y + radius, hexVerts[0].z);
                hexVerts[2] = new Vector3(hexVerts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[3] = new Vector3(hexVerts[0].x - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[4] = new Vector3(hexVerts[0].x, hexVerts[0].y - radius, hexVerts[0].z);
                hexVerts[5] = new Vector3(hexVerts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[6] = new Vector3(hexVerts[0].x + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].z);
           // }
           /*else
            {  //Flat toppped
                hexVerts[0] = tMap.GetCellCenterWorld(hexTile);
                hexVerts[0].z = hexTile.z;
                hexVerts[1] = new Vector3(hexVerts[0].x - radius, hexVerts[0].y, hexVerts[0].z);
                hexVerts[2] = new Vector3(hexVerts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[3] = new Vector3(hexVerts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].y - (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[4] = new Vector3(hexVerts[0].x + radius, hexVerts[0].y, hexVerts[0].z);
                hexVerts[5] = new Vector3(hexVerts[0].x + (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].z);
                hexVerts[6] = new Vector3(hexVerts[0].x - (radius * Mathf.Cos(60 * (Mathf.PI / 180))), hexVerts[0].y + (radius * Mathf.Sin(60 * (Mathf.PI / 180))), hexVerts[0].z);
            }*/
            

            for (int i = 0; i < hexVerts.Length; i++)
            {
                vertList.Add(hexVerts[i]);
            }

            towerMesh.SetVertices(vertList);
           
        //skirt

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
            }


            /* if (activeTiles.IndexOf(hexTile) == activeTiles.Count-1)
             {

                 foreach (var item in vertList)
                 { 
                     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                     sphere.transform.position = item;

                     sphere.transform.SetParent(hexTerrainGameObject.transform);
                     sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                     sphere.name = "Sphere " + vertList.IndexOf(item);
                 }
             }*/
            SetTileMaterial(hexTile);
            hexIterator++;
        }


        SetTriangles();
        //towerMesh.SetTriangles(towerMesh.triangles, 0);
    }

    public void GetActiveTiles()
    {
        //tMap.CompressBounds();    //May or may not need this... we'll see I guess
        for (int i = tMap.editorPreviewOrigin.x; i < tMap.size.x; i++)
        {
            for (int j = tMap.editorPreviewOrigin.y; j < tMap.size.y; j++)
            {
                for (int l = tMap.editorPreviewOrigin.z; l < tMap.size.z; l++)
                {
                    if (tMap.HasTile(new Vector3Int(i,j,l)))
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
            int[] triangles;
            //if (pointOnTop)
            //{
                triangles = new int[] {
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
           /* }
            else
            {
                triangles = new int[] {
                    index, index + 2, index + 1,    //Start hexagon
                    index, index + 3, index + 2,
                    index, index + 4, index + 3,
                    index, index + 5, index + 4,
                    index, index + 6, index + 5,
                    index, index + 1, index + 6,    //End hexagon
                    index + 1, index + 18, index + 17,  //Skirt Flap 1
                    index + 1, index + 17, index + 6,
                    index + 2, index + 8, index+ 7,   //Skirt Flap 2
                    index + 2, index + 7, index +1,
                    index + 3, index + 10, index + 9,  //Skirt Flap 3
                    index + 3, index + 9, index + 2,
                    index + 4, index + 12, index +11,   //Skirt Flap 4
                    index + 4, index + 11, index + 3,
                    index + 5, index + 14, index + 13,  //Skirt Flap 5
                    index + 5, index + 13, index + 4,
                    index + 6, index + 16, index + 15,  //Skirt Flap 6
                    index + 6, index + 15, index + 5
                };
            }*/
            
            

            for (int i = 0; i < triangles.Length; i++)
            {
                triangleList.Add(triangles[i]);
            }
            towerMesh.SetTriangles(triangleList.ToArray(), activeTiles.IndexOf(hexTile));
            //towerMesh.RecalculateNormals();
            index += 19;
        }

    }

    public List<Vector3> GetSurroundingMidpoints(Vector3Int hexTile)    //Looks at the hexTiles' neighbors then returns 6 midpoints around the hexTile
    {
        List<Vector3> nodes = new List<Vector3>();
        List<Vector3Int> intNodes = GetNeighbors(hexTile);  //The grid coordinates of the neighbors, if neighbors don't exist, then there are fake ones with defaultHeight
        Vector3 worldCoordHexTile = tMap.GetCellCenterWorld(hexTile); //Need to do this to not lose hexTile's z value
        worldCoordHexTile.z = hexTile.z;
        Vector3 node;
        foreach (Vector3Int intNode in intNodes)        //Getting the center of the tiles in World coordinates (and making it Vector3s instead of Vector3Ints while we're at it)
        {
          //  if (pointOnTop)
           // {
                node = tMap.GetCellCenterWorld(intNode);
            /*}
            else
            {
                node.x = tMap.GetCellCenterWorld(intNode).y;
                node.y = tMap.GetCellCenterWorld(intNode).x;
            }*/
           
            node.z = intNode.z; //GetCellCenterWorld() always returns 0 for z, so putting it back here
            nodes.Add(node);
        }
        Vector3[] midpoint = new Vector3[6];
                            //All the midpoints should align to the grid intersections, so just taking the hexagon definition code from earlier for the x,y coordinates of the midpoints,
                            //Then I just need to line them up right and to work with my previous code, then determind the z value                  
       

        //if (pointOnTop)
        //{
            midpoint[0] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
            midpoint[1] = new Vector3(worldCoordHexTile.x, worldCoordHexTile.y - 0.5f, 0);
            midpoint[2] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
            midpoint[3] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
            midpoint[4] = new Vector3(worldCoordHexTile.x, worldCoordHexTile.y + 0.5f, 0);
            midpoint[5] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), 0);
      /*  }
        else
        {
            midpoint[0] = new Vector3(worldCoordHexTile.x - 0.5f, worldCoordHexTile.y, worldCoordHexTile.z);
            midpoint[1] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.z);
            midpoint[2] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), worldCoordHexTile.y - (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.z);
            midpoint[3] = new Vector3(worldCoordHexTile.x + 0.5f, worldCoordHexTile.y, worldCoordHexTile.z);
            midpoint[4] = new Vector3(worldCoordHexTile.x + (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.z);
            midpoint[5] = new Vector3(worldCoordHexTile.x - (0.5f * Mathf.Cos(60 * (Mathf.PI / 180))), worldCoordHexTile.y + (0.5f * Mathf.Sin(60 * (Mathf.PI / 180))), worldCoordHexTile.z);
        }*/


        for (int i = 0; i < 6; i++) //Setting the z values
        {
           // if (pointOnTop)
            //{
                if (i == 5)
                {
                    if (nodes[5].z == floorHight)
                    {
                        if (nodes[0].z == floorHight)  //no neighbors in this direction
                        {
                            midpoint[5].z = floorHight;
                        }
                        else                                //1 neighbor
                        {
                            midpoint[5].z = (worldCoordHexTile.z + nodes[0].z) / 2;
                        }
                    }
                    else
                    {
                        if (nodes[0].z == floorHight)  //1 neighbor
                        {
                            midpoint[5].z = (worldCoordHexTile.z + nodes[5].z) / 2;
                        }
                        else                                //2 neighbors
                        {
                            midpoint[5].z = (nodes[5].z + nodes[0].z + worldCoordHexTile.z) / 3;
                        }
                    }

                }
                else
                {
                    if (nodes[i].z == floorHight)
                    {
                        if (nodes[i + 1].z == floorHight)  //no neighbors in this direction
                        {
                            midpoint[i].z = floorHight;
                        }
                        else                                //1 neighbor
                        {
                            midpoint[i].z = (worldCoordHexTile.z + nodes[i + 1].z) / 2;
                        }
                    }
                    else
                    {
                        if (nodes[i + 1].z == floorHight)  //1 neighbor
                        {
                            midpoint[i].z = (worldCoordHexTile.z + nodes[i].z) / 2;
                        }
                        else                                //2 neighbors
                        {
                            midpoint[i].z = (nodes[i].z + nodes[i + 1].z + worldCoordHexTile.z) / 3;
                        }
                    }
                }
            /*}
            else
            {
                if (i == 5)
                {
                    if (nodes[5].z == floorHight)
                    {
                        if (nodes[0].z == floorHight)  //no neighbors in this direction
                        {
                            midpoint[5].z = floorHight;
                        }
                        else                                //1 neighbor
                        {
                            midpoint[5].z = (worldCoordHexTile.z + nodes[0].z) / 2;
                        }
                    }
                    else
                    {
                        if (nodes[0].z == floorHight)  //1 neighbor
                        {
                            midpoint[5].z = (worldCoordHexTile.z + nodes[5].z) / 2;
                        }
                        else                                //2 neighbors
                        {
                            midpoint[5].z = (nodes[5].z + nodes[0].z + worldCoordHexTile.z) / 3;
                        }
                    }

                }
                else
                {
                    if (nodes[i].z == floorHight)
                    {
                        if (nodes[i + 1].z == floorHight)  //no neighbors in this direction
                        {
                            midpoint[i].z = floorHight;
                        }
                        else                                //1 neighbor
                        {
                            midpoint[i].z = (worldCoordHexTile.z + nodes[i + 1].z) / 2;
                        }
                    }
                    else
                    {
                        if (nodes[i + 1].z == floorHight)  //1 neighbor
                        {
                            midpoint[i].z = (worldCoordHexTile.z + nodes[i].z) / 2;
                        }
                        else                                //2 neighbors
                        {
                            midpoint[i].z = (nodes[i].z + nodes[i + 1].z + worldCoordHexTile.z) / 3;
                        }
                    }
                }
            }*/
            
        }

        List<Vector3> midpoints = new List<Vector3>();
        midpoints.AddRange(midpoint);   //ugly, but I can clean it up later, 1st priority is to get things working, so not changing other code right now

        return midpoints;
    }

    public List<Vector3Int> GetNeighbors(Vector3Int hexTile)    //Makes a list of neighbors around the hexTile, making one with a z of defaultHeight if one doesn't actually exist.
                                                                //Order of neighbors: 0 = right, then go around clockwise, then a secondary loop starting at 6
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        //if (pointOnTop)
        //{
            if (hexTile.y % 2 == 0) //Even Rows
            {
                Vector3Int neighbor = new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z);    //0
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)  //Checking if there's a tile at the x, y location of neighbor because this is the only way I can think of doing it at the moment
                                                              //Only looking for it between floor hight and ceiling hight.
                                                              //Currently will only return the neighbor with the lowest Z value if multiple tiles are placed in the same tile coordinates.
                                                              //So please don't place more than 1 tile per space per grid.
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //1
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z);    //2
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z);    //3
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z);    //4
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z);    //5
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
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
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)    
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x + 1, hexTile.y - 1, hexTile.z);    //1
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //2
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z);    //3
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z);    //4
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z);    //5
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);

            }
        /*}
        else
        {
            if (hexTile.x % 2 == 0) //Even Columns
            {
                Vector3Int neighbor = new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z);    //0
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++) 
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z);    //1
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x , hexTile.y + 1, hexTile.z);    //2
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y + 1, hexTile.z);    //3
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //4
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x + 1, hexTile.y - 1, hexTile.z);    //5
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
            }
            else                    //Odd Columns
            {
                Vector3Int neighbor = new Vector3Int(hexTile.x + 1, hexTile.y + 1, hexTile.z);    //0
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)    
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y - 1, hexTile.z);    //1
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y - 1, hexTile.z);    //2
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x - 1, hexTile.y, hexTile.z);    //3
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x, hexTile.y + 1, hexTile.z);    //4
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);
                neighbor = new Vector3Int(hexTile.x + 1, hexTile.y, hexTile.z);    //5
                neighbor.z = floorHight;
                for (int i = ceilHight; i < floorHight; i++)
                {
                    if (activeTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, i)))
                    {
                        neighbor.z = i;
                        break;
                    }
                }
                neighbors.Add(neighbor);

            }
        }*/

        return neighbors;
    }
    public void SetTileMaterial(Vector3Int hexTile) {
        Sprite tempSprite = tMap.GetSprite(hexTile);
        Material mat = new Material(testMaterial);
        Texture2D tex= new Texture2D((int) tempSprite.rect.width, (int) tempSprite.rect.height);
        tex.name = "hex" + hexIterator;
        var pix = tempSprite.texture.GetPixels((int)tempSprite.textureRect.x,
                                        (int)tempSprite.textureRect.y,
                                        (int)tempSprite.textureRect.width,
                                        (int)tempSprite.textureRect.height);
        tex.SetPixels(pix);
        tex.Apply();
        testMaterial.SetTexture(tex.name, tex);
        matList.Add(mat);
        mRender.materials = matList.ToArray();
    }
    public void ClearMesh() {
        activeTiles.Clear();
        matList.Clear();
        mRender.materials = matList.ToArray();
        DestroyImmediate(towerMesh);
    }
}
