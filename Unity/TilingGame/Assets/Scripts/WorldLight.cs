using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class WorldLight : MonoBehaviour
{
    private Light directionalLight;
    public float minLightIntensity = 0.0f;
    public float maxLightIntenstiy = 1.0f;
    public float deltaLightIntensity = 0.01f;
    public float xLightRotation = 30.0f;
    public float minYLightRotation = -90.0f;
    public float maxYLightRotation = 90.0f;
    public float deltaYLightRotation = 3.0f;
    private Vector3 lightRotation;
    private bool startDecreasingIntensity = false;

    private float timer = 0;
    private float timerMax = 0;

    // Start is called before the first frame update
    void Start()
    {
        directionalLight = GetComponent<Light>();
        resetLight();
    }

    // Update is called once per frame
    void Update()
    {   
        lightCycle();
        if (!Waited(5))
        {
            return;
        }
        resetLight();
    }

    void resetLight()
    {
        lightRotation = new Vector3(xLightRotation, minYLightRotation, 0.0f);
        transform.localEulerAngles = lightRotation;
        
        directionalLight.intensity = minLightIntensity;
        startDecreasingIntensity = false;
    }

    void lightCycle()
    {
        if (lightRotation.y < maxYLightRotation)
        {
            lightRotation.y += deltaYLightRotation * Time.deltaTime;
            transform.localEulerAngles = lightRotation;
        }

        if (!startDecreasingIntensity && directionalLight.intensity < maxLightIntenstiy)
        {
            directionalLight.intensity += deltaLightIntensity * Time.deltaTime;
        }
        else
        {
            startDecreasingIntensity = true;
        }

        if(startDecreasingIntensity)
        {
            directionalLight.intensity -= deltaLightIntensity * Time.deltaTime;
        }
    }

    private bool Waited(float seconds)
    {
        timerMax = seconds;
    
        timer += Time.deltaTime;
    
        if (timer >= timerMax)
        {
            return true; //max reached - waited x - seconds
        }
    
        return false;
    }
}
