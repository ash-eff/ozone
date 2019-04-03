using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    [SerializeField]
    private float minSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float minArc;
    [SerializeField]
    private float maxArc;
    private float speed;
    private float arc;

    Earth target;
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        arc = Random.Range(minArc, maxArc);
        target = FindObjectOfType<Earth>();
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        transform.RotateAround(target.transform.position, Vector3.forward, arc * Time.deltaTime);
        if (gc.GetGameOver)
        {
            Destroy(gameObject);
        }
    }
}
