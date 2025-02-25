using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float forceThreshold = 5f; // Minimum force to trigger damage
    public float health = 100f; // Initial health value
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the relative velocity from the collision data
        Vector2 relativeVelocity = collision.relativeVelocity;

        // Get the mass of this object from its Rigidbody2D
        float mass = rb2D.mass;

        // Calculate force: F = m * a (where a is approximated by the change in velocity)
        float impactForce = mass * relativeVelocity.magnitude;

        // Apply damage if impact exceeds threshold
        if (impactForce > forceThreshold)
        {
            health -= impactForce;

            if (health <= 0)
            {
                Death();
            }
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
    }

}
