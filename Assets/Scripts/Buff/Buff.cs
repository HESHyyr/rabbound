using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Buff
{
    public enum BuffType {
        NoBuff,
        SpeedBuff,
        FuelBuff,
        ToxicDebuff,
        SlowDebuff,
        Blackhole
    }

    protected bool triggered;
    protected bool once;

    public Buff(bool once = true) {
        this.triggered = false;
        this.once = once;
    }

    public void ApplyBuff(Player player) {
        if (once && triggered) return;
        ApplyBuffTo(player);
        PlaySoundEffect(player);
        triggered = true;
    }

    protected abstract void ApplyBuffTo(Player player);

    protected abstract void PlaySoundEffect(Player player);
}
