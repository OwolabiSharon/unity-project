using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Parallax : MonoBehaviour
{
    public ParallaxType type;
    [Range(0,1000)] public int distance = 150;

    [Range(0, 1)] public float horizontal = 1f;
    [Range(0, 1)] public float vertical  = 0.5f;

    public static bool preview;
    public bool oldPreview;

    public Vector3 originalPosition = Vector3.zero;
    public Vector3 lastPosition = Vector3.zero;
    Vector2 lastCameraPosition;

    bool fixedUp;

    void Start()
    {
        originalPosition = transform.position;
    }

    void FixedUpdate()
    {
        fixedUp = true;
    }

    void LateUpdate()
    {
        if (!fixedUp) return;
        fixedUp = false;

        float factor = 0.001f;
        Vector2 distances;
        if (type == ParallaxType.BACKGROUND)
        {
            distances = (Vector2)Camera.main.transform.position - (Vector2)originalPosition;
        }
        else
        {
            distances = (Vector2)originalPosition - (Vector2)Camera.main.transform.position;
            factor = 0.004f;
        }

        transform.position = originalPosition + new Vector3(distances.x * horizontal,distances.y * vertical,0) * (distance * factor);
    }
    
    void OnDrawGizmos()
    {
        if (oldPreview == true && Parallax.preview == false)
        {
            transform.position = originalPosition;
        }

        if (oldPreview == false && Parallax.preview == true)
        {
            originalPosition = Vector3.zero;
            lastPosition = Vector3.zero;
        }

        oldPreview = Parallax.preview;

        if (preview == false) return;

        if (lastPosition == Vector3.zero && originalPosition == Vector3.zero)
        {
            originalPosition = transform.position;
            lastPosition = originalPosition;
        }

        if (Camera.current != null && Camera.current.transform != null)
        {
            lastCameraPosition = Camera.current.transform.position;
        }

        if (lastPosition != transform.position)
        {
            originalPosition += (lastPosition - transform.position);
        }

        Vector2 distances;
        float factor = 0.001f;
        if(type == ParallaxType.BACKGROUND)
        {
            distances = (Vector2)lastCameraPosition - (Vector2)originalPosition;
        }
        else
        {
            distances = (Vector2)originalPosition - (Vector2)lastCameraPosition;
            factor = 0.004f;
        }

        transform.position = originalPosition + new Vector3(distances.x * horizontal, distances.y * vertical, 0) * (distance * factor);

        lastPosition = transform.position;
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(Parallax))]
public class ParallaxInspector : Editor
{
    float cooldown;

    public override void OnInspectorGUI()
    {
        Parallax par = (Parallax)target;

        displayLabel("");
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"), new GUIContent("Layer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"), new GUIContent("Distance"));
        GUILayout.EndVertical();
        displayLabel("");
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontal"), new GUIContent("Horizontal Factor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vertical"), new GUIContent("Vertical Factor"));
        GUILayout.EndVertical();
        displayLabel("");

        string title = "Enable Preview";
        if(Parallax.preview) title = "Disable Preview";

        if (GUILayout.Button(new GUIContent(title, "Preview all Parallax Effects")))
        {
            Parallax.preview = !Parallax.preview;
        }

        //Updating Sorting Layer:
        if (cooldown <= 0)
        {
            /*
            foreach (SpriteRenderer sr in par.GetComponentsInChildren<SpriteRenderer>())
            {
                if (par.type == ParallaxType.BACKGROUND)
                {
                    sr.sortingLayerName = "Background";
                    sr.sortingOrder = 1000 - par.distance;
                }
                else
                {
                    sr.sortingLayerName = "Foreground";
                    sr.sortingOrder = par.distance;
                }
            }
            */
            cooldown = 0.1f;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void displayLabel(string label)
    {
        GUILayout.BeginHorizontal(); GUILayout.Label(label); GUILayout.EndHorizontal();
    }
    public void displayTitle(string title)
    {
        GUILayout.BeginHorizontal(); EditorGUILayout.LabelField(title, EditorStyles.boldLabel); GUILayout.EndHorizontal();
    }
    public void displayHelpBox(string title)
    {
        GUILayout.BeginHorizontal(); EditorGUILayout.LabelField(title, EditorStyles.helpBox); GUILayout.EndHorizontal();
    }
}
#endif

[System.Serializable]
public enum ParallaxType
{
    BACKGROUND
    ,
    FOREGROUND
}
