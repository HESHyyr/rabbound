using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] Buff.BuffType buffType;
    Buff buff;

    void Awake()
    {
        BuffGenerator generator = new BuffGenerator();
        buff = generator.Type(buffType).GetBuff();
    }

    public Buff GetBuff() {
        return buff;
    }

    public void ApplyBuff(Player player) {
        buff?.ApplyBuff(player);
    }
}
