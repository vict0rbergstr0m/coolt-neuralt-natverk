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

public class PlaygroundGenerator : MonoBehaviour
{
    public Obstacle[] obstacles;
    public Vector3 arenaDimensions = new Vector3(20,1,20); 
    public int n_Obstacles = 10;

    private void Start() {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for(int i = 0; i < n_Obstacles; i++)
        {
            int randI = Random.Range(0,obstacles.Length);
            GameObject obj = Instantiate(obstacles[randI].prefab);
            obj.transform.SetParent(transform);
            Vector3 pos = new Vector3(Random.Range(-arenaDimensions.x,arenaDimensions.x)/2,Random.Range(-arenaDimensions.y,arenaDimensions.y)/2,Random.Range(-arenaDimensions.z,arenaDimensions.z)/2);
            obj.transform.localPosition = pos;

            Vector3 randomRot = new Vector3(obstacles[randI].randomRotation.x * Random.Range(0.0f,1.0f),obstacles[randI].randomRotation.y * Random.Range(0.0f,1.0f),obstacles[randI].randomRotation.z * Random.Range(0.0f,1.0f));
            obj.transform.eulerAngles += randomRot;

            Vector3 randomScale = new Vector3(Random.Range(obstacles[randI].minScale.x,obstacles[randI].maxScale.x),
            Random.Range(obstacles[randI].minScale.y,obstacles[randI].maxScale.y),
            Random.Range(obstacles[randI].minScale.z,obstacles[randI].maxScale.z));
            obj.transform.localScale = randomScale;
        }
    }
}
