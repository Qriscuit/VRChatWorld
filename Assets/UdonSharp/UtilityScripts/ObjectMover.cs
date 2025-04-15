using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectMover : UdonSharpBehaviour
{
    public GameObject object1;
    public GameObject object2;
    private GameObject objectToMove;
    [UdonSynced, FieldChangeCallback(nameof(SwapObjects))]
    private bool obj = true;
    public bool SwapObjects
    {
        get { return obj; }
        set
        {
            obj = value;
            if (value)
            {
                objectToMove = object1;
                ObjectFinalScale = 50;

            }
            else
            {
                objectToMove = object2;
                ObjectFinalScale = 5;
            }

            ResetVars();
        }
    }

    public void ResetVars()
    {
        originalLocation = objectToMove.transform.position;
        originalRotation = objectToMove.transform.rotation;
        originalScale = objectToMove.transform.localScale;

        moveVector = (originalLocation - gameObject.transform.position);
    }

    [SerializeField]
    private float ObjectFinalScale = 1f;
    [SerializeField]
    private float ObjectMoveTime = 5f;
    [SerializeField]
    [Tooltip("In Rotations Per Second")]
    private float ObjectRotateSpeed = 0.2f;

    [UdonSynced, FieldChangeCallback(nameof(MoveToLocation))]
    private bool moveTowardsLocation;
    private float moveTimer = 0;
    public bool MoveToLocation
    {
        get { return moveTowardsLocation; }
        set
        {
            moveTowardsLocation = value;

            if (!value)
            {
                objectToMove.transform.rotation = originalRotation;


            }

            moveTimer = ObjectMoveTime;
        }
    }

    [SerializeField]
    private float fadeTime = 3f;
    private float fadeTimer = 0;
    [UdonSynced, FieldChangeCallback(nameof(FadeOut))]
    private bool fadeOut = false;
    public bool FadeOut
    {
        get { return fadeOut; }
        set
        {
            fadeOut = value;
            fadeTimer = fadeTime;
            if(value)
            {
                SetMaterialsTo(1);
            }
            else
            {
                foreach (MeshRenderer renderer in WallsMeshRenderers)
                {
                    renderer.gameObject.SetActive(true);
                }

                ShelfMeshRenderers.gameObject.SetActive(true);
            }
        }
    }

    private Vector3 originalLocation;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private Vector3 moveVector;

    [SerializeField] public MeshRenderer[] WallsMeshRenderers;
    [SerializeField] private MeshRenderer ShelfMeshRenderers;

    [SerializeField] private Material MAT_OPAQUE_Plaster;
    [SerializeField] private Material MAT_OPAQUE_Shelf;
    [SerializeField] private Material MAT_TRANSPARENT_Plaster;
    [SerializeField] private Material MAT_TRANSPARENT_Shelf;
    private Color basePlaster;
    private Color baseShelf;

    [SerializeField] private GameObject Ceiling;
 
    [SerializeField] private GameObject WallsAndCeiling;

    public GameObject dirLightObj;
    private Light dirLight;
    private float origIntensity;

    void Start()
    {
        objectToMove = object1;
        ResetVars();


        basePlaster = MAT_OPAQUE_Plaster.GetColor("_Color");
        baseShelf = MAT_OPAQUE_Shelf.GetColor("_Color");

        SetMaterialsTo(0);

        dirLight = dirLightObj.GetComponent<Light>();
        origIntensity = dirLight.intensity;
    }

    // 0 - Opaque || 1 - Transparent
    void SetMaterialsTo(int surfaceType)
    {
        for (int i = 0; i < WallsMeshRenderers.Length; i++)
        {
            MeshRenderer renderer = WallsMeshRenderers[i];

            if(surfaceType == 0)
            {
                renderer.material = MAT_OPAQUE_Plaster; 
            }
            else
            {
                renderer.material = MAT_TRANSPARENT_Plaster;
            }
        }

        ShelfMeshRenderers.material = (surfaceType == 0) ? MAT_OPAQUE_Shelf : MAT_TRANSPARENT_Shelf;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.I) && Networking.IsOwner(gameObject) && fadeTimer <= 0 && moveTimer <= 0 && !moveTowardsLocation)
        {
            SwapObjects = !SwapObjects;
            ResetVars();
        }

        if (Input.GetKeyDown(KeyCode.P) && Networking.IsOwner(gameObject) && fadeTimer <= 0 && moveTimer <= 0)
        {
            FadeOut = !FadeOut;
        }

        if (FadeOut)
        {
            if (fadeTimer > 0)
            {
                fadeTimer -= Time.deltaTime;
                float interpTime = 1 - (fadeTimer / fadeTime);

                foreach(MeshRenderer renderer in WallsMeshRenderers)
                {
                    renderer.material.color = Color.Lerp(basePlaster, new Color(0, 0, 0, 0), interpTime);
                }

                ShelfMeshRenderers.material.color = Color.Lerp(baseShelf, new Color(0, 0, 0, 0), interpTime);

                dirLight.intensity = 0;
            }
            else
            {
                foreach (MeshRenderer renderer in WallsMeshRenderers)
                {
                    renderer.gameObject.SetActive(false);
                }

                ShelfMeshRenderers.gameObject.SetActive(false);
            }
        }
        else
        {
            if (fadeTimer > 0)
            {
                fadeTimer -= Time.deltaTime;
                float interpTime = (fadeTimer / fadeTime);

                foreach (MeshRenderer renderer in WallsMeshRenderers)
                {
                    renderer.material.color = Color.Lerp(basePlaster, new Color(0, 0, 0, 0), interpTime);
                }

                ShelfMeshRenderers.material.color = Color.Lerp(baseShelf, new Color(0, 0, 0, 0), interpTime);
            }
            else
            {
                SetMaterialsTo(0);
                dirLight.intensity = origIntensity;
            }
        }


        if (Input.GetKeyDown(KeyCode.O) && Networking.IsOwner(gameObject) && fadeOut && fadeTimer <= 0 && moveTimer <= 0)
        {
            MoveToLocation = !MoveToLocation;
        }

        if (MoveToLocation)
        {
            if (moveTimer > 0)
            {
                moveTimer -= Time.deltaTime;
                float interpValue = (moveTimer / ObjectMoveTime);
                interpValue = 1 / (1 + (Mathf.Exp(-12 * (interpValue - 0.5f))));

                objectToMove.transform.position = gameObject.transform.position + interpValue * moveVector;
                objectToMove.transform.localScale = originalScale * interpValue + new Vector3(ObjectFinalScale, ObjectFinalScale, ObjectFinalScale) * (1 - interpValue);

            }
            else
            {
                objectToMove.transform.rotation = Quaternion.Euler(0, 360 * ObjectRotateSpeed * Time.deltaTime, 0) * objectToMove.transform.rotation;
            }
        }
        else
        {
            if (moveTimer > 0)
            {
                moveTimer -= Time.deltaTime;
                float interpValue = (moveTimer / ObjectMoveTime);
                interpValue = 1 / (1 + (Mathf.Exp(-12 * (interpValue - 0.5f))));
                objectToMove.transform.position = gameObject.transform.position + (1 - interpValue) * moveVector;
                objectToMove.transform.localScale = originalScale * (1 - interpValue) + new Vector3(ObjectFinalScale, ObjectFinalScale, ObjectFinalScale) * interpValue;
            }
            else
            {
                
            }
        }
    }
}
