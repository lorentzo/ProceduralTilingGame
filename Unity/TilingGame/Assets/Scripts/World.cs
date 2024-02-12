using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject Player;

    public int worldSize = 30;
    public GameObject worldEndBlock;
    Dictionary<Vector2Int, string> existingWorldEndBlockCoordinates = new Dictionary<Vector2Int, string>();
    private int nWorldEndBlocksDebug;

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
    private float worldBlockStartingYPosition = -5.0f;
    private float worldBlockEndingYPosition = 0.0f;

    public GameObject[] WorldProps;
    private int nWorldProps;
    Dictionary<Vector2Int, string> existingWorldPropsCoordinates = new Dictionary<Vector2Int, string>();
    private float maxDistFromPlayerPropSpawn;
    private List<GameObject> worldPropsInAnimation = new List<GameObject>();
    private int nWorldPropsBuiltDebug = 0;
    private float worldPropsAnimationSpeed = 7.0f;
    private float worldPropStartingYPosition = 5.0f;
    private float worldPropEndingYPosition = 1.0f;

    public GameObject[] WorldPopups;
    private int nWorldPopups;
    Dictionary<Vector2Int, string> existingWorldPopupsCoordinates = new Dictionary<Vector2Int, string>();
    private float maxDistFromPlayerPopupSpawn;
    private List<GameObject> worldPopupsInAnimation = new List<GameObject>();
    private int nWorldPopupsBuiltDebug = 0;
    private float worldPopupsAnimationSpeed = 3.0f;
    Vector3 worldPopupsScaleChange;
    private float worldPopupStartingScale = 0.0f;
    private float worldPopupEndingScale = 1.0f;

    public GameObject objectToFind;

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
    
        // Randomly place object to find.
        float coX = Random.Range(-worldSize + 1, worldSize - 1);
        float coY = Random.Range(-worldSize + 1, worldSize - 1);
        objectToFind.transform.position = new Vector3(coX, 2.0f, coY);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerCo = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
        Vector3 ObjectTOFindCo = new Vector3(objectToFind.transform.position.x, 0.0f, objectToFind.transform.position.z);
        buildWorldEnd(PlayerCo);
        buildWorldBlocks(PlayerCo);
        animateWorldBlocks();
        buildWorldProps(PlayerCo);
        animateWorldProps();
        buildWorldPopups(PlayerCo);
        animateWorldPopups();
        Debug.Log("Player to Object To Find: " + Vector3.Distance(PlayerCo, ObjectTOFindCo));
    }

    void buildWorldEnd(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (Mathf.Abs(i) >= worldSize || Mathf.Abs(j) >= worldSize)
                {
                    if(!existingWorldEndBlockCoordinates.ContainsKey(new Vector2Int(i, j)))
                    {
                        Vector3 WorldEndBlockCo = new Vector3(worldBlockCenterDist*i, 0, worldBlockCenterDist*j);
                        GameObject worldEndBlockInst = Instantiate(worldEndBlock, WorldEndBlockCo, Quaternion.identity);
                        nWorldEndBlocksDebug++;
                        Debug.Log("World End Block Created! " + nWorldEndBlocksDebug);
                        existingWorldEndBlockCoordinates[new Vector2Int(i, j)] = worldEndBlockInst.name;                
                    }
                }
            }
        }
    }

    void buildWorldBlocks(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (Mathf.Abs(i) >= worldSize || Mathf.Abs(j) >= worldSize)
                {
                    continue;
                }
                if (!existingWorldBlockCoordinates.ContainsKey(new Vector2Int(i, j)))
                {
                    Vector3 WorldBlockCo = new Vector3(worldBlockCenterDist*i, 0, worldBlockCenterDist*j);
                    Vector3 PlayerToWorldBlock = WorldBlockCo - PlayerCo;
                    if (PlayerToWorldBlock.magnitude < maxDistFromPlayerWorldBlockSpawn)
                    {
                        float noise = Mathf.PerlinNoise(
                            (WorldBlockCo.x+0.5f)*worldBlockVariation+worldBlockJitter, 
                            (WorldBlockCo.z+0.5f)*worldBlockVariation+worldBlockJitter);
                        float tresh = worldBlockSpawnTreshDelta;
                        for (int cIdx = 0; cIdx < nWorldBlocks; cIdx++)
                        {
                            Debug.Log("Noise: " + noise + " tresh: " + tresh + " treshDelta: " + worldBlockSpawnTreshDelta);
                            if (noise > tresh - worldBlockSpawnTreshDelta && noise < tresh)
                            {
                                WorldBlockCo.y = worldBlockStartingYPosition;
                                GameObject worldBlockInst = Instantiate(WorldBlocks[cIdx], WorldBlockCo, Quaternion.identity);
                                //Renderer instRenderer = worldBlockInst.GetComponent<Renderer>();
                                //instRenderer.material.SetColor("_Color", Color.red);
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
            // Animated by rising from below.
            float movementY = worldBlocksAnimationSpeed * Time.deltaTime;
            worldBlock.transform.Translate(0, movementY, 0);
            if (worldBlock.transform.position.y < worldBlockEndingYPosition)
            {
                // Still needs to be animated.
                updatedWorldBlocksInAnimation.Add(worldBlock);
            }
            else
            {
                // It is where it should be, fix position.
                Vector3 worldBlockPosition = worldBlock.transform.position;
                worldBlockPosition.y = worldBlockEndingYPosition + Random.value * 0.2f;
                worldBlock.transform.position = worldBlockPosition;
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
                if (Mathf.Abs(i) >= worldSize || Mathf.Abs(j) >= worldSize)
                {
                    continue;
                }
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
                        PropCo.y = worldPropStartingYPosition;
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
            // Animated by falling from sky.
            float movementY = -worldPropsAnimationSpeed * Time.deltaTime;
            worldProp.transform.Translate(0, movementY, 0);
            if (worldProp.transform.position.y > worldPropEndingYPosition)
            {
                // It still needs to be animated.
                updatedWorldPropsInAnimation.Add(worldProp);
            }
            else
            {
                // It is where it should be, fix position.
                Vector3 worldPropPosition = worldProp.transform.position;
                worldPropPosition.y = worldPropEndingYPosition + Random.value * 0.2f; // a bit of jitter
                worldProp.transform.position = worldPropPosition;
            }
        }
        worldPropsInAnimation = updatedWorldPropsInAnimation;
    }

    void buildWorldPopups(Vector3 PlayerCo)
    {
        int k = (int)Mathf.Floor(PlayerCo.x);
        int l = (int)Mathf.Floor(PlayerCo.z);
        for (int i = k - nWorldBlocksAroundPlayerTest; i < k + nWorldBlocksAroundPlayerTest; i++)
        {
            for (int j = l - nWorldBlocksAroundPlayerTest; j < l + nWorldBlocksAroundPlayerTest; j++)
            {
                if (Mathf.Abs(i) >= worldSize || Mathf.Abs(j) >= worldSize)
                {
                    continue;
                }
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
                                PopupCo.y = worldBlockEndingYPosition + 1.0f;  // it depends on the size of world block
                                GameObject worldPopupInst = Instantiate(WorldPopups[worldPopupIdx], PopupCo, Quaternion.identity);
                                worldPopupInst.transform.localScale = new Vector3(worldPopupStartingScale,worldPopupStartingScale,worldPopupStartingScale);
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
            if (popup.transform.localScale.x < worldPopupEndingScale)
            {
                // Still needs to be animated
                updatedWorldPopupsInAnimation.Add(popup);
            }
            else
            {
                float popUpScale = worldPopupEndingScale + Random.value * 0.2f;
                popup.transform.localScale = new Vector3(popUpScale,popUpScale,popUpScale);
            }
        }
        worldPopupsInAnimation = updatedWorldPopupsInAnimation;
    }
}
