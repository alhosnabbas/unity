using UnityEngine;
using System.Collections;
public class ExplosionSound : MonoBehaviour
{
    public AudioClip explosionClip; // Assign in Inspector
    public AudioSource explosionAudioSource; // Assign in Inspector
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.enabled = true; // Ensure the AudioSource is enabled
        audio.transform.parent = null; // Detach from the rocket
        explosionAudioSource.clip = explosionClip; // Assign the clip to the AudioSource
        explosionAudioSource.Play(); // Play the clip using the AudioSource
//        Debug.Log(audio.clip.length);
        Destroy(gameObject, audio.clip.length);

    }


}
