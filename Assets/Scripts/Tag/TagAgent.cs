using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public abstract class TagAgent : Agent
{
    //TODO: instead of calling endEpisode, send a message to some training Manager that, in turn, calls endEpisode for both agents, generates a new level and sets their spawn points
    public struct RayDetector
    {
        public int objectId; //-1 is wall, 0,1,2... is agent teams
        public float distance;
        public Transform hit;
    }

    public bool alive = false;
    public int teamId = 0;
    public TagMatchManager manager {get; private set;}
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private Vector3 startArea = new Vector3(20f,0,20f);
    [SerializeField] private float fov = 90;
    [Tooltip("for each ray add 2 to Observations, distance and tag for each ray")]
    [SerializeField] private int rayCount = 15;
    [SerializeField] private float visionDistance = 10;

    public TagAgentMovement movement;

    public RayDetector[] detections {get; private set;}

    public void ClearDetections()
    {
        detections = new RayDetector[rayCount];
    }

    public void AssignMatchManager(TagMatchManager manager)
    {
        this.manager = manager;
    }
    public override void OnEpisodeBegin()//
    {
        manager.RequestBeginMatch();

        endEpisode = false;
        ClearDetections();
        OnBegin();
    }
    
    public virtual void OnBegin(){}

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < rayCount; i++)
        {
            float p = 0.5f;
            if(rayCount != 1)
            {
                p = (float)(i)/(float)(rayCount-1);
            }
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 dir = Quaternion.AngleAxis(fov*p-(fov/2), Vector3.up) * transform.forward;
            RayDetector detected = shotDetector(origin, dir);
            sensor.AddObservation(detected.distance);
            sensor.AddObservation(detected.objectId);
            detections[i] = detected;
        }

        if(endEpisode)
        {
            return;
        }
        OnObservation();
    }

    public virtual void OnObservation(){}


    //could use capsule cast instead of ray?
    RayDetector shotDetector(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(origin,direction, out hit, visionDistance,visionMask); 
        RayDetector detector = new RayDetector();
        detector.objectId = 0;
        detector.distance = visionDistance;
        TagAgent agent;
        Debug.DrawRay(origin, direction*detector.distance,Color.red);
        if(hit.transform != null)
        {
            detector.distance = Vector3.Magnitude(origin-hit.point);
            detector.hit = hit.transform;
            Debug.DrawRay(origin, direction*detector.distance,Color.green);
            if(hit.transform.TryGetComponent<TagAgent>(out agent))
            {
                Debug.DrawRay(origin, direction*detector.distance,Color.blue);
                detector.objectId = agent.teamId*10;//multiply with 10 to get a bigger difference in values, hopfully making it easier for netowkr to distinguis objects
            }
        }
        return detector;
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

    void Update() {
        if(endEpisode) //if one ends, both agents in the same arean should end, send an event to ArenaManager?
        {
            EndEpisode();
        }
    }

    public bool endEpisode {get; private set;} = false;
    public void RecursionSafeEndEpisode()
    {
        endEpisode = true;
    }

    public void OnHitWall()
    {
        if(!alive)
        {
            return;
        }

        alive = false;
        Debug.Log("Ouch! hit wall.", this);
        AddReward(-200f);
        manager.RequestEndGame();
    }

}
