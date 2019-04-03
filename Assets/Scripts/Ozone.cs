using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ozone : MonoBehaviour
{

    [SerializeField]
    private float speed;

    GameController gc;

    private void Start()
    {
        gc = FindObjectOfType <GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gc.GameStarted)
        {
            return;
        }

        if (!gc.GetGameOver)
        {
            float h = Input.GetAxisRaw("Horizontal");
            transform.Rotate(Vector3.forward * h * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Debris")
        {
            Destroy(collision.gameObject);
            gc.UpdateScore(1);
        }
    }
}
