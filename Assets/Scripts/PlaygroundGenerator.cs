using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Obstacle 
{
    public GameObject prefab;
    public Vector3 randomRotation = new Vector3(1,1,1);
    public Vector3 minScale = new Vector3(1,1,1);
    public Vector3 maxScale = new Vector3(1,1,1);
}
//TODO: could be good to gradually add more obstacles every iteration, so the agents have a chance to find eachother in the begining
public class PlaygroundGenerator : MonoBehaviour
{
    public Obstacle[] obstacles;
    private List<GameObject> obstacleObjects = new List<GameObject>(); //could use a pool but i dont care...
    public void GenerateLevel(int numberOfObstacles, Vector3 arenaDimensions)
    {
        foreach(var ob in obstacleObjects)
        {
            Destroy(ob);
        }
        obstacleObjects.Clear();

        for(int i = 0; i < numberOfObstacles; i++)
        {
            int randI = Random.Range(0,obstacles.Length);
            GameObject obj = Instantiate(obstacles[randI].prefab);
            obj.transform.SetParent(transform);
            Vector3 pos = new Vector3(Random.Range(-arenaDimensions.x,arenaDimensions.x)/2,0,Random.Range(-arenaDimensions.z,arenaDimensions.z)/2);
            obj.transform.localPosition = pos;

            Vector3 randomRot = new Vector3(obstacles[randI].randomRotation.x * Random.Range(0.0f,1.0f),obstacles[randI].randomRotation.y * Random.Range(0.0f,1.0f),obstacles[randI].randomRotation.z * Random.Range(0.0f,1.0f));
            obj.transform.eulerAngles += randomRot;

            Vector3 randomScale = new Vector3(Random.Range(obstacles[randI].minScale.x,obstacles[randI].maxScale.x),
            Random.Range(obstacles[randI].minScale.y,obstacles[randI].maxScale.y),
            Random.Range(obstacles[randI].minScale.z,obstacles[randI].maxScale.z));
            obj.transform.localScale = randomScale;
            obstacleObjects.Add(obj);
        }
    }
}
