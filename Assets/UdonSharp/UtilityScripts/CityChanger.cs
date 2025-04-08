using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;
using VRC.Udon.Common.Exceptions;

public class CityChanger : UdonSharpBehaviour
{
    public GameObject fadeObject;
    private MeshRenderer fadeRenderer;

    public GameObject[] sceneObjects; // Array to hold your objects
    public Material[] sceneSkybox;

    [UdonSynced, FieldChangeCallback(nameof(CurrentIndex))]
    private int currentIndex = 0; // Index of the currently active object

    private float cdTimer = 0;

    public float fogFadeTime = 1;
    private float fogTimer = 0;
    private bool fadeout = true;

    public int CurrentIndex
    {
        set
        {
            currentIndex = value;

            // Set variables for fog fade
            fadeout = true;
            fogTimer = fogFadeTime;
            fadeRenderer.gameObject.SetActive(true);
        }

        get { return currentIndex; }
    }


    void Start()
    {
        fadeRenderer = fadeObject.GetComponent<MeshRenderer>();

        // Ensure all objects are initially off, except the first one
        UpdateObjectStates();
        fogTimer = -fogFadeTime;

        fadeRenderer.materials[0].color = new Color(0, 0, 0, 0);
        fadeRenderer.gameObject.SetActive(false);

    }

    private void Update()
    {
        if(cdTimer > 0) cdTimer -= Time.deltaTime;

        // Check if the Q key is pressed
        if (Input.GetKeyDown(KeyCode.Q) && cdTimer <= 0 && Networking.IsOwner(gameObject))
        {
            cdTimer = 1;
            ToggleNext();
        }

        if(fogTimer > -fogFadeTime)
        {
            fogTimer -= Time.deltaTime;

            float interp = Mathf.Clamp(1 - (Mathf.Abs(fogTimer) / fogFadeTime), 0, 1);
            RenderSettings.fogDensity = interp;

            fadeRenderer.materials[0].color = new Color(0, 0, 0, interp);

            // Handle end of fade
            if(interp == 0 && !fadeout)
            {
                fadeRenderer.gameObject.SetActive(false);
            }

            // when we pass 0 swap objects
            if (fogTimer < 0 && fadeout)
            {
                fadeout = false;
                UpdateObjectStates();
            }
        }

    }

    private void ToggleNext()
    {
        // Increment index and wrap around if necessary
        CurrentIndex = (currentIndex + 1) % sceneObjects.Length;
    }

    private void UpdateObjectStates()
    {

        // Loop through each object and toggle its active state
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            sceneObjects[i].SetActive(i == currentIndex);
        }

        RenderSettings.skybox = sceneSkybox[currentIndex];
    }
}