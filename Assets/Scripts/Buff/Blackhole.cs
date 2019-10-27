using System;
public class Blackhole : Buff
{
    public override void ApplyBuff(Player player)
    {
        player.GameOver();
    }
}