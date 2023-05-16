using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TagHunterAgent : TagAgent
{
    [SerializeField] private float catchDistance = 1.5f;
    [SerializeField] private int episodeCounter = 0;
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector3 velocity = new Vector3(0,0,moveY).normalized; 
        movement.SetTargetMovement(velocity);

        movement.setTargetAngleVel(moveX);
        AddReward(-Time.deltaTime);
    }

    public override void OnBegin()
    {
        totalSeeReward = 0;
        episodeCounter += 1;
        print(episodeCounter);
    }

    float totalSeeReward = 0;
    public override void OnObservation()
    {
        float seeingTargetReward = 50.0f/(float)Math.Log10(episodeCounter);
        foreach(var ray in detections)
        {
            if(ray.objectId > 0 && ray.objectId != teamId) //if not wall and from other team
            {
                totalSeeReward += (Time.deltaTime*5.0f*seeingTargetReward);
                if(totalSeeReward < 1000)
                {
                    AddReward((Time.deltaTime*5.0f*seeingTargetReward));
                }
                if(ray.distance < catchDistance) //if close enough
                {
                    ray.hit.GetComponent<TagPreyAgent>().OnGotCaught(); //todo: if target is on another team but still a hunter things will break.
                    OnCaughtTarget();
                    break;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "obstacle")//if obstacle/dosnt contain rigidbody kill this
        {
            OnHitWall();
        }
    }

    public void OnCaughtTarget()
    {
        if(endEpisode)
        {
            return;
        }

        ClearDetections();
        Debug.Log("Won!", this);
        AddReward(500f);
        RecursionSafeEndEpisode();
    }
}
