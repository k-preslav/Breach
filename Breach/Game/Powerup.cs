using System.Linq;
using BraketsEngine;
using Microsoft.Xna.Framework;

namespace Breach;

public class Powerup : Sprite
{
    private float _speed = 100;
    
    private float _effectTimer = 10;
    private bool _effectRunning = false;

    private Player player;

    private bool effectedShooter = false;
    private bool effectedSpeed = false;
    private bool effectedShooterSpeed = false;

    private ParticleEmitter activateParticles;
    private ParticleEmitter trailParticles;

    public Powerup(Vector2 pos) : base("powerup", pos, "powerup", 3, true)
    {
        this.Scale = 0.5f;
        
        this.activateParticles = new ParticleEmitter("activatePowerupParticles", new Vector2(0), new ParticleEmitterData{
            angleVariance = 360,
            lifeSpanMin = 0.4f,
            lifeSpanMax = 1.2f,
            emitCount = 64,
            sizeStartMin = 12,
            sizeStartMax = 16,
            sizeEndMin = 20,
            sizeEndMax = 24,
            interval = 0.05f,
            speedMin=100,
            speedMax=350,
            colorStart = Color.LightGreen,
            colorEnd = Color.LimeGreen,
            visible = false
        }, 3);

        this.trailParticles = new ParticleEmitter("powerupTrailParticles", new Vector2(0), new ParticleEmitterData{
            angleVariance = 8,
            lifeSpanMin = 0.6f,
            lifeSpanMax = 1.4f,
            emitCount = 8,
            sizeStartMin = 12,
            sizeStartMax = 18,
            sizeEndMin = 6,
            sizeEndMax = 8,
            interval = 0.15f,
            speedMin=15,
            speedMax=45,
            colorStart = Color.LightGreen,
            colorEnd = Color.LimeGreen,
            visible = true,
        }, 2);
    }

    public override void Update(float dt)
    {
        this.Position.Y += _speed * dt;
        trailParticles.Position = this.Position;

        if (this.Position.Y >= 520)
        {
            this.visible = false;
            trailParticles.SetVisible(false);
        }

        if (this.visible && this.HitsGet("s_player") is Player p)
        {
            _effectRunning = true;
            visible = false;
            player = p;

            if (Globals.GameManager.areSoundsOn)
                ContentLoader.GetSound("powerup").Play(50);
            
            Globals.Camera.Shake(1.5f, 0.1f);

            trailParticles.SetVisible(false);

            activateParticles.Position = this.Position;
            activateParticles.Burst(100);

            int randomEffect = Randomize.IntInRange(1, 6);
            if (randomEffect == 1) RandomizeShooter();
            else if (randomEffect == 2) RandomizeSpeed();
            else if (randomEffect == 3) RandomizeShootSpeed();
            else if (randomEffect == 4) RestoreHealth();
            else if (randomEffect == 5) KillAll();
        }

        if (_effectRunning)
        {
            _effectTimer -= dt;
            if (_effectTimer <= 0)
            {
                _effectRunning = false;
                if (effectedShooter) ShooterReset();
                else if (effectedSpeed) SpeedReset();
                else if (effectedShooterSpeed) ShootSpeedReset();

                this.DestroySelf();
            }
        }
    }

    private void RandomizeShooter()
    {
        effectedShooter = true;

        player.shooters = Randomize.IntInRange(2, 6);
        this._effectTimer = 10 - player.shooters;

        player.currentPowerup = "More Bullets!";
    }
    private void ShooterReset()
    {
        effectedShooter = false;
        player.shooters = 1;
    }

    private void RandomizeSpeed()
    {
        effectedSpeed = true;

        player.speed = Randomize.IntInRange(350, 500);
        this._effectTimer = 10;   

        player.currentPowerup = "More Speed!";
    }
    private void SpeedReset()
    {
        effectedSpeed = false;
        player.speed = 300;
    }

    private void RandomizeShootSpeed()
    {
        effectedShooterSpeed = true;

        player.shootCooldown = Randomize.FloatInRange(0.06f, 0.03f);
        this._effectTimer = 6;

        player.currentPowerup = "More Bullet Speed!";
    }
    private void ShootSpeedReset()
    {
        effectedShooterSpeed = false;
        player.shootCooldown = 0.15f;
    }

    private void RestoreHealth()
    {
        Globals.GameManager.Health += 35;
        player.currentPowerup = "+35 Health";
    }

    private void KillAll()
    {
        Globals.GameManager.Points += 3;
        foreach (var enemy in Globals.GameManager.enemies.ToList())
        {
            enemy.Destroy();
        }
        player.currentPowerup = "Kill Theam All!";
    }
}