using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform playerTransform;  // Drag your player's transform here in the inspector
    private Vector3 offset;  // Distance from the player

    private void Start()
    {
        offset = transform.position - playerTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(playerTransform.position.x + offset.x, transform.position.y, playerTransform.position.z + offset.z);
        transform.position = newPosition;
    }
}
