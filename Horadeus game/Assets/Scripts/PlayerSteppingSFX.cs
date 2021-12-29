using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteppingSFX : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] clips;

    private AudioSource audioSource;

    public bool playSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlaySound();
    }


    private void PlaySound()
    {
        if (playSound == true)
        {
            AudioClip clip = GetRandomClip();
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetRandomClip()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
}
