using UnityEngine;

public class RunInBackground : MonoBehaviour
{
    void Start()
    {
        // Allow Unity to run in the background
        Application.runInBackground = true;
    }
}
