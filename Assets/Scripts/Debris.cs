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
    [SerializeField]
    private ParticleSystem debrisParticle;
    [SerializeField]
    private ParticleSystem explode;
    //[SerializeField]
    //private AudioClip explosionEarth;
    //[SerializeField]
    //private AudioClip explosionOzone;

    //AudioSource audioSource;

    private float speed;
    private float arc;
    private bool canMove;

    Earth target;
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        canMove = true;
        speed = Random.Range(minSpeed, maxSpeed);
        arc = Random.Range(minArc, maxArc);
        target = FindObjectOfType<Earth>();
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.RotateAround(target.transform.position, Vector3.forward, arc * Time.deltaTime);
        }

        if (gc.GetGameOver)
        {
            Destroy(gameObject);
        }
    }

    public void Explode()
    {
        canMove = false;
        debrisParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        explode.Play();
        Destroy(gameObject, explode.main.duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.gameObject.tag == "Ozone")
        //{
        //    audioSource.PlayOneShot(explosionOzone);
        //}
        //if (collision.gameObject.tag == "Earth")
        //{
        //    audioSource.PlayOneShot(explosionEarth);
        //}

        Explode();
    }
}
