using System;
public class FuelBuff : Buff
{
    public FuelBuff(Player player) : base(player)
    {
    }

    public override void ApplyBuff()
    {
        player.GetFuelTank().RechargeFull();
    }
}