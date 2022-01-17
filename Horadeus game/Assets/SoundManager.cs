using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SoundManager : MonoBehaviour
{
    #region INSPECTOR_VARIABLES
    public AudioSource audioSource;
    [Header("Play when this is bool is true (for animations)")]
    public bool play;
    [Header("Audio Triggers")]
    public bool onInput;
    [Tooltip("The inputs get assigned to the equal clip index. (Ex: Input 0 -> Clip 0)")]
    public KeyCode[] inputs;
    public bool randomClip;
    public bool randomTime;
    public bool areaEnter;
    public bool collision;
    public bool onAwake;
    [Header("Audio Settings")]
    public bool loop;
    [Range(0.0f, 1f)]
    public float spatialBlend;
    [SerializeField]
    public LayerMask interactionLayers;
    [SerializeField]
    public AudioClip[] clips;
    public AudioClip[] randomClips;
    [Header("Random Time Range")]
    public float minTime;
    public float maxTime;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region DEBUG
        if (clips.Length == 0) //checks if the clips array doesn't have elements
        {
            Debug.LogError("CLIP ARRAY IS EMPTY IN GAME OBJECT: " + gameObject.ToString());//displays error
            Debug.Break();//pauses the editor
        }

        if (randomClip)
        {
            if (randomClips.Length == 0) //checks if the random clips array doesn't have elements
            {
                Debug.LogError("RANDOM CLIP ARRAY IS EMPTY IN GAME OBJECT: " + gameObject.ToString());//displays error
                Debug.Break();//pauses the editor
            }
        }
        

        for (int i = 0; i < clips.Length; i++)//checks every clips array element
        {
            if (clips[i] == null)//checks if current array element is null
            {
                Debug.LogError("CLIPS ARRAY ELEMENT " + i.ToString() + " IS EMPTY AT GAMEOBJECT: " + gameObject.ToString());//displays error
                Debug.Break();
            }
        }

        for (int i = 0; i < randomClips.Length; i++)//checks every randomClips array element
        {
            if (randomClips[i] == null)//checks if current array element is null
            {
                Debug.LogError("RANDOM CLIPS ARRAY ELEMENT " + i.ToString() + " IS EMPTY AT GAMEOBJECT: " + gameObject.ToString());//displays error
                Debug.Break();
            }
        }
        #endregion

        if (loop) { audioSource.loop = true; }

        if (onAwake)
        {
            audioSource.playOnAwake = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (onInput)
        {
            PlayOnInput();
        }
    }

    private AudioClip GetRandomClip()
    {
        return randomClips[UnityEngine.Random.Range(0, randomClips.Length)];
    }
    private float GetRandomTimeInRange()
    {
        return UnityEngine.Random.Range(minTime, maxTime);
    }

    private void PlayOnInput()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (Input.GetKeyDown(inputs[i]))
            {
                audioSource.PlayOneShot(clips[i]);
            }
        }
    }

}

