using System.Collections.Generic;
using UnityEngine;

public class BackgroundElements : MonoBehaviour
{
    [SerializeField] EndlessRunnerEmitter emitter;
    public List<GameObject> elements;
    public float movementFactor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject element in elements)
        {
            element.transform.position +=
                Vector3.back *
                emitter.currentMoveSpeed *
                movementFactor *
                Time.deltaTime;
        }
    }
}
