using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSFX : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] clips;
    public AudioSource audioSource;

    private Weapon weapon;

    void Start()
    {
        Debug.Log(GetComponent<Player>().currentWeapon.GetType().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        var currW = GetComponent<Player>().currentWeapon.GetType().ToString();
        if (currW == "Bow")
        {
            if (Input.GetMouseButtonDown(0))
            {
                AudioClip clip = clips[0];
                audioSource.PlayOneShot(clip);
            }
            if (Input.GetMouseButtonUp(0))
            {
                audioSource.Stop();
            }
        }
            
    }
}
