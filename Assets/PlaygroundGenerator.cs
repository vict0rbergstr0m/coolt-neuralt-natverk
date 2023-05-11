using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundGenerator : MonoBehaviour
{
    public GameObject[] obstacles;
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
            GameObject obj = Instantiate(obstacles[randI]);
            obj.transform.SetParent(transform);
            Vector3 pos = new Vector3(Random.Range(-arenaDimensions.x,arenaDimensions.x)/2,Random.Range(-arenaDimensions.y,arenaDimensions.y)/2,Random.Range(-arenaDimensions.z,arenaDimensions.z)/2);
            obj.transform.localPosition = pos;
        }
    }
}
