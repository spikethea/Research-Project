using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Pavement : MonoBehaviour
{
    public bool isPalmTree = false;
    
    [SerializeField] public GameObject palmTree;
    [SerializeField] public GameObject[] commuterPrefabs;

    private List<GameObject> commuters = new List<GameObject>();
    public bool commutersSpawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hazard") || other.CompareTag("Consumable")) {
            Destroy(other.gameObject);
        }
    }

    public void MoveCommutersWithTile(Vector3 movement)
    {
        //transform.position += movement;

        foreach (GameObject commuter in commuters)
        {
            if (commuter != null)
                commuter.transform.position += movement;
        }
    }

    public void clearCommuters()
    {
        foreach (GameObject commuter in commuters)
        {
            if (commuter != null)
                Destroy(commuter);
            commutersSpawned = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!palmTree.activeSelf && isPalmTree)
        {
            palmTree.SetActive(true);
        }
        if (!isPalmTree && !commutersSpawned)
        {
            int numOfCommuters = Random.Range(1, 4);
            for (int i = 0; i < numOfCommuters; i++)
            {
                // Instantiate a random commuter prefab at the position of the pavement
                GameObject commuterPrefab = commuterPrefabs[Random.Range(0, commuterPrefabs.Length)];

                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-4.5f, 4.5f), 0, Random.Range(-25f, 25f));
                
                float randomYRotation = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);

                GameObject commuter = Instantiate(
                    commuterPrefab,
                    spawnPosition,
                    randomRotation
                );

                commuters.Add(commuter);

            }

            commutersSpawned = true;
        }
    }
}
