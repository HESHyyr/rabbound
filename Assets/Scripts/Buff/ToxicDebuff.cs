using System;
using UnityEngine;

public class ToxicDebuff : Buff
{
    const float DRAIN = 30;
    public override void ApplyBuff(Player player)
    {
        player.GetFuelTank().Drain(DRAIN);
    }
}