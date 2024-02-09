using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiling : MonoBehaviour
{
    public GameObject Player;

    public GameObject[] WorldBlocks;
    private int nWorldBlocks;
    float worldBlockCenterDist;
    public int nWorldBlocksAroundPlayerTest = 3;
    public float maxDistFromPlayerWorldBlockSpawn = 4.0f;
    public float worldBlockVariation = 0.2f;
    public float worldBlockJitter = 0.12412f;
    Dictionary<Vector2Int, string> existingWorldBlockCoordinates = new Dictionary<Vector2Int, string>();
    private float worldBlockSpawnTreshDelta;
    private int nWorldBlocksBuiltDebug = 0;
    private List<GameObject> worldBlocksInAnimation = new List<GameObject>();
    private float worldBlocksAnimationSpeed = 10.0f;

    public GameObject[] WorldProps;
    private int nWorldProps;
    Dictionary<Vector2Int, string> existingWorldPropsCoordinates = new Dictionary<Vector2Int, string>();
    private float maxDistFromPlayerPropSpawn;
    private List<GameObject> worldPropsInAnimation = new List<GameObject>();
    private int nWorldPropsBuiltDebug = 0;
    private float worldPropsAnimationSpeed = 7.0f;

    public GameObject[] WorldPopups;
    private int nWorldPopups;
    Dictionary<Vector2Int, string> existingWorldPopupsCoordinates = new Dictionary<Vector2Int, string>();
    private float maxDistFromPlayerPopupSpawn;
    private List<GameObject> worldPopupsInAnimation = new List<GameObject>();
    private int nWorldPopupsBuiltDebug = 0;
    private float worldPopupsAnimationSpeed = 3.0f;
    Vector3 worldPopupsScaleChange;

    // Start is called before the first frame update
    void Start()
    {
        // World Blocks.
        worldBlockCenterDist = 1.0f; // TODO: compute from WorldBlockCo prefab.
        nWorldBlocks = WorldBlocks.Length;
        worldBlockSpawnTreshDelta = 1.0f / nWorldBlocks; // Perlin noise returns value [0,1]

        // Props Bocks.
        maxDistFromPlayerPropSpawn = maxDistFromPlayerWorldBlockSpawn * 0.6f;
        nWorldProps = WorldProps.Length;

        // Popups.
        maxDistFromPlayerPopupSpawn = maxDistFromPlayerPropSpawn * 0.6f;
        worldPopupsScaleChange = new Vector3(worldPopupsAnimationSpeed, worldPopupsAnimationSpeed, worldPopupsAnimationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerCo = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
        buildWorldBlocks(PlayerCo);
        animateWorldBlocks();
        buildWorldProps(PlayerCo);
        animateWorldProps();
        buildWorldPopups(PlayerCo);
        animateWorldPopups();
    }

    void buildWorldPopups(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                // Popup only if prop exist and it is not empty.
                bool createPopup = false;
                if (existingWorldPropsCoordinates.ContainsKey(new Vector2Int(i, j)))
                {
                    createPopup = true;
                    string existingWorldPropsCoordinatesValue;
                    existingWorldPropsCoordinates.TryGetValue(new Vector2Int(i, j), out existingWorldPropsCoordinatesValue);
                    if (existingWorldPropsCoordinatesValue == "Empty")
                    {
                        createPopup = false;
                    }
                }
                if (createPopup)
                {
                    if (!existingWorldPopupsCoordinates.ContainsKey(new Vector2Int(i, j)))
                    {
                        int nPopupsToSpawn = 4;
                        for (int pi = 0; pi < nPopupsToSpawn; pi++)
                        {
                            // World popups appear close to world props.
                            Vector3 PopupCo = new Vector3(
                                worldBlockCenterDist * i + Random.Range(-1.0f, 1.0f), 
                                0, 
                                worldBlockCenterDist * j + Random.Range(-1.0f, 1.0f));
                            Vector3 PlayerToPopup = PopupCo - PlayerCo;
                            if (PlayerToPopup.magnitude < maxDistFromPlayerPopupSpawn)
                            {
                                int worldPopupIdx = (int)Mathf.Floor(Random.value * nWorldPopups);
                                PopupCo.y = 1.5f;
                                GameObject worldPopupInst = Instantiate(WorldPopups[worldPopupIdx], PopupCo, Quaternion.identity);
                                worldPopupInst.transform.localScale = new Vector3(0,0,0);
                                existingWorldPopupsCoordinates[new Vector2Int(i, j)] = worldPopupInst.name;
                                worldPopupsInAnimation.Add(worldPopupInst);  
                                nWorldPopupsBuiltDebug++;
                                Debug.Log("World Popup created! " + nWorldPopupsBuiltDebug);
                            }
                        }
                    }
                }
            }
        }
    }

    void animateWorldPopups()
    {
        List<GameObject> updatedWorldPopupsInAnimation = new List<GameObject>();
        foreach (GameObject popup in worldPopupsInAnimation)
        {
            popup.transform.localScale += worldPopupsScaleChange * Time.deltaTime;
            if (popup.transform.localScale.x < 1.0f - Random.value + 0.1f)
            {
                updatedWorldPopupsInAnimation.Add(popup);
            } 
        }
        worldPopupsInAnimation = updatedWorldPopupsInAnimation;
    }

    void buildWorldBlocks(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (!existingWorldBlockCoordinates.ContainsKey(new Vector2Int(i, j)))
                {
                    Vector3 WorldBlockCo = new Vector3(worldBlockCenterDist*i, 0, worldBlockCenterDist*j);
                    Vector3 PlayerToWorldBlock = WorldBlockCo - PlayerCo;
                    if (PlayerToWorldBlock.magnitude < maxDistFromPlayerWorldBlockSpawn)
                    {
                        float noise = Mathf.PerlinNoise(
                            WorldBlockCo.x*worldBlockVariation+worldBlockJitter, 
                            WorldBlockCo.y*worldBlockVariation+worldBlockJitter*2.0f);
                        float tresh = worldBlockSpawnTreshDelta;
                        for (int cIdx = 0; cIdx < nWorldBlocks; cIdx++)
                        {
                            if (noise > tresh - worldBlockSpawnTreshDelta && noise < tresh)
                            {
                                WorldBlockCo.y = -3.0f;
                                GameObject worldBlockInst = Instantiate(WorldBlocks[cIdx], WorldBlockCo, Quaternion.identity);
                                nWorldBlocksBuiltDebug++;
                                Debug.Log("World Block created! " + nWorldBlocksBuiltDebug);
                                existingWorldBlockCoordinates[new Vector2Int(i, j)] = worldBlockInst.name;
                                worldBlocksInAnimation.Add(worldBlockInst);
                                break;
                            }
                            tresh += worldBlockSpawnTreshDelta;
                        }
                    }
                } 
            }
        }
    }

    void animateWorldBlocks()
    {
        List<GameObject> updatedWorldBlocksInAnimation = new List<GameObject>();
        foreach (GameObject worldBlock in worldBlocksInAnimation)
        {
            float movementY = worldBlocksAnimationSpeed * Time.deltaTime;
            worldBlock.transform.Translate(0,movementY,0);
            if (worldBlock.transform.position.y < 0.0f)
            {
                updatedWorldBlocksInAnimation.Add(worldBlock);
            } 
        }
        worldBlocksInAnimation = updatedWorldBlocksInAnimation;
    }

    void buildWorldProps(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (Random.value > 0.3) // TODO: control with noise!
                {
                    existingWorldPropsCoordinates[new Vector2Int(i, j)] = "Empty";
                    continue;
                }
                if (!existingWorldPropsCoordinates.ContainsKey(new Vector2Int(i, j)))
                {
                    // World props are created with jitter around world block centers.
                    Vector3 PropCo = new Vector3(
                        worldBlockCenterDist * i + Random.Range(-1.0f, 1.0f), 
                        0, 
                        worldBlockCenterDist * j + Random.Range(-1.0f, 1.0f));
                    Vector3 PlayerToProp = PropCo - PlayerCo;
                    if (PlayerToProp.magnitude < maxDistFromPlayerPropSpawn)
                    {
                        int worldPropIdx = (int)Mathf.Floor(Random.value * nWorldProps);
                        PropCo.y = 5.0f;
                        GameObject worldPropInst = Instantiate(WorldProps[worldPropIdx], PropCo, Quaternion.identity);
                        existingWorldPropsCoordinates[new Vector2Int(i, j)] = worldPropInst.name;
                        worldPropsInAnimation.Add(worldPropInst);  
                        nWorldPropsBuiltDebug++;
                        Debug.Log("World Prop created! " + nWorldPropsBuiltDebug);
                    }
                }
            }
        }
    }

    void animateWorldProps()
    {
        List<GameObject> updatedWorldPropsInAnimation = new List<GameObject>();
        foreach (GameObject worldProp in worldPropsInAnimation)
        {
            float movementY = -worldPropsAnimationSpeed * Time.deltaTime; // Maybe scaling?
            worldProp.transform.Translate(0, movementY, 0);
            if (worldProp.transform.position.y > 1.0f)
            {
                updatedWorldPropsInAnimation.Add(worldProp);
            } 
        }
        worldPropsInAnimation = updatedWorldPropsInAnimation;
    }
}
