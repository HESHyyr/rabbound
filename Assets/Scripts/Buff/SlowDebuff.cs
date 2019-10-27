using System;
public class SlowDebuff : Buff
{
    float multiplier;

    public SlowDebuff(float multiplier = 2) : base(false)
    {
        this.multiplier = multiplier;
    }

    protected override void ApplyBuffTo(Player player)
    {
        player.Speed -= multiplier;
        player.Invoke("SetOriginalSpeed", 4f);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayDebuffAudio();
    }
}
