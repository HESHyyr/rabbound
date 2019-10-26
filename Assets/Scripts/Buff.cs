using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Buff
{
    protected Player player;

    public Buff(Player player) {
        this.player = player;
    }

    public abstract void ApplyBuff();
}
