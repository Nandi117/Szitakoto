using System.Diagnostics;
using UnityEngine;

public class ShrinkingWaterSource : MonoBehaviour
{
    public float shrinkRate = 0.01f;
    public float minSize = 1f;
    public float destroyDelay = 0f;

    private WaterSourcePool waterSourcePool;

    void Start()
    {
        // Explicitly use UnityEngine.Object to resolve ambiguity
        waterSourcePool = UnityEngine.Object.FindFirstObjectByType<WaterSourcePool>();
        
    }

    void Update()
    {
        if (transform.localScale.x > minSize)
        {
            float shrinkAmount = shrinkRate * Time.deltaTime;
            transform.localScale -= new Vector3(shrinkAmount, 0, shrinkAmount);
        }
        else
        {
            // Return the water source to the pool
            if (waterSourcePool != null)
            {
                waterSourcePool.ReturnWaterSource(gameObject);
            }
        }
    }
}
