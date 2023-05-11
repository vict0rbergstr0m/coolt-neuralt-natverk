using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TagPreyAgent : Agent
{

    [SerializeField]
    private Transform target;
    public Rigidbody rigid;
    public Vector3 startArea = new Vector3(20f,0,20f);

    public override void OnEpisodeBegin()
    {
        //TODO: the position should be set by playground generator, when setting position make sure you are not ontop of obstacle or other agent
        Vector3 pos = new Vector3(Random.Range(-startArea.x,startArea.x)/2,Random.Range(-startArea.y,startArea.y)/2,Random.Range(-startArea.z,startArea.z)/2);
        transform.localPosition = pos;
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

        Vector3 velocity = new Vector3(moveX,0,moveY) * 6; //TODO: make this smoother somhow? add force maybe. need fixed update then? should probably use fixed update for velocity either way
        rigid.velocity = velocity;

        AddReward(Time.deltaTime);

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
        Debug.Log("lost!", this);
        AddReward(-10f);
        EndEpisode();
    }

}
