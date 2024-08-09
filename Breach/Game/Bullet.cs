using System;
using System.Net;
using System.Reflection.Metadata;
using BraketsEngine;
using Microsoft.Xna.Framework;

namespace Breach;

public class Bullet : Sprite
{
    public float speed = 350;
    public float xTarget;
    private ParticleEmitter bulletParticles;
    
    private int destroyX = Randomize.IntInRange(100, 126);

    public Bullet(Vector2 pos, float XTarget) : base("s_bullet", pos, "bullet", 1, true)
    {
        this.Position.Y -= 25;
        this.Scale = 0.2f;
        
        this.Tint = Color.Yellow;
        this.xTarget = XTarget;

        bulletParticles = new ParticleEmitter("bulletParticles", new Vector2(0), new ParticleEmitterData
        {
            angleVariance = 360,
            lifeSpanMin = 0.2f,
            lifeSpanMax = 0.4f,
            emitCount = 16,
            sizeStartMin = 4,
            sizeStartMax = 6,
            sizeEndMin = 7,
            sizeEndMax = 8,
            interval = 0.1f,
            speedMin=50,
            speedMax=75,
            colorStart=Color.LightYellow,
            colorEnd=Color.Yellow,
            visible = false
        }, 3);
    }

    public override void Update(float dt)
    {
        this.Position.X -= (speed * dt) * xTarget;
        this.Position.Y -= speed * dt;

        if (this.HitsGet("s_enemy") is Enemy s)
        {
            s.Destroy();
            Globals.GameManager.Points++;
            Destroy();
        }

        if (this.Position.Y <= destroyX)
            Destroy();
    }

    private void Destroy()
    {
        bulletParticles.Position = this.Position;
        bulletParticles.Burst(100);
        this.DestroySelf();
    }
}