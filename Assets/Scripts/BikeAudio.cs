using UnityEngine;

public class BikeAudio : MonoBehaviour
{
    [SerializeField] Bike bike;
    [SerializeField] EndlessRunnerEmitter Emitter;

    [SerializeField] AudioClip WheelClip;
    [SerializeField] AudioSource wheelAudioSource;

    [SerializeField] AudioSource phoneAudioSource;
    [SerializeField] AudioClip notificationSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wheelAudioSource.loop = true;

        wheelAudioSource.clip = WheelClip;
    }

    public void PlayNotificationSound()
    {
        phoneAudioSource.PlayOneShot(notificationSoundClip);
    }

    // Update is called once per frame
    void Update()
    {
        if (!wheelAudioSource.isPlaying)
        {
            wheelAudioSource.Play();
        }

        
        if (Emitter.currentMoveSpeed > 5f)
        {
            wheelAudioSource.volume = Mathf.Clamp(Emitter.currentMoveSpeed / bike.ySpeed, 0f, 0.4f);
            wheelAudioSource.pitch = Mathf.Clamp(Emitter.currentMoveSpeed * 0.1f, 0.5f, 1.5f);
        }
        else {
            wheelAudioSource.volume = 0f;
        }
    }
}
