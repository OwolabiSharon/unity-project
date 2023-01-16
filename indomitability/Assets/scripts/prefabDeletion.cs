using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabDeletion : MonoBehaviour
{
    public GameObject player;
    public float threshold = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > threshold)
        {
            Destroy(gameObject);
        }
        
    }
}
