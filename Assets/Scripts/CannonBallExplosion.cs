using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallExplosion : MonoBehaviour
{
    public float explosionForce = 500f; // Force of the explosion
    public float explosionRadius = 5f; // Range of the explosion
    public LayerMask affectedLayers; // Define which layers are affected
    public LayerMask obstacleLayers; // Define which layers block the explosion force
    public bool checkLineOfSight = true; // Toggle for line-of-sight check

    private void Start()
    {
        this.gameObject.GetComponent<CannonBallManage>().isExplosive=true;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        this.gameObject.isStatic = true;
        
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        Explode();
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
                float forceMultiplier = Mathf.Clamp01(1 - (distance / explosionRadius)); // Reduce force with distance

                if (checkLineOfSight)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, explosionRadius);
                    foreach (RaycastHit2D rayHit in hits)
                    {
                        if (rayHit.collider == hit)
                        {
                            rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
                            if(hit.gameObject.GetComponent<SlimeDeath>() != null)
                            {
                                hit.gameObject.GetComponent<SlimeDeath>().GetComponent<Animator>().SetBool("Dead", true);
                            }

                            break; // Stop checking further objects behind
                        }
                        else if (rayHit.collider != null && rayHit.collider.gameObject != gameObject)
                        {
                            break; // If another object is hit first, block force application
                        }
                    }
                }
                else
                {
                    rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
                    if (hit.gameObject.GetComponent<SlimeDeath>() != null)
                    {
                        hit.gameObject.GetComponent<SlimeDeath>().GetComponent<Animator>().SetBool("Dead", true);
                    }
                }
            }
        }
        Destroy(this.gameObject.GetComponent<Rigidbody2D>());
        this.gameObject.GetComponent<CannonBallManage>().StartCoroutine("Cancel");
        Destroy(this.gameObject, 3f);
    }

   
}
