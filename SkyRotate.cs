using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateSky();
    }
    void RotateSky()
    {
        float num1 = Camera.main.GetComponent<Skybox>().material.GetFloat("_Rotation");
        Camera.main.GetComponent<Skybox>().material.SetFloat("_Rotation",num1+0.009f);
        float num2 = Camera.main.GetComponent<Skybox>().material.GetFloat("_Exposure");
        if(num2>0.1f)
        {
            Camera.main.GetComponent<Skybox>().material.SetFloat("_Exposure", num2 - 0.001f);
        }
    }
}
