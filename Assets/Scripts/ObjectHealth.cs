using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float forceThreshold = 5f; // Minimum force to trigger damage
    public float health = 100f; // Initial health value
    private Rigidbody2D rb2D;
    public LayerMask affectedLayers; // Define which layers are affected
    public float explosionForce = 500f; // Force of the explosion
    public float explosionRadius = 5f; // Range of the explosion
    public bool isExplode;
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
            if (health <= 0 && isExplode==false)
            {
                Death();
            }else if(health <= 0 && isExplode==true)
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                this.gameObject.isStatic = true;
           
                Explode();
            }
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, affectedLayers);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float forceMultiplier = Mathf.Clamp01(1 - (distance / explosionRadius));
                                                                                        
                rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
                if (hit.gameObject.GetComponent<SlimeDeath>() != null)
                {
                    hit.gameObject.GetComponent<SlimeDeath>().GetComponent<Animator>().SetBool("Dead", true);
                }
                
            }
        }

        Destroy(this.gameObject.GetComponent<Rigidbody2D>());
        Destroy(this.gameObject, 3f);
    }
}
