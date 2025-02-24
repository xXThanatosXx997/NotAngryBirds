using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CannonBallManage : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasStopped = false;
    [SerializeField] public GameObject parent;
    void Start()
    {
        parent.GetOrAddComponent<CannonScript>().controlsOn = false;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasStopped && rb.velocity.magnitude < 1f)
        {
            Debug.Log("Cannonball has stopped moving.");
            StartCoroutine("Cancel");
        }
    }

    IEnumerator Cancel()
    {


        yield return new WaitForSeconds(2f);
        parent.GetComponent<CannonScript>().ResetCamera();
        parent.GetOrAddComponent<CannonScript>().controlsOn =true;
        Destroy(this.GetComponent<CannonBallManage>());

    }
}
