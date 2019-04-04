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

    public IEnumerator SpawnDebris()
    {
        float time = 1.2f;
        while (!gc.GetGameOver)
        {

            if (time > 0.25f)
            {
                time -= Time.deltaTime / 3;
            }
            Vector2 newAxis = GetRandomAxis(screenWidth, screenHeight);
            Instantiate(debrisPrefab, newAxis, Quaternion.identity);
            yield return new WaitForSeconds(time);
        }
    }

    Vector2 GetRandomAxis(float _width, float _height)
    {
        float quarterChance = Random.value;
        float x = _width;
        float y = _height;

        // up
        if (quarterChance > .75f)
        {
            x = Random.Range(-_width, _width);
            y = _height;
        }
        // down
        else if (quarterChance > .5f)
        {
            x = Random.Range(-_width, _width);
            y = -_height;
        }
        // right
        else if (quarterChance > .25f)
        {
            x = _width;
            y = Random.Range(-_height, _height);
        }
        // left
        else if (quarterChance > 0)
        {
            x = -_width;
            y = Random.Range(-_height, _height);
        }

        Vector2 newAxis = new Vector2(x, y);
        return newAxis;
    }
}
