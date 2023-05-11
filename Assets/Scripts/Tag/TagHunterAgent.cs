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
        //TODO: more visual input, raycasts/spheracasts in a cricle around player, or just infront 30* fov e.g.?
        //TODO: should they always know position of opponent? maybe each raycast returns 1 if wall, 2 if enemy 0 if nothing and distance aswell?
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        //TODO: the agent class should work as a input class, then we pass all the inputs to a "agentController" that has a constant fixed update loop
        //i dont think this function runs every frame, i do not know the exact intervall either.

        Vector3 velocity = new Vector3(moveX,0,moveY) * 5; //TODO: make this smoother somhow? add force maybe. need fixed update then? should probably use fixed update for velocity either way
        rigid.velocity = velocity;

        if(Vector3.SqrMagnitude(target.localPosition-transform.localPosition) < 2*2)
        {
            target.gameObject.SendMessage("OnReachedTarget"); //TODO: SendMessage is terrible, fix this later, should probably have reference to prey/hunter agent
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
        Debug.Log("won!", this);
        AddReward(10f);
        EndEpisode();
    }

}
