using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagMatchManager : MonoBehaviour
{
    //TODO: when an agent dies, it should send an event to this class, and then this class calls end game for both agents
    //TODO: this class should also handle all spawning and level generation etc.

    [SerializeField] private TagAgent[] agents;
    [SerializeField] private PlaygroundGenerator generator;
    private void Start() { //need to give all agents a reference to this class/component
        foreach (var agent in agents)
        {
            agent.AssignMatchManager(this);
        }
    }

    //THESE ARE EXAMPLE FUNCTIONS THAT MIGHT WORK WELL
    public void OnBeginMatch()
    {
        generator.GenerateLevel();
        //"Spawn all agents"
    }

    void Update() 
    {
        if(endGame) //EndGame cant be called from certain functions
        {
            StartCoroutine(EndGame());
        }
    }

    bool endGame = false;
    public void RequestEndGame()//this should be called by the agents when they "die"
    {
        //idk if you might need to do some weird checks before EndGame.
        endGame = true;
    }

    IEnumerator EndGame()//this ensures we only call EndGame once in one frame;
    {
        yield return null;
        endGame = false;
        foreach (var agent in agents)
        {
            agent.EndEpisode();
        }
        //call end game for all agents
    }

}
