using UnityEngine;

public class FoodBag : MonoBehaviour
{
    [SerializeField] Transform ChildMesh;
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChildMesh = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.Sin(Time.time * 2f) * 0.5f;
        ChildMesh.localPosition = new Vector3(ChildMesh.localPosition.x, y, ChildMesh.localPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            audioSource.PlayOneShot(audioClip);
            Invoke(nameof(DestroySelf),5f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
