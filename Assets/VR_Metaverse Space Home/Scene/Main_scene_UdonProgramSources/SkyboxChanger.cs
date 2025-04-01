
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
    private Material[] materials = new Material[0];
    [SerializeField]
    private float swapTime = 30f;

    [UdonSynced, FieldChangeCallback(nameof(CurrentMaterial))]
    private int currentMat = 0;


    private float timer = 0;
    void Start()
    {

        if(materials.Length > 0)
        {
            RenderSettings.skybox = materials[currentMat];  
            //DynamicGI.UpdateEnvironment();
        }

    }

    public int CurrentMaterial
    {
        set
        {
            currentMat = value;
            RenderSettings.skybox = materials[currentMat];
        }
        get { return currentMat; }
    }

    private void FixedUpdate()
    {
        if (Networking.IsOwner(gameObject))
        {
            timer += Time.fixedDeltaTime;
            if (timer > swapTime && materials.Length > 0)
            {
                CurrentMaterial = (CurrentMaterial + 1) % materials.Length;
                //DynamicGI.UpdateEnvironment();
                timer = 0;
            }
        }
    }
}
