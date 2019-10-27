using System;
public class FuelBuff : Buff
{
    const float RECHARGE_TIME = 1;

    public FuelBuff() : base(true) { }

    protected override void ApplyBuffTo(Player player)
    {
        player.RechargeOvertime(RECHARGE_TIME);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayBuffAudio();
    }
}