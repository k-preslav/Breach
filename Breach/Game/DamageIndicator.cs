using System;
using BraketsEngine;
using Microsoft.Xna.Framework;

namespace Breach;

public class DamageIndicator : Sprite
{
    float targetOpacity;
    float lerpAmmount;

    public DamageIndicator() : base("s_damage_indicator", new Vector2(0), "damage_indicator", 2, true)
    {
        this.Position = new Vector2(256, 256);
        this.Opacity = 0;
    }

    public void SetTargetOpacity(float opacity, float lerpAmmount)
    {
        this.targetOpacity = opacity;
        this.lerpAmmount = lerpAmmount;
    }

    public override void Update(float dt)
    {
        this.Opacity = Single.Lerp(this.Opacity, this.targetOpacity, this.lerpAmmount * dt);
        
        base.Update(dt);
    }
}
