using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    public GameObject Player;

    public int worldSize = 30;
    public GameObject worldEndBlock;
    Dictionary<Vector2Int, GameObject> existingWorldEndBlockCoordinates = new Dictionary<Vector2Int, GameObject>();
    private int nWorldEndBlocksDebug;

    public GameObject[] lights;
    Dictionary<Vector2Int, GameObject> existingLightCoordinates = new Dictionary<Vector2Int, GameObject>();
    private int nCreatedLightsDebug = 0;

    public GameObject[] WorldBlocks;
    private int nWorldBlocks;
    float worldBlockCenterDist;
    public int nWorldBlocksAroundPlayerTest = 3;
    public float maxDistFromPlayerWorldBlockSpawn = 4.0f;
    public float worldBlockVariation = 0.2f;
    private float worldBlockJitter;
    private Dictionary<Vector2Int, GameObject> existingWorldBlockCoordinates = new Dictionary<Vector2Int, GameObject>();
    private float worldBlockSpawnTreshDelta;
    private int nWorldBlocksBuiltDebug = 0;
    private List<GameObject> worldBlocksInAnimation = new List<GameObject>();
    private float worldBlocksAnimationSpeed = 20.0f;
    private float worldBlockStartingYPosition = -5.0f;
    private float worldBlockEndingYPosition = 0.0f;

    public GameObject[] WorldProps;
    private int nWorldProps;
    Dictionary<Vector2Int, GameObject> existingWorldPropsCoordinates = new Dictionary<Vector2Int, GameObject>();
    private float maxDistFromPlayerPropSpawn;
    private List<(GameObject, float)> worldPropsInAnimation = new List<(GameObject, float)>();
    private int nWorldPropsBuiltDebug = 0;
    private float worldPropsAnimationSpeed = 25.0f;
    private float worldPropStartingYPosition = 5.0f;

    public GameObject[] WorldPopups;
    private int nWorldPopups;
    Dictionary<Vector2Int, GameObject> existingWorldPopupsCoordinates = new Dictionary<Vector2Int, GameObject>();
    private float maxDistFromPlayerPopupSpawn;
    private List<GameObject> worldPopupsInAnimation = new List<GameObject>();
    private int nWorldPopupsBuiltDebug = 0;
    private float worldPopupsAnimationSpeed = 3.0f;
    Vector3 worldPopupsScaleChange;
    private float worldPopupStartingScale = 0.0f;
    private float worldPopupEndingScale = 0.3f;

    public GameObject objectToFind;

    // Start is called before the first frame update
    void Start()
    {
        worldBlockJitter = Random.value * 50.0f;

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
        createLightsOnProps(PlayerCo);
        animateWorldProps();
        buildWorldPopups(PlayerCo);
        animateWorldPopups();
        //animateLights();
        if (Vector3.Distance(PlayerCo, ObjectTOFindCo) < 1.0f)
        {
            resetWorld();
        }
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    void resetWorld()
    {
        SceneManager.LoadScene("MainScene");
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
                        existingWorldEndBlockCoordinates[new Vector2Int(i, j)] = worldEndBlockInst;                
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
                        Debug.Log(noise);
                        float tresh = worldBlockSpawnTreshDelta;
                        for (int cIdx = 0; cIdx < nWorldBlocks; cIdx++)
                        {
                            if (noise > tresh - worldBlockSpawnTreshDelta && noise < tresh)
                            {
                                WorldBlockCo.y = worldBlockStartingYPosition;
                                GameObject worldBlockInst = Instantiate(WorldBlocks[cIdx], WorldBlockCo, Quaternion.identity);
                                // Vary world block Y scale.
                                //Vector3 newScale = worldBlockInst.transform.localScale;
                                //newScale.y = Random.value * 2.0f + 0.5f;
                                //worldBlockInst.transform.localScale = newScale;
                                nWorldBlocksBuiltDebug++;
                                Debug.Log("World Block created! (" + nWorldBlocksBuiltDebug +")");
                                existingWorldBlockCoordinates[new Vector2Int(i, j)] = worldBlockInst;
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
                // It is where it should be, fix position with a bit of variation.
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
                Vector2Int currCoordinates = new Vector2Int(i, j);
                if (Mathf.Abs(i) >= worldSize || Mathf.Abs(j) >= worldSize)
                {
                    continue;
                }
                if (Random.value > 0.2) // TODO: control with noise!
                {
                    existingWorldPropsCoordinates[currCoordinates] = null;
                    continue;
                }
                if (!existingWorldPropsCoordinates.ContainsKey(currCoordinates))
                {                    
                    // World props are created with jitter around world block centers.
                    Vector3 PropCo = new Vector3(
                        worldBlockCenterDist * i + Random.Range(-1.0f, 1.0f), 
                        0, 
                        worldBlockCenterDist * j + Random.Range(-1.0f, 1.0f));
                    //PropCo = new Vector3(worldBlockCenterDist*i, 0, worldBlockCenterDist*j);
                    Vector3 PlayerToProp = PropCo - PlayerCo;
                    if (PlayerToProp.magnitude < maxDistFromPlayerPropSpawn)
                    {
                        // Select random world prop.
                        int worldPropIdx = (int)Mathf.Floor(Random.value * nWorldProps);

                        // Make sure that y coordinate is properly placed above world block.
                        Vector3 currPropSize = WorldProps[worldPropIdx].GetComponent<Collider>().bounds.size;
                        Vector3 currPropCenter = WorldProps[worldPropIdx].GetComponent<Collider>().bounds.center;
                        float worldBlockY = findYCoordinateOfWorldBlock(currCoordinates);
                        float worldPropEndingYPosition = worldBlockY / 2.0f;// + currPropSize.y / 2.0f + currPropCenter.y;
                        Debug.Log(currPropSize.y + " " + worldBlockY + " " + worldPropEndingYPosition);
                        PropCo.y = worldPropStartingYPosition;

                        // Create instance.
                        GameObject worldPropInst = Instantiate(WorldProps[worldPropIdx], PropCo, Quaternion.identity);
                        existingWorldPropsCoordinates[currCoordinates] = worldPropInst;
                        worldPropsInAnimation.Add((worldPropInst, worldPropEndingYPosition));  
                        worldPropInst.transform.Rotate(new Vector3(0.0f, Random.value * 360.0f, 0.0f));
                        nWorldPropsBuiltDebug++;
                        Debug.Log("World Prop created! (" + nWorldPropsBuiltDebug + ")");
                    }
                }
            }
        }
    }

    float findYCoordinateOfWorldBlock(Vector2Int xz)
    {
        GameObject currWorldBlock;
        existingWorldBlockCoordinates.TryGetValue(xz, out currWorldBlock);
        Vector3 currWorldBlockSize = currWorldBlock.GetComponent<Collider>().bounds.size;
        return currWorldBlockSize.y;
    }

    void animateWorldProps()
    {
        List<(GameObject, float)> updatedWorldPropsInAnimation = new List<(GameObject, float)>();

        foreach ((GameObject, float) worldProp in worldPropsInAnimation)
        {
            // Animated by falling from sky.
            float movementY = -worldPropsAnimationSpeed * Time.deltaTime;
            worldProp.Item1.transform.Translate(0, movementY, 0);
            if (worldProp.Item1.transform.position.y > worldProp.Item2)
            {
                // It still needs to be animated.
                updatedWorldPropsInAnimation.Add((worldProp.Item1,worldProp.Item2));
            }
            else
            {
                // It is where it should be, fix position.
                Vector3 worldPropPosition = worldProp.Item1.transform.position;
                worldPropPosition.y = worldProp.Item2 + Random.value * 0.2f; // a bit of jitter
                worldProp.Item1.transform.position = worldPropPosition;
            }
        }
        worldPropsInAnimation = updatedWorldPropsInAnimation;
    }

    void createLightsOnProps(Vector3 PlayerCo)
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
                Vector2Int currCoords = new Vector2Int(i, j);
                // Light create only if prop exist and it is not empty.
                bool createLight = false;
                if (existingWorldPropsCoordinates.ContainsKey(currCoords))
                {
                    createLight = true;
                    GameObject existingWorldPropsCoordinatesValue;
                    existingWorldPropsCoordinates.TryGetValue(currCoords, out existingWorldPropsCoordinatesValue);
                    if (existingWorldPropsCoordinatesValue == null)
                    {
                        createLight = false;
                    }
                }

                if (createLight)
                {
                    if (Random.value > 0.1)
                    {
                        createLight = false;
                        existingLightCoordinates[currCoords] = null;
                    }
                }

                if(createLight)
                {
                    // Lights are created at world props.
                    if(!existingLightCoordinates.ContainsKey(currCoords))
                    {
                        int lightIdx = (int)Mathf.Floor(Random.value * lights.Length);
                        float worldBlockY = findYCoordinateOfWorldBlock(currCoords);
                        GameObject lightInst = Instantiate(lights[lightIdx], new Vector3(i, worldBlockY/2.0f + 1.5f, j), Quaternion.identity);
                        nCreatedLightsDebug++;
                        Debug.Log("Light created: " + nCreatedLightsDebug);
                        existingLightCoordinates[currCoords] = lightInst;
                    }
                }                
            }
        }   
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
                Vector2Int currCoord = new Vector2Int(i, j);
                // Popup only if prop exist and it is not empty.
                bool createPopup = false;
                if (existingWorldPropsCoordinates.ContainsKey(currCoord))
                {
                    createPopup = true;
                    GameObject existingWorldPropsCoordinatesValue;
                    existingWorldPropsCoordinates.TryGetValue(currCoord, out existingWorldPropsCoordinatesValue);
                    if (existingWorldPropsCoordinatesValue == null)
                    {
                        createPopup = false;
                    }
                }
                if (createPopup)
                {
                    if (!existingWorldPopupsCoordinates.ContainsKey(currCoord))
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
                                
                                // Compute Y position.
                                float worldBlockY = findYCoordinateOfWorldBlock(currCoord);
                                Vector3 currPopUpSize = WorldPopups[worldPopupIdx].GetComponent<Collider>().bounds.size;
                                PopupCo.y = worldBlockY / 2.0f + currPopUpSize.y / 2.0f;  // it depends on the size of world block

                                GameObject worldPopupInst = Instantiate(WorldPopups[worldPopupIdx], PopupCo, Quaternion.identity);
                                worldPopupInst.transform.localScale = new Vector3(worldPopupStartingScale,worldPopupStartingScale,worldPopupStartingScale);
                                existingWorldPopupsCoordinates[currCoord] = worldPopupInst;
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

    void animateLights()
    {
        foreach(KeyValuePair<Vector2Int, GameObject> entry in existingLightCoordinates)
        {
            if (entry.Value != null)
            {
                float noise = Mathf.PerlinNoise(
                            (entry.Value.transform.position.x+0.5f), 
                            (entry.Value.transform.position.z+0.5f)) - 1.0f;
                Vector3 translateVector = new Vector3(noise, 0.0f, noise);
                entry.Value.transform.Translate(translateVector * Time.deltaTime, Space.World);
            }
        }
    }
}
