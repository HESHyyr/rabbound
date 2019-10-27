using System;
using UnityEngine;

public class SpeedBuff : Buff
{
    const float MAX_SPEED = 8;
    float multiplier;

    public SpeedBuff(float multiplier = 2) : base(true)
    {
        this.multiplier = multiplier;
    }

    protected override void ApplyBuffTo(Player player)
    {
        player.Speed = Mathf.Min(MAX_SPEED, player.Speed + multiplier);
        player.Invoke("SetOriginalSpeed", 4f);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayBuffAudio();
    }
}
