using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourcePool : MonoBehaviour
{
    public GameObject waterPrefab;
    public int largeSources = 1;
    public int mediumSources = 5;
    public int smallSources = 10;
    public float LargeSize = 30f;
    public float MediumSize = 20f;
    public float SmallSize = 5f;
    public Vector2 spawnAreaMin = new Vector2(-48f, -48f);
    public Vector2 spawnAreaMax = new Vector2(48f, 48f);
    public float spawnPadding = 2f;

    private List<GameObject> waterSources = new List<GameObject>();

    public void ResetWaterSources()
    {
        foreach (var waterSource in waterSources)
        {
            if (waterSource != null)
                Destroy(waterSource);
        }
        waterSources.Clear();

        SpawnWaterSources(largeSources, LargeSize);
        SpawnWaterSources(mediumSources, MediumSize);
        SpawnWaterSources(smallSources, SmallSize);
    }

    private void SpawnWaterSources(int count, float size)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;

            do
            {
                Vector3 localSpawnPosition = new Vector3(
                    UnityEngine.Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    0f,
                    UnityEngine.Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                spawnPosition = transform.position + localSpawnPosition; // Adjust for parent's position

                attempts++;
                if (attempts > 100)
                {
                    UnityEngine.Debug.LogWarning("Failed to find a non-overlapping spawn position.");
                    return;
                }

            } while (IsOverlapping(spawnPosition, size));

            GameObject waterSource = Instantiate(waterPrefab, spawnPosition, Quaternion.identity, transform);
            waterSource.transform.localScale = new Vector3(size, 1f, size);
            waterSources.Add(waterSource);
        }
    }

    private bool IsOverlapping(Vector3 position, float size)
    {
        foreach (var waterSource in waterSources)
        {
            if (waterSource == null) continue;

            float distance = Vector3.Distance(position, waterSource.transform.position);
            float combinedRadius = (size + waterSource.transform.localScale.x) / 2f;

            if (distance < combinedRadius + spawnPadding)
            {
                return true;
            }
        }
        return false;
    }

    public void ReturnWaterSource(GameObject waterSource)
    {
        if (waterSources.Contains(waterSource))
        {
            waterSources.Remove(waterSource);
            Destroy(waterSource);
        }
    }
}
