using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BraketsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breach;

public class Player : Sprite
{
    public string currentPowerup = "none";

    public int shooters = 1;
    public float shooterSpread = 0.3f;

    public float speed = 300;

    bool canShoot = true;
    public float shootCooldown = 0.15f;
    float _shootTimer;
    private ParticleEmitter shootParticles;

    public Player(Vector2 pos, bool auto_load=true) : base("s_player", pos, "player", 1, auto_load)
    {
        this.Scale = 1f;

        shootParticles = new ParticleEmitter("shootParticles", new Vector2(0), new ParticleEmitterData{
            angleVariance = 8,
            lifeSpanMin = 0.2f,
            lifeSpanMax = 1f,
            emitCount = 32,
            sizeStartMin = 4,
            sizeStartMax = 6,
            sizeEndMin = 12,
            sizeEndMax = 18,
            interval = 0.1f,
            speedMin=100,
            speedMax=225,
            visible = false
        }, 2);
    }

    public override void Update(float dt)
    {
        if (Globals.GameManager.GameOver)
        {
            this.visible = false;
            return;
        }

        this.Position = Vector2.Clamp(this.Position, new Vector2(33), new Vector2(476));

        MouseState mouseState = Mouse.GetState();
        if ((mouseState.LeftButton == ButtonState.Pressed || Input.IsDown(Keys.Space)) && canShoot)
        {
            for (int i = 0; i < shooters; i++)
            {
                float angle = (i - (shooters - 1) / 2f) * shooterSpread;
                new Bullet(this.Position, angle);
            }
         
            canShoot = false;
            ContentLoader.GetSound("shoot").Play(volume: 10);

            shootParticles.Position = new Vector2(Position.X, Position.Y - 35);
            shootParticles.Burst(100);
        }

        _shootTimer -= dt;
        if (_shootTimer <= 0)
        {
            _shootTimer = shootCooldown;
            canShoot = true;

        }

        // Movement
        Vector2 dir = Vector2.Zero;
        if (Input.IsDown(Keys.Left) || Input.IsDown(Keys.A))
        {
            dir.X -= 1;
            _effects = SpriteEffects.FlipHorizontally;
        }
        if (Input.IsDown(Keys.Right) || Input.IsDown(Keys.D))
        {
            dir.X += 1;
            _effects = SpriteEffects.None;
        }

        this.Position += dir * speed * dt;
    }
}