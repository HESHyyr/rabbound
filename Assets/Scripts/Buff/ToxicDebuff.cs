using System;
using UnityEngine;

public class ToxicDebuff : Buff
{
    const float DRAIN = 30;

    public ToxicDebuff() : base(false) { }

    protected override void ApplyBuffTo(Player player)
    {
        player.GetFuelTank().Drain(DRAIN);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayDebuffAudio();
    }
}