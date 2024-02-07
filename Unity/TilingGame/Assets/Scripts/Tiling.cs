using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiling : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] WorldBlocks;
    private int nWorldBlocks;
    public int nWorldBlocksAroundPlayerTest = 3;
    public float maxDistFromPlayerWorldBlockSpawn = 4.0f;
    public float worldBlockVariation = 0.2f;
    public float worldBlockJitter = 0.12412f;
    Dictionary<Vector2Int, string> takenWorldBlockCoordinates = new Dictionary<Vector2Int, string>();
    private float worldBlockSpawnTreshDelta;
    private int nWorldBlocksBuilt = 0;

    // Start is called before the first frame update
    void Start()
    {
        nWorldBlocks = WorldBlocks.Length;
        worldBlockSpawnTreshDelta = 1.0f / nWorldBlocks; // Perlin noise returns value [0,1]
    }

    // Update is called once per frame
    void Update()
    {
        float worldBlockCenterDist = 1.0f; // TODO: compute from Cube prefab.
        float PlayerCoX = Player.transform.position.x;
        float PlayerCoZ = Player.transform.position.z;
        int k = (int)Mathf.Floor(PlayerCoX);
        int l = (int)Mathf.Floor(PlayerCoZ);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (!takenWorldBlockCoordinates.ContainsKey(new Vector2Int(i, j)))
                {
                    Vector3 CubeCo = new Vector3(worldBlockCenterDist*i, 0, worldBlockCenterDist*j);
                    Vector3 PlayerCo = new Vector3(PlayerCoX, 0, PlayerCoZ);
                    Vector3 PlayerToCube = CubeCo - PlayerCo;
                    if (PlayerToCube.magnitude < maxDistFromPlayerWorldBlockSpawn)
                    {
                        float noise = Mathf.PerlinNoise(
                            CubeCo.x*worldBlockVariation+worldBlockJitter, 
                            CubeCo.y*worldBlockVariation+worldBlockJitter*2.0f);
                        float tresh = worldBlockSpawnTreshDelta;
                        for (int cIdx = 0; cIdx < nWorldBlocks; cIdx++)
                        {
                            if (noise > tresh - worldBlockSpawnTreshDelta && noise < tresh)
                            {
                                GameObject worldBlockInst = Instantiate(WorldBlocks[cIdx], CubeCo, Quaternion.identity);
                                nWorldBlocksBuilt++;
                                Debug.Log("World Blocck created! " + nWorldBlocksBuilt);
                                takenWorldBlockCoordinates[new Vector2Int(i, j)] = worldBlockInst.name;
                                break;
                            }
                            tresh += worldBlockSpawnTreshDelta;
                        }
                    }
                } 
            }
        }
    }
}
