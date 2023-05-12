using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public abstract class TagAgent : Agent
{

    struct RayDetector
    {
        public int objectId; //-1 is wall, 0,1,2... is agent teams
        public float distance;
    }

    public int teamId = 0;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private Vector3 startArea = new Vector3(20f,0,20f);
    [SerializeField] private float fov = 90;
    [Tooltip("for each ray add 2 to Observations, distance and tag for each ray")]
    [SerializeField] private int rayCount = 15;
    [SerializeField] private float visionDistance = 10;

    public TagAgentMovement movement;

    public override void OnEpisodeBegin()
    {
        //TODO: the position should be set by playground generator, when setting position make sure you are not ontop of obstacle or other agent
        Vector3 pos = new Vector3(Random.Range(-startArea.x,startArea.x)/2,Random.Range(-startArea.y,startArea.y)/2,Random.Range(-startArea.z,startArea.z)/2);
        transform.localPosition = pos;
    }
    
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
        }
        
    }

    //could use capsule cast instead of ray?
    RayDetector shotDetector(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(origin,direction, out hit, visionDistance,visionMask); 
        RayDetector detector = new RayDetector();
        detector.objectId = -1;
        detector.distance = 10;
        TagAgent agent;
        if(hit.transform != null)
        {
            if(hit.transform.TryGetComponent<TagAgent>(out agent))
            {
                detector.objectId = agent.teamId;
            }

            detector.distance = Vector3.Magnitude(origin-hit.point);
        }
        Debug.DrawRay(origin, direction*detector.distance,Color.red);
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

}
