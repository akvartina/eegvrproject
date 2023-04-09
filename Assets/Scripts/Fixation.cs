using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixation : MonoBehaviour
{
    public GameObject fixationObject;
    public float fixationTime = 0.65f; // in seconds
    private bool isFixationActive = false;
    
    void Start()
    {
        fixationObject.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        //
    }

    public void ActivateFixation()
    {
        isFixationActive = true;
        Debug.Log("Fixation active");
        fixationObject.SetActive(true);
        // Start the timer
        Invoke("DeactivateFixation", fixationTime);
    }
    
    void DeactivateFixation()
    {
        // Deactivate the fixation object
        fixationObject.SetActive(false);
        Debug.Log("Fixation is over");
        isFixationActive = false;
    }
}
