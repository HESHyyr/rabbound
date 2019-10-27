using System;
public class SlowDebuff : Buff
{
    float multiplier;

    public SlowDebuff(float multiplier = 2)
    {
        this.multiplier = multiplier;
    }

    public override void ApplyBuff(Player player)
    {
        if (triggered) return;
        player.Speed -= multiplier;
        player.Invoke("SetOriginalSpeed", 4f);
        triggered = true;
    }
}
