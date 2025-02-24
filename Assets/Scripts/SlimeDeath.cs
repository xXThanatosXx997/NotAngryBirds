using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeath : MonoBehaviour
{
    public float forceThreshold = 5f; // Minimum force to trigger a log
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

        // Log the impact if it exceeds the threshold
        if (impactForce > forceThreshold)
        {
            Debug.Log($"2D Impact Force: {impactForce} N with {collision.gameObject.name}");
            this.GetComponent<Animator>().SetBool("Dead", true);
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

}
