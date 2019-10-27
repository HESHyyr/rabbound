using System;
public class FuelBuff : Buff
{

    public FuelBuff() : base(true) { }

    protected override void ApplyBuffTo(Player player)
    {
        player.GetFuelTank().RechargeFull();
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayBuffAudio();
    }
}