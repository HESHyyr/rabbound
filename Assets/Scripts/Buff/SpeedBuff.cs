using System;
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
        player.Speed += multiplier;
        player.Invoke("SetOriginalSpeed", 4f);
        triggered = true;
    }
}
