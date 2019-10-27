using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] Buff.BuffType buffType;
    Buff buff;
    GameObject wave;

    void Awake()
    {
        BuffGenerator generator = new BuffGenerator();
        buff = generator.Type(buffType).GetBuff();
        wave = GameObject.Find("Wave");
    }

    void Update()
    {
        if (wave.GetComponent<WaveMovement>().IsBehindWave(transform.position, 10.0f))
            Destroy(gameObject);
    }

    public Buff GetBuff() {
        return buff;
    }

    public void ApplyBuff(Player player) {
        buff?.ApplyBuff(player);
    }
}
