using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlimeManager : MonoBehaviour
{
    public GameObject uiElement; // Assign the UI element in the inspector
    public Text resultText; // Assign a UI Text component in the inspector
    public GameObject cannon;
    public CannonScript cannonScript;

    void Start()
    {
        cannonScript =cannon.GetComponent<CannonScript>();
        if (uiElement != null)
        {
            uiElement.SetActive(false); // Hide UI initially
        }
    }

    void Update()
    {
        if (transform.childCount == 0)
        {
            EndGame("You Win");
        }
        else if (cannonScript != null && cannonScript.cannonBallPrefabs.Count == 0&& cannonScript.controlsOn==true)
        {
            EndGame("You Lose");
        }
    }

    void EndGame(string message)
    {
        if (uiElement != null && resultText != null)
        {
            uiElement.SetActive(true);
            resultText.text = message;
            StartCoroutine(ReloadScene());
        }
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
