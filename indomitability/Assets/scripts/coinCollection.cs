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
    void Update()
    {
        GameObject coinTextObject = GameObject.FindWithTag("coinText");
        if (coinTextObject) {
            coinText = coinTextObject.GetComponent<TextMeshProUGUI>();
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            characterMovement script = other.gameObject.GetComponent<characterMovement>();
            Transform particle = gameObject.transform.Find("particles");
            Vector3 particleSpawn = particle.position;
            Instantiate(script.collectCoin,particleSpawn,Quaternion.identity);
            Destroy(gameObject);
            script.coins += 1f;
            coins = script.coins;
            coinText.text = $"Coins: {coins}";
            PlayerPrefs.SetFloat("Coins", PlayerPrefs.GetFloat("Coins") + 1f);
        }
        
    }
}
