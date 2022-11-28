using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Script for the camera to follow the player
    [SerializeField] private Transform player;

    [SerializeField] private float cameraSpeed = 0.1f;

    private bool cameraLock = true;
    public static CameraController Instance;
    
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float rotationY;
    
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void FixedUpdate()
    {
            UpdateCamera(Time.fixedTime);
    }

    private void UpdateCamera(float deltaTime)
    {
        //if the player is not null
        if (!player) return;
        Vector3 nextPos;

        //if the camera is locked the camera follows the player
        if (cameraLock)
        {
            nextPos = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, nextPos, deltaTime * lerpSpeed);
        }
        else
        {
            nextPos = transform.position;

            if (Input.mousePosition.x >= Screen.width - 1)
            {
                nextPos += transform.right * cameraSpeed;
            }

            if (Input.mousePosition.x <= 0)
            {
                nextPos -= transform.right * cameraSpeed;
            }

            if (Input.mousePosition.y >= Screen.height - 1)
            {
                nextPos += transform.forward * cameraSpeed;
            }

            if (Input.mousePosition.y <= 0)
            {
                nextPos -= transform.forward * cameraSpeed;
            }

            transform.position = Vector3.Lerp(transform.position, nextPos, deltaTime* lerpSpeed);
        }

        transform.rotation = Quaternion.Euler(transform.rotation.x, rotationY, transform.rotation.z);
    }

    public void LinkCamera(Transform target)
    {
        player = target;
        //InputManager.PlayerMap.Camera.LockToggle.performed += OnToggleCameraLock;
    }

    public void UnLinkCamera()
    {
        player = null;
        //InputManager.PlayerMap.Camera.LockToggle.performed -= OnToggleCameraLock;
    }
}
