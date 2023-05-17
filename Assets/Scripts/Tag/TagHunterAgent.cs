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
    }

    float totalSeeReward = 0;
    public override void OnObservation()
    {
        float seeingTargetReward = 50.0f/(float)Math.Log10(manager.episodeCounter);
        foreach(var ray in detections)
        {
            if(ray.objectId > 0 && ray.objectId != teamId) //if not wall and from other team
            {
                totalSeeReward += (Time.deltaTime*5.0f*seeingTargetReward);
                if(totalSeeReward < 5000/(float)Math.Log10(manager.episodeCounter))
                {
                    AddReward((Time.deltaTime*5.0f*seeingTargetReward));
                }

                TagPreyAgent prey;
                if(ray.distance < catchDistance && ray.hit.TryGetComponent<TagPreyAgent>(out prey)) //if close enough and target is prey
                {
                    if(prey.alive)
                    {
                        OnCaughtTarget(prey);
                        break;
                    }
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

    public void OnCaughtTarget(TagPreyAgent prey)
    {
        if(endEpisode)
        {
            return;
        }

        prey.OnGotCaught();
        ClearDetections();
        Debug.Log("Won!", this);
        AddReward(500f);
        manager.RequestEndGame();
       // RecursionSafeEndEpisode();
    }
}
