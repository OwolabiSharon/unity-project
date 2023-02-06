using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class coinCollection : MonoBehaviour
{
    public float coins = 0f;
    TextMeshProUGUI coinText;
    // Start is called before the first frame update
    void Start()
    {
       coinText = GameObject.FindWithTag("coinText").GetComponent<TextMeshProUGUI>();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            characterMovement script = other.gameObject.GetComponent<characterMovement>();
            Transform particle = gameObject.transform.Find("particles");
            Vector3 particleSpawn = particle.position;
            Instantiate(script.collectCoin,particleSpawn,Quaternion.identity);
            Destroy(gameObject);
            coins += 1f;
            coinText.text = $"Coins: {coins}";
            PlayerPrefs.SetFloat("Coins", PlayerPrefs.GetFloat("Coins") + 1f);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
