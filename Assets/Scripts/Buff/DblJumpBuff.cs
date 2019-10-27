using System;
public class DblJumpBuff : Buff
{
    public override void ApplyBuff(Player player)
    {
        if (triggered) return;
        player.EnableDoubleJump();
        triggered = true;
    }
}