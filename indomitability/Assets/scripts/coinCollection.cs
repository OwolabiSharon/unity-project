using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinCollection : MonoBehaviour
{
    public float coins = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            Destroy(gameObject);
            coins += 1f;
            PlayerPrefs.SetFloat("Coins", PlayerPrefs.GetFloat("Coins") + 1f);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
