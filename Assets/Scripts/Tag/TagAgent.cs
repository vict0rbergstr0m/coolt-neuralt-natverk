using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public abstract class TagAgent : Agent
{
    [SerializeField] private Vector3 startArea = new Vector3(20f,0,20f);
    public TagAgentMovement movement;

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

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        //Here you can override the action inputs (for example manually moving using arrow keys)
        //continousActions[1] = rightarrow
        //continousActions[0] = uparrow
        //....
        continousActions[0] = Input.GetKey("a")?1:0;
        continousActions[1] = Input.GetKey("w")?1:0;

    }

}
