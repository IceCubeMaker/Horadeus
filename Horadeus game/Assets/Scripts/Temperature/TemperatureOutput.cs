using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureOutput : MonoBehaviour
{
    private float prevTemp = 0;
    public float temp = 0;
    public float radius = 1f;
    private void Update()
    {
        float currentTemp = 1-Mathf.InverseLerp(0, radius, Vector3.Distance(transform.position, Game.inst.player.movement.transform.position));
        Game.inst.player.temperature += currentTemp * temp - prevTemp;
        prevTemp = currentTemp * temp;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void OnDisable()
    {
        Game.inst.player.temperature -= prevTemp;
        prevTemp = 0;
    }
}
