
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectMover : UdonSharpBehaviour
{

    public GameObject objectToMove;
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
        set { 
            moveTowardsLocation = value;

            if(!value)
            {
                objectToMove.transform.rotation = originalRotation;
            }

            moveTimer = ObjectMoveTime;
        }
    }

    [UdonSynced]
    private Vector3 originalLocation;
    [UdonSynced]
    private Quaternion originalRotation;
    [UdonSynced]
    private Vector3 originalScale;

    private Vector3 moveVector;

    void Start()
    {
        if(Networking.IsOwner(gameObject) && objectToMove && moveTimer <= 0)
        {
            originalLocation = objectToMove.transform.position;
            originalRotation = objectToMove.transform.rotation;
            originalScale = objectToMove.transform.localScale;
        }

        moveVector = (originalLocation - gameObject.transform.position);
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

        if (Input.GetKeyDown(KeyCode.O) && Networking.IsOwner(gameObject))
        {
            MoveToLocation = !MoveToLocation;
        }


        if(MoveToLocation)
        {
            if(moveTimer > 0)
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
        }

    }
}