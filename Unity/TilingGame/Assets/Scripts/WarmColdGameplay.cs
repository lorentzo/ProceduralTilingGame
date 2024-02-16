using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarmColdGameplay : MonoBehaviour
{
    private Image distanceImage;
    public GameObject Player;
    public GameObject objectToFind;
    public Sprite warm;
    public Sprite cold;
    public Sprite neutral;
    private Sprite lastSprite;
    private float oldDist;

    void Awake()
    {
        distanceImage = gameObject.GetComponent<Image>();
        distanceImage.sprite = neutral;
        oldDist = computeDistance();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float newDist = computeDistance();
        if (Mathf.Abs(newDist-oldDist) < 0.005f)
        {
            distanceImage.sprite = lastSprite;
        }
        else if (newDist > oldDist)
        {
            distanceImage.sprite = cold;
            lastSprite = cold;
        }
        else if (newDist < oldDist)
        {
            distanceImage.sprite = warm;
            lastSprite = warm;
        }
        oldDist = newDist;
    }

    float computeDistance()
    {
        Vector3 PlayerCo = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
        Vector3 ObjectTOFindCo = new Vector3(objectToFind.transform.position.x, 0.0f, objectToFind.transform.position.z);
        return Vector3.Distance(PlayerCo, ObjectTOFindCo);
    }

    float computeAngle()
    {
        Vector3 PlayerCo = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
        Vector3 ObjectTOFindCo = new Vector3(objectToFind.transform.position.x, 0.0f, objectToFind.transform.position.z);
        return Vector3.Angle(PlayerCo, ObjectTOFindCo);
    }
}
