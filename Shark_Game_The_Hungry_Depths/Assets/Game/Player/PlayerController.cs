using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Used to move camera
    public Camera cam;
    public GameObject cameraRotator;
    public Transform centerOfWorld;
    public Rigidbody rb;
    private float yVelocity = 0f;

    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public SharkSO sharkScriptableObject;

    private float camStartingXAngle;

    private Vector3 playerDiedPos;

    private void Awake()
    {
        SignalBus.OnPlayerDeathRevive += OnPlayerRevive;
    }

    private void OnDestroy()
    {
        SignalBus.OnPlayerDeathRevive -= OnPlayerRevive;
    }


    public void Setup(Camera cam, GameObject cameraRotator, Transform centerOfWorld, Rigidbody rigidbody, SharkSO sharkScriptableObject)
    {
        this.sharkScriptableObject = sharkScriptableObject;

        Instantiate(sharkScriptableObject.sharkModelPrefab, transform);
        
        this.cam = cam;
        this.cameraRotator = cameraRotator;
        this.centerOfWorld = centerOfWorld;
        this.rb = rigidbody;

        camStartingXAngle = cam.transform.rotation.eulerAngles.x;

        currentHealth = sharkScriptableObject.maxHealth;
    }

    void FixedUpdate()
    {
        HandleHealth();

        if (currentHealth > 0)
        {
            HandleYVelocity();
            HandleSwimming();
            RotateCam(); 
        }
    }

    void HandleSwimming()
    {
        Vector2 swimDir = InputManager.instance.swimDir;
        Vector3 velocity = Vector3.zero;

        Vector3 forward = transform.position - cam.transform.position;
        forward.y = 0;
        Vector3 right = -Vector3.Cross(forward, Vector3.up);
        forward *= swimDir.y;
        right *= swimDir.x;
        velocity += (forward + right).normalized * sharkScriptableObject.moveSpeed;

        velocity.y = yVelocity;
        transform.LookAt(transform.position + velocity);
        rb.velocity = velocity;
    }
    void HandleYVelocity()
    {
        if (InputManager.instance.pressingSwimUp)
        {
            yVelocity = sharkScriptableObject.moveSpeed;
        }

        if (yVelocity > -20)
        {
            yVelocity -= 9f * 2 * Time.fixedDeltaTime;
        }
    }
    void RotateCam()
    {
        // Convert the angle to degrees
        float angleDeg = AngleBetweenPoints(new Vector2(transform.position.x, transform.position.z), new Vector2(centerOfWorld.position.x, centerOfWorld.position.z));

        Vector3 currCamRotationEuler = cameraRotator.transform.localRotation.eulerAngles;
        currCamRotationEuler.y = -angleDeg;
        cameraRotator.transform.localRotation = Quaternion.Euler(currCamRotationEuler);

        // Also move the cam with the distance to the player on x and z achsis
        Vector3 centerWorldPos = new Vector3(centerOfWorld.position.x, 0, centerOfWorld.position.z);
        Vector3 playerPos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 dirToPlayer = (playerPos - centerWorldPos).normalized;
        float distToPlayer = Vector3.Distance(centerWorldPos, playerPos);


        Vector3 camPos = cam.transform.position;

        camPos.x = dirToPlayer.x * (distToPlayer + sharkScriptableObject.camDistance);
        camPos.z = dirToPlayer.z * (distToPlayer + sharkScriptableObject.camDistance);

        // damp the y
        //                                                                            this is how fast per second the cam adjusts
        camPos.y = Mathf.Lerp(camPos.y - sharkScriptableObject.camYOffset, transform.position.y, 2f * Time.fixedDeltaTime);
        camPos.y += sharkScriptableObject.camYOffset;

        cam.transform.position = camPos;

        cam.transform.LookAt(transform);
        Vector3 camRot = cam.transform.rotation.eulerAngles;
        camRot.x = camStartingXAngle;
        cam.transform.rotation = Quaternion.Euler(camRot);
    }
    void HandleHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth -= Time.fixedDeltaTime * 0.1f;
            if (currentHealth < 0.001f)
            {
                currentHealth = 0;
                OnPlayerDied();
                SignalBus.OnPlayerDeathInvoke();
            }
        }
    }
    void OnPlayerDied()
    {
        playerDiedPos = transform.position;

        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x,  -5, rb.velocity.z);
        }
    }

    void OnPlayerRevive()
    {
        transform.position = playerDiedPos;
        rb.velocity = Vector3.zero;
        yVelocity = 0;
        currentHealth = sharkScriptableObject.maxHealth;
    }

    float AngleBetweenPoints(Vector2 center, Vector2 other)
    {
        // Calculate the direction vector from center to the other point
        Vector2 direction = other - center;

        // Calculate the angle in radians using Atan2
        float angleRad = Mathf.Atan2(direction.y, direction.x);

        // Convert the angle to degrees
        float angleDeg = angleRad * Mathf.Rad2Deg;

        // Ensure the angle is between 0 and 360 degrees
        angleDeg = (angleDeg + 360) % 360;

        return angleDeg;
    }

}
