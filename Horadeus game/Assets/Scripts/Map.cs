using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public static Map inst;

    public List<Fish> fishes;

    private Timer fishSpawnTimer;

    private int fishCounter = 0;

    public void Init() {
        inst = this;

        fishSpawnTimer = new Timer(1f);
    }

    public void InternalUpdate() {
        if(fishSpawnTimer & fishCounter < 20) {
            fishSpawnTimer.AddFromNow();

            SpawnFish();
            fishCounter++;
        }
    }

    private void SpawnFish() {
        Vector2 circlePos = Random.insideUnitCircle * 20f;
        Vector3 pos = new Vector3(circlePos.x, 5f ,circlePos.y);

        Fish fish = MPool.Get<Fish>();
        fish.transform.position = pos;
    }

    public void DestroyFish(Fish fish) {
        fishes.Remove(fish);
        Destroy(fish.gameObject);
    }

}
