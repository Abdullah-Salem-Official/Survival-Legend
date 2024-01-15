using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 0.8f;
    public float smoothSpeed = 0.125f;
    public float xOffset = 0f;
    public float yOffset = 0f;

    private Vector3 desiredPosition;
    private Vector3 smoothVelocity;

    private void LateUpdate()
    {
        if (player != null)
        {
            desiredPosition = player.position + new Vector3(xOffset, yOffset, 0f);
            desiredPosition.z = transform.position.z;

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, smoothSpeed);
            transform.position = smoothedPosition;

            // Move camera with arrow keys or WASD keys
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0f || verticalInput != 0f)
            {
                Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}