using System.Diagnostics;
using UnityEngine;

public class ShrinkingWaterSource : MonoBehaviour
{
    public float shrinkRate = 0.01f;
    public float minSize = 1f;
    public float destroyDelay = 0f;
    private bool isShrinking = false;

    private WaterSourcePool waterSourcePool;

    void Start()
    {
        // Explicitly use UnityEngine.Object to resolve ambiguity
        waterSourcePool = UnityEngine.Object.FindFirstObjectByType<WaterSourcePool>();
        
    }

    void Update()
    {
        // Shrink the water only if it is flagged as shrinking
        if (isShrinking && transform.localScale.x > minSize)
        {
            float shrinkAmount = shrinkRate * Time.deltaTime;
            transform.localScale -= new Vector3(shrinkAmount, 0, shrinkAmount);

            // Check if the water has reached its minimum size
            if (transform.localScale.x <= minSize)
            {
                // Return the water source to the pool
                if (waterSourcePool != null)
                {
                    waterSourcePool.ReturnWaterSource(gameObject);
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the agent
        if (other.CompareTag("Agent"))
        {
            isShrinking = true; // Start shrinking when the agent collides
        }
    }
}
