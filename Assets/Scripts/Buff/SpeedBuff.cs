﻿using System;
public class SpeedBuff : Buff
{
    float multiplier;

    public SpeedBuff(float multiplier = 3)
    {
        this.multiplier = multiplier;
    }

    public override void ApplyBuff(Player player)
    {
        if (triggered) return;
        player.SetSpeed(player.GetSpeed() + multiplier);
        player.Invoke("SetOriginalSpeed", 6f);
        triggered = true;
    }
}