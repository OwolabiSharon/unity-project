using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinCollection : MonoBehaviour
{
    [SerializeField] GameObject coin;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            Destroy(coin);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
