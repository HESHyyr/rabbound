using System;
using UnityEngine;

public class ToxicDebuff : Buff
{
    const float DRAIN = 30;
    public override void ApplyBuff(Player player)
    {
        if (triggered) return;
        player.GetFuelTank().Drain(DRAIN);
        triggered = true;
    }
}