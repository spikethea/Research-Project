using UnityEngine;
public enum LaneTileType
{
    Road,
    Pavement,
    Building
}

public class LaneTile : MonoBehaviour
{
    public float Length = 0f;
    public LaneTileType tileType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {
        Length = GetComponent<Renderer>().bounds.size.z;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
