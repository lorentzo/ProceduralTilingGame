using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarmColdGameplay : MonoBehaviour
{
    private TextMeshProUGUI distanceText;
    public GameObject Player;
    public GameObject objectToFind;

    // Start is called before the first frame update
    void Start()
    {
        distanceText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerCo = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
        Vector3 ObjectTOFindCo = new Vector3(objectToFind.transform.position.x, 0.0f, objectToFind.transform.position.z);
        distanceText.text = "DISTANCE: " + Vector3.Distance(PlayerCo, ObjectTOFindCo);
    }
}
