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
            Destroy(collision.gameObject);
            gameManager.HitBlock();
        }
        if(collision.gameObject.tag == "Racket"){
            gameManager.HitRacket();
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.name == "Miss") gameManager.MissRacket();
    }
}
