using UnityEngine;

public class Pavement : MonoBehaviour
{
    public bool isPalmTree = false;
    
    [SerializeField] public GameObject palmTree;
    [SerializeField] public GameObject[] commuterPrefabs;

    private bool commutersSpawned = false;

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

                GameObject commuter = Instantiate(commuterPrefab, spawnPosition, randomRotation);

                commuter.transform.SetParent(transform, true);

                commutersSpawned = true;
            }
        }
    }
}
