using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TagPreyAgent : TagAgent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        Vector3 velocity = new Vector3 (0, 0 ,0);
        if (manager.episodeCounter <= 1000000)
        {
           velocity = Vector3.Normalize(new Vector3(0, 0, moveY));
        } else
        {
           velocity = new Vector3(0, 0, moveY);
        }
        movement.SetTargetMovement(velocity);

        movement.setTargetAngleVel(moveX);
        AddReward(Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "obstacle")//if obstacle/dosnt contain rigidbody kill this
        {
            OnHitWall();
        }
    }
    public void timeOver()
    {
        AddReward(200f);
    }
    public void OnGotCaught()
    {
        if(endEpisode)
        {
            return;
        }

        alive = false;
        ClearDetections();
        Debug.Log("Lost!", this);
        AddReward(-200f);
        manager.RequestEndGame();
       // RecursionSafeEndEpisode();
    }

}
