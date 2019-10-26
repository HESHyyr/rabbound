using System;
public class SpeedBuff : Buff
{
    float multiplier;

    public SpeedBuff(Player player, float multiplier) : base(player)
    {
        this.multiplier = multiplier;
    }

    public override void ApplyBuff()
    {
        player.SetSpeed(player.GetSpeed() * multiplier);
    }
}
