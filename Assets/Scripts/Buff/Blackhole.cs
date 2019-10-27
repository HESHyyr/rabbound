using System;
public class Blackhole : Buff
{
    public Blackhole() : base(false) { }

    protected override void ApplyBuffTo(Player player)
    {
        player.Die();
    }

    protected override void PlaySoundEffect(Player player)
    {
    }
}