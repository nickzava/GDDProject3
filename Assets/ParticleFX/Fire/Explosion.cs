using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Light mLight;
    float time;
    float startIntensity;
    // Start is called before the first frame update
    void Start()
    {
        mLight = gameObject.GetComponent<Light>();
        time = 0;
        startIntensity = mLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 10)
            Destroy(gameObject);
        mLight.intensity = Mathf.Lerp(startIntensity, 0, time);
    }
}
