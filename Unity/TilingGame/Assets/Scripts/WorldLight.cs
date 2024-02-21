using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class WorldLight : MonoBehaviour
{
    private Light directionalLight;
    public float minLightIntensity = 0.0f;
    public float maxLightIntensity = 1.0f;
    public float deltaLightIntensity = 0.1f;
    public float xLightRotation = 30.0f;
    public float minYLightRotation = -90.0f;
    public float maxYLightRotation = 90.0f;
    public float deltaYLightRotation = 3.0f;
    private Vector3 lightRotation;
    private bool startDecreasingIntensity = false;
    private bool waitForLightCycle = false;
    private bool restartLightCycle = false;

    private float timer = 0;
    private float timerMax = 0;

    // Start is called before the first frame update
    void Start()
    {
        directionalLight = GetComponent<Light>();
        resetLight();

        waitForLightCycle = false;
        restartLightCycle = false;
    }

    // Update is called once per frame
    void Update()
    {   
        //Debug.Log("Before light cycle: " + waitForLightCycle + " " + restartLightCycle);
        if (!waitForLightCycle && !restartLightCycle)
        {
            Debug.Log("Light Cycle!");
            lightCycle();
        }
        
        //Debug.Log("Before waiting: " + waitForLightCycle + " " + restartLightCycle);
        if (waitForLightCycle && !restartLightCycle)
        {
            if (!Waited(5))
            {
                Debug.Log("Waiting!");
                return;
            }
            else
            {
                restartLightCycle = true;
                waitForLightCycle = false;
                timer = 0;
            }
        }
        
        //Debug.Log("Before restart: " + waitForLightCycle + " " + restartLightCycle);
        if (restartLightCycle && !waitForLightCycle)
        {
            resetLight();
            Debug.Log("Light cycle restarted!");
        }
    }

    void resetLight()
    {
        restartLightCycle = false;
        waitForLightCycle = false;
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
            
            if (lightRotation.y < maxYLightRotation / 4.0f)
            {
                if (directionalLight.intensity < maxLightIntensity)
                {
                    directionalLight.intensity += deltaLightIntensity;// * Time.deltaTime;
                }
            }
            else
            {
                if (directionalLight.intensity > minLightIntensity)
                {
                    directionalLight.intensity -= deltaLightIntensity*2.0f;// * Time.deltaTime;
                }
            }
            
        }
        else
        {
            waitForLightCycle = true;
        }

        /*
        if (!startDecreasingIntensity && directionalLight.intensity < maxLightIntensity)
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

        if (Mathf.Abs(directionalLight.intensity - minLightIntensity) <= 1e-3)
        {
            waitForLightCycle = true;
        }
        */
    }

    private bool Waited(float seconds)
    {
        // https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
        timerMax = seconds;
    
        timer += Time.deltaTime;
    
        if (timer >= timerMax)
        {
            return true; //max reached - waited x - seconds
        }
    
        return false;
    }
}
