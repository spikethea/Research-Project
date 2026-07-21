using UnityEngine;

[RequireComponent(typeof(GazeEvent))]
public class EventOnlyGaze : Gaze
{
    void Start()
    {
        useEvents = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}