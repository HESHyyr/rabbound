using System;
using UnityEngine;

public class SlowDebuff : Buff
{
    const float MIN_SPEED = 0.5f;
    float multiplier;

    public SlowDebuff(float multiplier = 3) : base(false)
    {
        this.multiplier = multiplier;
    }

    protected override void ApplyBuffTo(Player player)
    {

        player.Speed = Mathf.Max(MIN_SPEED, player.Speed - multiplier);
        player.Invoke("SetOriginalSpeed", 4f);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayDebuffAudio();
    }
}
