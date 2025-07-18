using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGeneration : MonoBehaviour
{


    public GameObject plane;

    public GameObject player;

    public int radius = 10;
    private int planeOffset = 11;

    // water start position
    public float HeightOffset = -3.12f;

    private Vector3 startPos = Vector3.zero;

    // X and Z player movement
    private int XPlayerMove => (int)(player.transform.position.x - startPos.x);
    private int ZPlayerMove => (int)((player.transform.position.z - startPos.z));

    // X and Z Player location
    private int XPlayerLocation => (int)Mathf.Floor(player.transform.position.x / planeOffset) * planeOffset;
    private int ZPlayerLocation => (int)Mathf.Floor(player.transform.position.z / planeOffset) * planeOffset;

    Vector2 PlayerChunkPos;


    private Hashtable tilePlane = new Hashtable();


    private void Awake()
    {
        startPos = new Vector3(0, HeightOffset, 0);
    }

    ///////////////////////////////////

    void FixedUpdate()
    {
        if (startPos == Vector3.zero)
        {
            Hashtable newTiles = new Hashtable();
            float cTime = Time.realtimeSinceStartup;

            PlayerChunkPos = new Vector2(Mathf.Floor((player.transform.position.x + 5) / 10), Mathf.Floor((player.transform.position.z + 5) / 10));

            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3 pos = new Vector3((x * planeOffset + XPlayerLocation),//  X
                        0,//                                                          Y
                        (z * planeOffset + ZPlayerLocation));//                       Z

                    if (Vector2.Distance(PlayerChunkPos, PlayerChunkPos + new Vector2(x, z)) <= radius / 1.5f)
                    {
                        if (!tilePlane.Contains(pos))// if there isnt already a plane in that position
                        {
                            GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
                            Tile t = new Tile(cTime, _plane);
                            tilePlane.Add(pos, t);
                        }
                        else
                        {
                            ((Tile)tilePlane[pos]).Timestamp = cTime;
                        }
                    }
                }
            }

            foreach (Tile t in tilePlane.Values)
            {
                if (!t.Timestamp.Equals(cTime))
                {
                    Destroy(t.tileObject);
                }
                else
                {
                    newTiles.Add(t.tileObject, t);
                }
            }

            tilePlane = newTiles;
            startPos = player.transform.position;
        }

        if (hasPlayerMoved())
        {
            Hashtable newTiles = new Hashtable();
            float cTime = Time.realtimeSinceStartup;

            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3 pos = new Vector3((x * planeOffset + XPlayerLocation),//  X
                        0,//                                                          Y
                        (z * planeOffset + ZPlayerLocation));//                       Z

                    if (Vector2.Distance(PlayerChunkPos, PlayerChunkPos + new Vector2(x, z)) <= radius / 1.5f)
                    {
                        if (!tilePlane.Contains(pos))// if there isnt already a plane in that position
                        {
                            GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
                            Tile t = new Tile(cTime, _plane);
                            tilePlane.Add(pos, t);
                        }
                        else
                        {
                            ((Tile)tilePlane[pos]).Timestamp = cTime;
                        }
                    }

                }
            }

            foreach (Tile t in tilePlane.Values)
            {
                if (!t.Timestamp.Equals(cTime))
                {
                    Destroy(t.tileObject);
                }
                else
                {
                    newTiles.Add(t.tileObject, t);
                }
            }

            tilePlane = newTiles;
            startPos = player.transform.position;
        }
    }

    bool hasPlayerMoved()
    {
        if (Mathf.Abs(XPlayerMove) >= planeOffset || Mathf.Abs(ZPlayerMove) >= planeOffset)
        {
            return true;
        }
        return false;
    }

    private class Tile
    {
        public float Timestamp;
        public GameObject tileObject;

        public Tile(float timestamp, GameObject tileObject)
        {
            this.tileObject = tileObject;
            this.Timestamp = timestamp;
        }
    }
}
