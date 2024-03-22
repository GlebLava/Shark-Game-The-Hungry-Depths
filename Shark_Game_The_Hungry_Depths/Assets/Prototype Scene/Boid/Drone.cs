using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public Joystick joystick;

    // Used to move camera
    public GameObject cameraRotator;
    public Transform centerOfWorld;
    public Camera cam;

    public BoidManagerPrototype boidManager;

    public RectTransform touchArea;

    public Transform mouthPos;

    public float moveSpeed = 4f;
    public float killRadius = 1.5f;
    private float yVelocity = 0f;

    Rigidbody rb;
    private float startingAngleCam;
    private float startingYOffsetCam;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingAngleCam = AngleBetweenPoints(new Vector2(transform.position.x, transform.position.z), new Vector2(centerOfWorld.position.x, centerOfWorld.position.z));
        startingYOffsetCam = cam.transform.position.y - transform.position.y;
    }

    void FixedUpdate()
    {
        HandleYVelocity();

        Vector3 velocity = Vector3.zero;


        if (joystick.Direction.magnitude > 0.01f)
        {
            Vector3 forward = transform.position - cam.transform.position;
            forward.y = 0;
            Vector3 right = -Vector3.Cross(forward, Vector3.up);
            forward *= joystick.Direction.y;
            right *= joystick.Direction.x;
            velocity += (forward + right).normalized * moveSpeed;
        }
        else
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (dir.magnitude > 0.01f)
            {
                Vector3 forward = transform.position - cam.transform.position;
                forward.y = 0;
                Vector3 right = -Vector3.Cross(forward, Vector3.up);
                forward *= dir.y;
                right *= dir.x;
                velocity += (forward + right).normalized * moveSpeed;
            }
        }


        velocity.y = yVelocity;

        transform.LookAt(transform.position + velocity);

        KillAllBoidsInRange();

        rb.velocity = velocity;

        RotateCam();
    }

    void HandleYVelocity()
    {
        if (Input.touchCount > 0)
        {
            // Loop through all touch inputs
            foreach (Touch touch in Input.touches)
            { 
                // Check if the touch position is within the defined touch area
                if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touch.position))
                {
                    yVelocity = moveSpeed;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, Input.mousePosition))
            {
                yVelocity = moveSpeed;
            }
        }

        if (yVelocity > -20)
        {
            yVelocity -= 9f * Time.fixedDeltaTime;
        }
    }

    void RotateCam()
    {
        // Convert the angle to degrees
        float angleDeg = AngleBetweenPoints(new Vector2(transform.position.x, transform.position.z), new Vector2(centerOfWorld.position.x, centerOfWorld.position.z));

        Vector3 currCamRotationEuler = cameraRotator.transform.localRotation.eulerAngles;
        currCamRotationEuler.y = -angleDeg + startingAngleCam;
        cameraRotator.transform.localRotation = Quaternion.Euler(currCamRotationEuler);

        // Also move the cam with the distance to the player on x and z achsis
        Vector3 centerWorldPos = new Vector3(centerOfWorld.position.x, 0, centerOfWorld.position.z);
        Vector3 playerPos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 dirToPlayer = (playerPos - centerWorldPos).normalized;
        float distToPlayer = Vector3.Distance(centerWorldPos, playerPos);


        Vector3 camPos = cam.transform.position;

        camPos.x = dirToPlayer.x * (distToPlayer + 7);
        camPos.z = dirToPlayer.z * (distToPlayer + 7);

        // damp the y
        camPos.y = Mathf.Lerp(camPos.y - startingYOffsetCam, transform.position.y, 0.125f * Time.fixedDeltaTime);
        camPos.y += startingYOffsetCam;
        

        cam.transform.position = camPos;
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

    private void KillAllBoidsInRange()
    {
        
        if (boidManager.boids == null)
            return;

        for (int i = 0; i < boidManager.boids.Length; i++)
        {
            if (boidManager.boids[i] == null) continue;

            Vector3 diff = mouthPos.position - boidManager.boids[i].transform.position;
            float distSquared = Vector3.Dot(diff, diff);

            if (distSquared <= killRadius * killRadius)
            {
                AudioManager.instance.PlayOneShot("pop");
                Destroy(boidManager.boids[i]);
            }
        }
        
    }
}
