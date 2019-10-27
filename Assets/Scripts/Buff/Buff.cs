﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Buff
{
    public enum BuffType {
        NoBuff,
        SpeedBuff,
        FuelBuff,
        ToxicDebuff,
        DblJumpBuff
    }

    protected bool triggered;

    public abstract void ApplyBuff(Player player);
}
