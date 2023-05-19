using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagMatchManager : MonoBehaviour
{
    public int episodeCounter = 0; //should probably be private set, but i want to see it in inspector
    [SerializeField] private TagAgent[] agents;
    [SerializeField] private PlaygroundGenerator generator;
    [SerializeField] private LayerMask spawnObstacleMask;
    [SerializeField] private Vector3 arenaDimensions = new Vector3(20,1,20); 

    [SerializeField] private int currentObstacleCount = 0; //doesnt have to be visable, but i want it to for debug reasons
    [SerializeField] private Vector2 startEndObstacleCount = new Vector2(10,200);
    [SerializeField] private int obstacleIncrementEnd = 1000000;
    [SerializeField] private int maxGameTime = 40;
    private float deltaTimer = 0;

    //[SerializeField] private int maxGameTime = 60;

    private void Start() { //need to give all agents a reference to this class/component
        foreach (var agent in agents)
        {
            agent.AssignMatchManager(this);
        }
        episodeCounter = 0;
    }

    //THESE ARE EXAMPLE FUNCTIONS THAT MIGHT WORK WELL
    bool beginMatch = false;
    public void RequestBeginMatch()
    {
        beginMatch = true;
    }

    IEnumerator OnBeginMatchIE;
    IEnumerator OnBeginMatch() //THIS IS NEVER CALLED LEL
    {
        yield return null;
        beginMatch = false;
        deltaTimer = 0;
        currentObstacleCount = (int)Mathf.Lerp(startEndObstacleCount.x,startEndObstacleCount.y,(float)episodeCounter/(float)obstacleIncrementEnd);
        currentObstacleCount = (int)Mathf.Clamp(currentObstacleCount,0,startEndObstacleCount.y);
        
        generator.GenerateLevel(currentObstacleCount, arenaDimensions);
        //"Spawn all agents"
        Debug.Log("begin match plz");
        foreach(var agent  in agents)
        {
            SpawnAgent(agent);
        }

        yield return null;
        episodeCounter += 1;
        OnBeginMatchIE = null;
    }

    void SpawnAgent(TagAgent agent)
    {
        Vector3 pos = new Vector3(Random.Range(-arenaDimensions.x,arenaDimensions.x)/2,0,Random.Range(-arenaDimensions.z,arenaDimensions.z)/2);
        pos = agent.transform.TransformPoint(pos);
        bool invalidPosition = true;
        float radius = 1.5f;
        int tryCounter = 0;
        while(invalidPosition) //retry spawn position until we hit somthing that isnt an obstacle or other agent
        {
            pos = new Vector3(Random.Range(-arenaDimensions.x,arenaDimensions.x)/2,0,Random.Range(-arenaDimensions.z,arenaDimensions.z)/2);
            pos = agent.transform.TransformPoint(pos); //ensure we are shooting ray in world space
            RaycastHit[] hits = Physics.SphereCastAll(pos+Vector3.up*5, radius, Vector3.down, 50, spawnObstacleMask);
            
            if(hits.Length > 0)
            {
                invalidPosition = false;
                foreach(var hit in hits)
                {
                    if(hit.transform.tag == "obstacle" || hit.transform.tag == "agent")
                    {
                        invalidPosition = true;
                        break;
                    }
                }
            }else
            {
                invalidPosition = true;
            }
            tryCounter++;

            if(tryCounter > 5000)
            {
                Debug.LogWarning("Could not find spawn position", this);
                break;
            }
        }
        pos.y = 0;
        agent.movement.rigid.velocity = Vector3.zero;
        agent.movement.rigid.position = pos;
        agent.movement.SetTargetMovement(Vector3.zero);
        agent.alive = true;
    }

    void Update() 
    {
        deltaTimer += Time.deltaTime;
        print(deltaTimer);
        if (beginMatch && OnBeginMatchIE == null)
        {
            OnBeginMatchIE = OnBeginMatch();
            StartCoroutine(OnBeginMatchIE);
        }
        if(deltaTimer > maxGameTime)
        {
            foreach (var agent in agents)
            {
                TagPreyAgent prey;
                TagHunterAgent hunter;
                if (agent.TryGetComponent<TagPreyAgent>(out prey)) { prey.timeOver(); }
                else if (agent.TryGetComponent<TagHunterAgent>(out hunter)) { hunter.timeOver(); }
                //agent.timeOver();
            }
            RequestEndGame();
        }

        if(endGame && endGameIE == null) //EndGame cant be called from certain functions
        {
            endGameIE = EndGame();
            StartCoroutine(endGameIE);
        }


    }

    bool endGame = false;
    public void RequestEndGame()//this should be called by the agents when they "die"
    {
        //idk if you might need to do some weird checks before EndGame.
        endGame = true;
    }

    IEnumerator endGameIE;
    IEnumerator EndGame()//this ensures we only call EndGame once in one frame;
    {
        yield return null;
        endGame = false;
        foreach (var agent in agents)
        {
            agent.EndEpisode();
        }
        yield return null;
        endGameIE = null;
    }

}
