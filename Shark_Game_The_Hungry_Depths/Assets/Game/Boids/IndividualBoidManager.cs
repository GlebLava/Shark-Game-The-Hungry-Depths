using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// To put on sharks on the title screen for example and it just works
/// Very ineficient
/// Avoids all colliders
/// </summary>
public class IndividualBoidManager : MonoBehaviour
{

    readonly int maxSpeed = 5;
    Vector3 velocity;

    private void Awake()
    {
        velocity = Random.insideUnitSphere;
    }

    private void FixedUpdate()
    {
        Vector3 acceleration = Vector3.zero;
        acceleration += SteerTowards(GetAvoidEverythingVelocity()) * 4;
        acceleration += SteerTowards(Random.insideUnitSphere);

        velocity += acceleration * Time.fixedDeltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = speed == 0 ? Vector3.zero : velocity / speed;
        speed = Mathf.Clamp(speed, 0.1f, maxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.fixedDeltaTime;
        transform.LookAt(transform.position + velocity);
    }

    private Vector3 GetAvoidEverythingVelocity()
    {
        float radius = 3;
        Vector3 dir = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider coll in colliders)
        {
            float weight = 1 - (Vector3.Distance(transform.position, coll.transform.position) / radius);
            dir += weight * (transform.position - coll.transform.position).normalized;
        }

        return dir;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, 0.2f);
    }


}
