using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SzitakotoAgent : Agent
{
    public float speed = 6f; // Adjusted for faster pursuit
    public int rotation_speed = 300;
    public float StartingNeedOfWater = 100f;
    public float needOfWater = 100f;
    private bool nearWater = false;
    public WaterSourcePool waterSourcePool;
    public int numWaterSources = 5;

    public override void OnEpisodeBegin()
    {
        Vector3 agentPosition;
       
            agentPosition = new Vector3(
                UnityEngine.Random.Range(-48f, 48f),
                1f,
                UnityEngine.Random.Range(-48f, 48f)
            );
 

        transform.localPosition = agentPosition;
        transform.rotation = Quaternion.identity;
        needOfWater = StartingNeedOfWater;

        // Reset water sources
        waterSourcePool.ResetWaterSources();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(needOfWater / StartingNeedOfWater); // Normalize thirst level (0 to 1)
        sensor.AddObservation(nearWater ? 1f : 0f); // Add 1 if near water, else 0
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forward = actions.ContinuousActions[0]; // Forward/backward movement
        float rotate = actions.ContinuousActions[1];  // Rotation
        float waterLevel = needOfWater / StartingNeedOfWater;

        // Apply movement and rotation
        transform.Translate(Vector3.forward * forward * speed * Time.fixedDeltaTime);
        transform.Rotate(0, rotate * rotation_speed * Time.fixedDeltaTime, 0);

        // Keep the agent at a fixed height
      

        // Decrease thirst over time
        needOfWater -= Time.fixedDeltaTime * 10f;

        //UnityEngine.Debug.Log($"needOfWater: {needOfWater}, StartingNeedOfWater: {StartingNeedOfWater}");

        
        
        if (waterLevel >= 0.75f)
        {
            AddReward(1);
        } else if (waterLevel >= 0.5f && waterLevel < 0.75f)
        {
            AddReward(0.75f);
        } else if (waterLevel >= 0.25f && waterLevel < 0.5f)
        {
            AddReward(0.5f);
        } else
        {
            AddReward(0.25f);
        } 
        // UnityEngine.Debug.Log($"reward: {needOfWater / StartingNeedOfWater}");

            // End episode if thirst runs out
            if (needOfWater <= 0)
        {
            AddReward(-100f);
            EndEpisode();
        }



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-5f);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            nearWater = true;
            //UnityEngine.Debug.Log("IN");
            if (needOfWater > StartingNeedOfWater-1)
            {
                needOfWater = StartingNeedOfWater;
            }
            else
            {
                needOfWater += Time.fixedDeltaTime * 25f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            nearWater = false;
            //UnityEngine.Debug.Log("EXIT");
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Vertical");  // Forward/backward
        actions[1] = Input.GetAxis("Horizontal");  // Rotate
    }
}
