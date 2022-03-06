using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarHUD : MonoBehaviour
{
    #region Variables
    private GameObject healthBarSlider; //GameObject Then Grab the Slider Component
    private float timer = 0; // This is so you can lose health over time.
    [Header("How Fast Do You lose health over time")]
    public float waitTime = 30.0f; // How Long Until you lose health again. //30.0f = 3min 20 Sec In RealTime
    #endregion

    #region Built In Methods
    // Start is called before the first frame update
    void Start()
    {
        healthBarSlider = GameObject.Find("HealthBar");
        healthBarSlider.GetComponent<Slider>().value = Game.inst.player.playerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        LoseHealthTimer();  
    }
    #endregion

    #region Custom Methods
    

    /// <summary>
    /// This Takes the Timer waits for it to be reached then takes health from the player from hunger.
    /// </summary>
    private void LoseHealthTimer()
    {
        timer += Time.deltaTime;
        healthBarSlider.GetComponent<Slider>().value = Game.inst.player.playerHealth;
        if (timer >= waitTime)
        {
            Game.inst.player.playerHealth-= 10;
            timer -= waitTime;
        }
    }
    #endregion
}
