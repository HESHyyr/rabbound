using System;
public class SpeedBuff : Buff
{
    float multiplier;

    public SpeedBuff(float multiplier = 3) : base(true)
    {
        this.multiplier = multiplier;
    }

    protected override void ApplyBuffTo(Player player)
    {
        player.Speed += multiplier;
        player.Invoke("SetOriginalSpeed", 4f);
    }

    protected override void PlaySoundEffect(Player player)
    {
        player.PlayBuffAudio();
    }
}
