using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TagHunterAgent : Agent
{

    [SerializeField]
    private Transform target;
    
    public Vector3 startPosition = new Vector3(-0.25f,0,0);

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPosition;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX,0,moveY) *Time.deltaTime * 5;

        if(Vector3.SqrMagnitude(target.localPosition-transform.localPosition) < 1)
        {
            OnReachedTarget();
        }

        AddReward(-Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        //Here you can override the action inputs (for example manually moving using arrow keys)
        //continousActions[1] = rightarrow
        //continousActions[0] = uparrow
        //....
    }

    void OnReachedTarget()
    {
        AddReward(10f);
        EndEpisode();
    }

}
