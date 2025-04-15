using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;
using VRC.Udon.Common.Exceptions;
using UnityEngine.UIElements;

public class CityChanger : UdonSharpBehaviour
{
    public GameObject fadeObject;
    private MeshRenderer fadeRenderer;

    public GameObject[] sceneObjects; // Array to hold your objects
    public Material[] sceneSkybox;

    [UdonSynced, FieldChangeCallback(nameof(CurrentIndex))]
    private int currentIndex = 0; // Index of the currently active object

    private float swapTime = 5;
    private float swapTimer = 0;

    //private float cdTimer = 0;

    public float skyboxFadeTime = 0.5f;
    public float fogFadeTime = 1;
    private float fogTimer = 0;
    private bool fadeout = true;

    public Material MAT_InteriorWall;
    public Material MAT_WindowFrame;
    public Material MAT_Glass;

    public int CurrentIndex
    {
        set
        {
            currentIndex = value;

            // Set variables for fog fade
            fadeout = true;
            fogTimer = fogFadeTime + skyboxFadeTime;
            //fadeRenderer.gameObject.SetActive(true);
        }

        get { return currentIndex; }
    }


    void Start()
    {
        //fadeRenderer = fadeObject.GetComponent<MeshRenderer>();

        // Ensure all objects are initially off, except the first one
        UpdateObjectStates();
        fogTimer = -(fogFadeTime + skyboxFadeTime);
        RenderSettings.skybox.SetFloat("_Fade", 0);
        RenderSettings.fogDensity = 0;

        //fadeRenderer.materials[0].color = new Color(0, 0, 0, 0);
        //fadeRenderer.gameObject.SetActive(false);

    }

    private void Update()
    {
        //if(cdTimer > 0) cdTimer -= Time.deltaTime;

        // Check if the Q key is pressed
        /*if (Input.GetKeyDown(KeyCode.Q) && cdTimer <= 0 && Networking.IsOwner(gameObject))
        {
            cdTimer = 1;
            ToggleNext();
        }*/

        if (Input.GetKeyDown(KeyCode.O))
        {
            //MAT_WindowFrame.color = new Color(100,100,100,0);

            //MAT_Glass.color = new Color(100, 100, 100, 0);
            //MAT_InteriorWall.color = new Color(100, 100, 100, 0);
            //MAT_InteriorWall.SetColor("_EmissionColor", new Color(100, 100, 100, 0));
            //MAT_WindowFrame.color = new Color(100, 100, 100, 0);
        }

        ChangeSurroundings();
    }

    private void ChangeSurroundings()
    {
        swapTimer += Time.deltaTime;
        if (swapTimer > swapTime && Networking.IsOwner(gameObject))
        {
            swapTimer = 0;
            ToggleNext();
        }

        fogTimer -= Time.deltaTime;
        if (fogTimer > fogFadeTime)
        {
            float interp = Mathf.Clamp(1 - ((fogTimer - fogFadeTime) / skyboxFadeTime), 0, 1);
            RenderSettings.skybox.SetFloat("_Fade", interp);
        }
        else if (fogTimer > -fogFadeTime)
        {
            RenderSettings.skybox.SetFloat("_Fade", 1);
            float interp = Mathf.Clamp(1 - (Mathf.Abs(fogTimer) / fogFadeTime), 0, 1);
            RenderSettings.fogDensity = interp;

            // when we pass 0 swap objects
            if (fogTimer < 0 && fadeout)
            {
                fadeout = false;
                UpdateObjectStates();
            }
        }
        else if (fogTimer > -(fogFadeTime + skyboxFadeTime))
        {
            RenderSettings.fogDensity = 0;
            float interp = Mathf.Clamp(1 - (Mathf.Abs(fogTimer + fogFadeTime) / skyboxFadeTime), 0, 1);
            RenderSettings.skybox.SetFloat("_Fade", interp);
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