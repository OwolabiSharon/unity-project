using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class textParticles : MonoBehaviour
{
    public TextMeshProUGUI textPrefab;
    public float lifespan = 1f;
    public float speed = 1f;
    public float spread = 0.5f;
    public Color color = Color.white;
    
    public void EmitText(string text)
    {
        TextMeshProUGUI textMesh = Instantiate(textPrefab);
        textMesh.text = text;
        textMesh.color = color;
        
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Vector3 randomDirection = Random.insideUnitSphere * spread;
        particleSystem.Emit(transform.position, randomDirection.normalized * speed, lifespan, particleSystem.startLifetime, color);
        
        Destroy(textMesh.gameObject, lifespan);
    }
}
