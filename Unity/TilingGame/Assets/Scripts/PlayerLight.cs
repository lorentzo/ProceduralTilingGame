using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerLight : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        setToPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        setToPlayerPosition();
    }

    void setToPlayerPosition()
    {
        Vector3 lightPosition = player.transform.position;
        lightPosition.y += 1.0f;
        transform.position = lightPosition;
    }
}
