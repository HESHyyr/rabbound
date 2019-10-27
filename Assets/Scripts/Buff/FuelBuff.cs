using System;
public class FuelBuff : Buff
{
    public override void ApplyBuff(Player player)
    {
        if (triggered) return; 
        player.GetFuelTank().RechargeFull();
        triggered = true;
    }
}