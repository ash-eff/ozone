using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{

    public GameObject debrisPrefab;

    private float screenHeight;
    private float screenWidth;
    GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;
    }

    void Update()
    {
        if (!gc.GameStarted)
        {
            return;
        }

        if (!gc.GetGameOver)
        {
            screenHeight = Camera.main.orthographicSize;
            screenWidth = screenHeight * Camera.main.aspect;
        }
    }

    public void StartDebrisSpawner()
    {
        StartCoroutine(SpawnDebris());
    }

    IEnumerator SpawnDebris()
    {
        while (!gc.GetGameOver)
        {
            float xAxis = GetRandomAxis(screenWidth);
            float yAxis = GetRandomAxis(screenHeight);

            Instantiate(debrisPrefab, new Vector2(xAxis, yAxis), Quaternion.identity);
            yield return new WaitForSeconds(0.4f);
        }
    }

    float GetRandomAxis(float dimension)
    {
        float rand = Random.Range(dimension + 1, dimension + 2);
        if (Random.Range(0, 100) <= 50)
        {
            rand = rand * -1f;
        }

        return rand;
    }
}
