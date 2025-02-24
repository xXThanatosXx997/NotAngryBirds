using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CannonScript : MonoBehaviour
{
    [Header("Cannon Settings")]
    public List<GameObject> cannonBallPrefabs; // List of cannonball prefabs to shoot (last element will be used)
    public Transform firePoint;              // The point from where the cannonball will be instantiated
    public float rotationSpeed = 5f;         // How fast the cannon rotates toward the mouse
    public float chargeSpeed = 1f;           // How fast the charge increases
    public float maxCharge = 10f;            // Maximum charge value
    public float chargeMultiplier = 2f;

    [Header("Debug")]
    public float currentCharge = 0f;         // Current charge value (for testing/visualization)

    private bool isCharging = false;

    [SerializeField] private CinemachineVirtualCamera mycamera;
    [SerializeField] private Slider chargeSlider;
    private float initialOrthoSize;

    public bool controlsOn;
    void Start()
    {
        initialOrthoSize = mycamera.m_Lens.OrthographicSize;
    }

    void Update()
    {
        // Update the UI slider
        chargeSlider.value = currentCharge / maxCharge;

        // Adjust camera zoom based on charge level
        mycamera.m_Lens.OrthographicSize = initialOrthoSize + (currentCharge / maxCharge) * 2f;

        // Only process rotation and charging if the left mouse button is held down
        if (Input.GetMouseButton(0) && controlsOn)
        {
            RotateTowardMouse();

            // Increase charge over time until maximum is reached
            isCharging = true;
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
        }
        else if (Input.GetMouseButtonUp(0) && isCharging &&controlsOn)
        {
            // On mouse release, shoot the cannonball with the current charge value
            ShootCannonBall();
            currentCharge = 0f;
            isCharging = false;

            // Reset camera zoom
            mycamera.m_Lens.OrthographicSize = initialOrthoSize;
        }
    }

    // Rotate the cannon to face the mouse position
    void RotateTowardMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Calculate the direction from the cannon to the mouse
        Vector3 direction = mousePos - transform.position;
        direction.Normalize();

        // Determine the target angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards the target angle
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Instantiate the last prefab and remove it from the list
    void ShootCannonBall()
    {
        if (cannonBallPrefabs.Count == 0)
        {
            Debug.LogWarning("No cannonball prefabs left in the list!");
            return;
        }

        // Get the last prefab in the list
        int lastIndex = cannonBallPrefabs.Count - 1;
        GameObject cannonBallPrefab = cannonBallPrefabs[lastIndex];

        // Instantiate the cannonball at the fire point with the same rotation as the cannon
        GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, transform.rotation);

        // Optionally, if your cannonball has a Rigidbody2D or Rigidbody, you can apply a force based on charge
        Rigidbody2D rb2D = cannonBall.GetComponent<Rigidbody2D>();

        rb2D.GetComponent<CannonBallManage>().parent = this.gameObject;
        // Example: apply a force proportional to the charge value
        Vector2 forceDir2D = firePoint.right; // Assuming the cannon's right direction is the forward direction

        if (rb2D != null)
        {
            rb2D.AddForce(forceDir2D * currentCharge * chargeMultiplier, ForceMode2D.Impulse);

            mycamera.Follow = rb2D.transform;
        }
        else
        {
            Debug.LogWarning("The cannonball prefab has no Rigidbody component!");
        }

        // Remove the used prefab from the list
        cannonBallPrefabs.RemoveAt(lastIndex);
    }

    public void ResetCamera()
    {
        mycamera.Follow = this.transform;
        mycamera.m_Lens.OrthographicSize = initialOrthoSize;
    }
}
