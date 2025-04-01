
using System.Collections.Generic;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;


public class SkyboxChanger : UdonSharpBehaviour
{
    [SerializeField]
    //private DataList materials = new DataList();
    private Material[] materials = new Material[0];
    [SerializeField]
    private float swapTime = 30f;

    int currentMat = 0;
    float timer = 0;
    void Start()
    {
        if(materials.Length > 0)
        {
            RenderSettings.skybox = materials[currentMat];
            //DynamicGI.UpdateEnvironment();
        }

    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > swapTime && materials.Length > 0)
        {
            currentMat = (currentMat + 1) % materials.Length;
            RenderSettings.skybox = materials[currentMat];
            //DynamicGI.UpdateEnvironment();
            timer = 0;
        }
    }
}
