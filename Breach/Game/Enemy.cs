using System;
using System.Threading.Tasks;
using BraketsEngine;
using Microsoft.Xna.Framework;

namespace Breach;

public class Enemy : Sprite
{
    float Speed;
    bool hasAttacked = false;
    private ParticleEmitter hitParticles;

    public Enemy(Vector2 pos, float speed) : base("s_enemy", pos, "enemy", 1, true)
    {
        this.Speed = speed;
        this.Scale = 0.5f;

        hitParticles = new ParticleEmitter("hitParticles", new Vector2(0), new ParticleEmitterData{
            angleVariance = 360,
            lifeSpanMin = 0.4f,
            lifeSpanMax = 1.2f,
            emitCount = 16,
            sizeStartMin = 12,
            sizeStartMax = 16,
            sizeEndMin = 20,
            sizeEndMax = 24,
            interval = 0.05f,
            speedMin=100,
            speedMax=175,
            colorStart = Color.Red,
            colorEnd = Color.DarkRed,
            visible = false
        }, 2);
    }

    public override void Update(float dt)
    {
        this.Position.Y += Speed * dt;
        
        if (Globals.GameManager.GameOver)
            return;


        if (this.Position.Y >= 500)
        {
            if (!this.hasAttacked) Globals.GameManager.Health -= 10;
            this.Attack();
            Globals.Camera.Shake(7f, 0.2f);
        }
        else if (this.Hits("s_player"))
        {
            if (!this.hasAttacked) Globals.GameManager.Health -= 25;
            this.Attack();
            Globals.Camera.Shake(10f, 0.2f);
        }
    }

    private async void Attack()
    {
        this.Destroy();

        Globals.GameManager.damageIndicator.SetTargetOpacity(1, 7);
        await Task.Delay(175);
        Globals.GameManager.damageIndicator.SetTargetOpacity(0, 7);

        hitParticles.SetEnable(false);
    }

    public void Destroy()
    {
        // Get random destroy sound
        if (Globals.GameManager.areSoundsOn)
            ContentLoader.GetSound("enemy_destroyed").Play(100);

        // Destroy
        this.visible = false;
        hasAttacked = true;

        hitParticles.Burst(125);
        hitParticles.Position = this.Position;

        Globals.GameManager.enemies.Remove(this);
        this.DestroySelf();
    }
}