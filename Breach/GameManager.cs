using System;
using System.Diagnostics.Contracts;
using BraketsEngine;
using ImGuiNET;
using Breach;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using FontStashSharp;
using System.Threading;
using System.Linq;

public class GameManager
{
    public int Health;
    public int Points;
    public int Wave;
    public bool GameOver = false;
    public DamageIndicator damageIndicator;

    Player player;
    
    public List<Enemy> enemies;
    float enemySpeed = 100;
    float enemySpawnCooldown = 2;
    float _enemySpawnTimer = 0;

    
    List<Powerup> powerUps = new List<Powerup>();
    float powerUpSpawnChanceCooldown = 1f;
    float _powerupSpawnTimer = 0;
    float _powerupUiTimer = 2;

    string backgroundMusic = "background";

    public void Start()
    {
        Health = 100;
        Points = 0;
        Wave = 0;
        GameOver = false;

        Globals.GameManager = this;
        Globals.Camera.Center();

        ContentLoader.Load(ContentType.Level, "level0");

        var song = ContentLoader.GetSong(backgroundMusic);
        song.Play();
        song.SetVolume(5);

        enemies = new List<Enemy>();
        player = new Player(new Vector2(Globals.APP_Width / 2, Globals.APP_Height - 50));
        damageIndicator = new DamageIndicator();
    }

    public void Update(float dt)
    {
        if (GameOver)
        {
            if (Input.IsPressed(Keys.Enter))
            {
                Globals.ENGINE_Main.Sprites.Clear();

                ParticleManager.UnloadAll();
                ContentLoader.UnloadLevel("level0");

                Start();
            }
            return;
        }

        if (Input.IsDown(Keys.LeftAlt) && Input.IsPressed(Keys.S))
        {
            backgroundMusic = "background2";
            ContentLoader.GetSong(backgroundMusic).Play();
        }

        if (Health <= 0)
            GameOver = true;

        _powerupUiTimer -= dt;

        _enemySpawnTimer -= dt;
        if (_enemySpawnTimer <= 0)
        {
            SpawnEnemies();
            _enemySpawnTimer = enemySpawnCooldown;
        }

        _powerupSpawnTimer -= dt;
        if (_powerupSpawnTimer <= 0)
        {
            SpawnPowerUp();
            _powerupSpawnTimer = powerUpSpawnChanceCooldown;
        }

        if (Points < 5)
        {
            enemySpeed = 75;
            enemySpawnCooldown = 2.5f;
            Wave = 0;
        }
        else if (Points < 10)
        {
            enemySpeed = 100;
            enemySpawnCooldown = 2;
            Wave = 1;
        }
        else if (Points < 25)
        {
            enemySpeed = 125;
            enemySpawnCooldown = 1.8f;
            Wave = 2;
        }
        else if (Points < 35)
        {
            enemySpeed = 150;
            enemySpawnCooldown = 1.65f;
            Wave = 3;
        }
        else if (Points < 50)
        {
            enemySpeed = 175;
            enemySpawnCooldown = 1.45f;
            Wave = 4;
        }           
        else if (Points < 75)
        {
            enemySpeed = 200;
            enemySpawnCooldown = 1;
            Wave = 5;
        }
        else if (Points < 100)
        {
            enemySpeed = 250;
            enemySpawnCooldown = 0.5f;
            Wave = 6;
        }
    }

    internal void TempRendrer() // TODO: REMOVE THIS! THIS IS TEMPORARY
    {
        if (GameOver)
        {
            Globals.ENGINE_SpriteBatch.DrawString(
                ContentLoader.GetFont("NeorisMedium", 72), $"Game Over! \n     Score {Points}", 
                new Vector2(100, 200), Color.White
            );
            return;
        }
        
        Globals.ENGINE_SpriteBatch.DrawString(
            ContentLoader.GetFont("NeorisMedium", 24), $"HP: {Health}", 
            new Vector2(250, 10), Color.White
        );
        Globals.ENGINE_SpriteBatch.DrawString(
            ContentLoader.GetFont("NeorisMedium", 24), $"Points: {Points}", 
            new Vector2(325, 10), Color.White
        );
        Globals.ENGINE_SpriteBatch.DrawString(
            ContentLoader.GetFont("NeorisMedium", 24), $"Wave: {Wave}", 
            new Vector2(425, 10), Color.White
        );

        if (player.currentPowerup != "none" && _powerupUiTimer > 0)
        {
            Globals.ENGINE_SpriteBatch.DrawString(
                ContentLoader.GetFont("NeorisMedium", 56), $"{player.currentPowerup}", 
                new Vector2(256 - ContentLoader.GetFont("NeorisMedium", 56).MeasureString(player.currentPowerup).X / 2, 256), Color.DimGray
            );
        }
        else 
        {
            _powerupUiTimer = 2;
            player.currentPowerup = "none";
        }
    }

    internal void Stop()
    {
        
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < 8; i++)
        {
            int spawnChance = new Random().Next(0, 10);
            
            if(spawnChance < 3)
                enemies.Add(new Enemy(new Vector2((64 * i) + 32, new Random().Next(-100, -10)), enemySpeed));
        }
    }
    public void SpawnPowerUp()
    {
            int spawnChance = new Random().Next(0, 10);
            
            if(spawnChance == 3)
                powerUps.Add(new Powerup(new Vector2(Randomize.IntInRange(10, 500), -10)));
    }
}