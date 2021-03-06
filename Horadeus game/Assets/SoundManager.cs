using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class SoundManager : MonoBehaviour
{
    #region INSPECTOR_VARIABLES
    public AudioSource audioSource;
    [Tooltip("Used by collision trigger")]
    public Rigidbody rigidBody;
    public Collider collider;

    public bool playOnTrueIsOn;
    [System.Serializable]
    public struct PlayClip
    {
        [Tooltip("Play when this is bool is true")]
        public bool play;
        public int playClip;
    }
    [Tooltip("Aimed at animations")]
    public PlayClip[] playOnTrue;

    [Header("Audio Triggers")]
    public bool onInput;

    [System.Serializable]
    public struct InClip
    {
        public KeyCode input;
        public int inClip;
        [Tooltip("Check to use random clips")]
        public bool clipsOrRandom;
    }

    [SerializeField]
    public InClip[] inClips;

    [Header("")]

    public bool areaEnter;
    [System.Serializable]
    public struct OnAreaEnter
    {
        public LayerMask areaInteractionLayer;
        public int areaClip;
        public bool clipsOrRandom;
    }
    public OnAreaEnter[] onAreaEnterClips;
    [Header("")]

    public bool onCollision;
    [System.Serializable]
    public struct OnCollision
    {
        public int areaClip;
        public bool clipsOrRandom;
    }
    public OnCollision[] onCollisionClips;

    [Header("")]

    public bool onAwake;
    [Header("Audio Settings")]
    public bool loop;
    [Range(0.0f, 1f)]
    public float spatialBlend;
    [SerializeField]
    public AudioClip[] clips;
    public AudioClip[] randomClips;
    [Header("Random Time Range")]
    public float minTime;
    public float maxTime;

    [Tooltip("This is used only by On Awake, Area Enter and collision triggers")]
    public bool randomClip;
    public bool randomTime;

    private bool isInArea;
    private bool hasCollided;
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

        if (playOnTrueIsOn)
        {
            PlayOnTrue();
        }
        if (areaEnter)
        {
            _OnTriggerEnter();
        }
    }

    private AudioClip GetRandomClip()
    {
        return randomClips[UnityEngine.Random.Range(0, randomClips.Length)];
    }//Returns a random clip from the RandomClips array

    private float GetRandomTimeInRange()
    {
        return UnityEngine.Random.Range(minTime, maxTime);
    }//Returns a random number

    private void PlayOnInput()
    {
        for (int i = 0; i < inClips.Length; i++)//checks every array element
        {

            if (inClips[i].clipsOrRandom)//checks if random clips is enabled for the current input setting
            {
                if (Input.GetKeyDown(inClips[i].input))//plays a random clip when the input is pressed
                {
                    audioSource.PlayOneShot(GetRandomClip());
                }
            }
            else
            {
                if (Input.GetKeyDown(inClips[i].input))//plays the indicated clip from the Clips array
                {
                    audioSource.PlayOneShot(clips[inClips[i].inClip]);
                }
            }
                
        }
    }//plays a clip or randomclip when a specific input is pressed

    private void PlayOnTrue()
    {
        for (int i = 0; i < playOnTrue.Length; i++)
        {
            if (randomClip)
            {
                audioSource.PlayOneShot(GetRandomClip());
            }
            else
            {
                audioSource.PlayOneShot(clips[playOnTrue[i].playClip]);
            }
        }
    }//just plays a clip or randomclip when the play bool is true
    
    private void _OnTriggerEnter()
    {
        for (int i = 0; i < onAreaEnterClips.Length; i++)
        {
            if (isInArea)
            {
                if (onAreaEnterClips[i].clipsOrRandom)
                {
                    audioSource.PlayOneShot(GetRandomClip());
                }
                else
                {
                    audioSource.PlayOneShot(clips[onAreaEnterClips[i].areaClip]);
                }
            }
        }
    }

    private void _OnCollisionEnter()
    {

    }

    public void OnTriggerEnter()
    {
        isInArea = true;
    }
    public void OnTriggerExit()
    {
        isInArea = false;
    }

    public void OnCollisionEnter()
    {
        hasCollided = true;
    }
}

