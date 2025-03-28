using UnityEngine;

public class LoopYAxis : MonoBehaviour
{
    public float speed = 2f; // Speed of movement
    public float range = 3f; // Range for movement

    private float originalY;

    private void Start()
    {
        speed = Random.Range(0.5f, 1.5f);
        originalY = transform.position.y; // Store the initial Y position
    }

    private void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, 2 * range) - range; // Calculate offset
        transform.position = new Vector3(transform.position.x, originalY + offset, transform.position.z);
    }
}
