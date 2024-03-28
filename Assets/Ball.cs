using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Miss").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        GameObject.Find("SndBall").GetComponent<AudioSource>().Play();
        if(collision.gameObject.tag == "Block"){
            collision.gameObject.GetComponent<Renderer>().enabled = false;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            gameManager.HitBlock();
        }
        if(collision.gameObject.tag == "Racket"){
            gameManager.HitRacket();
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        gameManager.MissRacket();
    }
}
