using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixation : MonoBehaviour
{
    public float fixationDuration = 0.65f; // in seconds
    private float timer = 0.0f;
    private bool fixationStarted = false;
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if fixation has started
        if (fixationStarted)
        {
            // Increment timer
            timer += Time.deltaTime;

            // Check if fixation duration has elapsed
            if (timer >= fixationDuration)
            {
                // Fixation completed, do something
                gameObject.SetActive(false);
                Debug.Log("Fixation completed!");
                fixationStarted = false;
            }
        }
        else
        {
            // Fixation hasn't started yet, start it now
            Debug.Log("Starting fixation...");
            gameObject.SetActive(true);
            fixationStarted = true;
            timer = 0.0f;
        }
    }

    public void FixCross()
    {
        fixationStarted = true;
    }
}
