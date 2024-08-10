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
using System.Threading.Tasks;

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


    string backgroundMusicName = "background";

    UIScreen gameHud = new UIScreen();
    UIText healthText, pointsText, heatText, currentPowerupText;
    UIImage healthImage, heatImage; 
    public UIImage pointsImage;

    UIScreen gameOverHud = new UIScreen();
    UIText gameOverText, gameOverScoreText, countinueText;

    UIScreen mainMenu = new UIScreen();
    UIText titleText, startText;
    bool isOnMainMenu = false;

    public void Start()
    {
        Globals.Camera.BackgroundColor = Color.Black;

        Globals.DEBUG_Overlay = false;
        isOnMainMenu = true;

        titleText = new UIText("Breach");
        titleText.SetAlign(UIAllign.Center, new Vector2(0));
        titleText.SetFontSize(62);

        startText = new UIText("Press Enter to Play!");
        startText.SetAlign(UIAllign.BottomCenter, new Vector2(0, 36));
        startText.SetFont("NeorisRegular");
        startText.SetFontSize(32);
        startText.Opacity = 0.75f;
        startText.Animate(UIAnimation.SmoothFlashing, 750, min: 0.35f);

        mainMenu.AddElements([
            titleText, startText
        ]);
    }

    private async void StartGame()
    {
        LoadingScreen.Initialize();
        LoadingScreen.Show();
        
        Health = 100;
        Points = 0;
        Wave = 0;
        GameOver = false;

        Globals.GameManager = this;
        Globals.Camera.Center();

        // Load all the UI
        healthText = new UIText("100");
        healthText.SetFontSize(22);
        healthText.SetFont("NeorisBold");
        healthText.SetAlign(UIAllign.TopLeft, new Vector2(48, 8));

        healthImage = new UIImage("ui/hearth");
        healthImage.SetAlign(UIAllign.TopLeft, new Vector2(10));
        healthImage.Size = new Vector2(32);

        heatText = new UIText("0");
        heatText.SetFontSize(22);
        heatText.SetFont("NeorisBold");
        heatText.SetAlign(UIAllign.TopLeft, new Vector2(128, 8));

        heatImage = new UIImage("ui/heat");
        heatImage.SetAlign(UIAllign.TopLeft, new Vector2(96, 10));
        heatImage.Size = new Vector2(32);

        pointsText = new UIText("0");
        pointsText.SetFontSize(22);
        pointsText.SetFont("NeorisBold");
        pointsText.SetAlign(UIAllign.TopLeft, new Vector2(198, 8));

        pointsImage = new UIImage("ui/points");
        pointsImage.SetAlign(UIAllign.TopLeft, new Vector2(166, 10));
        pointsImage.Size = new Vector2(32);

        currentPowerupText = new UIText();
        currentPowerupText.SetAlign(UIAllign.Center, new Vector2(0));
        currentPowerupText.SetFontSize(56);
        currentPowerupText.Opacity = 0.35f;
        currentPowerupText.visible = false;

        gameHud.AddElements([
            healthText, healthImage,
            heatText, heatImage,
            pointsText, pointsImage,
            currentPowerupText
        ]);

        gameOverText = new UIText("Game Over!");
        gameOverText.SetAlign(UIAllign.Center, new Vector2(0, -15));
        gameOverText.SetFontSize(56);
    
        gameOverScoreText = new UIText("Score: 0");
        gameOverScoreText.SetAlign(UIAllign.Center, new Vector2(0, 35));
        gameOverScoreText.SetFontSize(46);
        gameOverScoreText.Opacity = 0.75f;

        countinueText = new UIText("Press Enter to Try Again!");
        countinueText.SetAlign(UIAllign.BottomCenter, new Vector2(0, 36));
        countinueText.SetFont("NeorisRegular");
        countinueText.SetFontSize(32);
        countinueText.Opacity = 0.75f;
        countinueText.Animate(UIAnimation.SmoothFlashing, 750, min: 0.35f);

        gameOverHud.AddElements([
            gameOverText, gameOverScoreText,
            countinueText
        ]);
        gameOverHud.Hide();

        // Other loading (level, songs, etc)
        ContentLoader.Load(ContentType.Level, "level0");

        var song = ContentLoader.GetSong(backgroundMusicName);
        song.Play();
        song.SetVolume(5);

        enemies = new List<Enemy>();
        player = new Player(new Vector2(Globals.APP_Width / 2, Globals.APP_Height - 50));
        damageIndicator = new DamageIndicator();

        isOnMainMenu = false;
        await LoadingScreen.Hide();
    }

    public void Update(float dt)
    {
        if (isOnMainMenu)
        {
            if (Input.IsPressed(Keys.Enter))
            {                
                mainMenu.Unload();
                StartGame();
            }
            return;
        }

        if (GameOver)
        {
            ContentLoader.GetSong(backgroundMusicName).Stop();
            gameHud.Hide();

            gameOverHud.Show();
            gameOverScoreText.Text = $"Score: {Points}";

            if (Input.IsPressed(Keys.Enter))
            {
                Globals.ENGINE_Main.Sprites.Clear();
                
                gameHud.Unload();
                gameOverHud.Unload();

                ParticleManager.UnloadAll();
                ContentLoader.UnloadLevel("level0");

                StartGame();
            }
            return;
        }

        if (Input.IsDown(Keys.LeftAlt) && Input.IsPressed(Keys.S))
        {
            backgroundMusicName = "background2";
            ContentLoader.GetSong(backgroundMusicName).Play();
        }

        if (Health <= 0)
            GameOver = true;

        if (Health <= 50)
            if (!healthImage.animationRunning)
                healthImage.Animate(UIAnimation.ZoomInOut, Health * 5, min:1, max:1.2f);

        if (Wave >= 5)
            if (!heatImage.animationRunning)
                heatImage.Animate(UIAnimation.ZoomInOut, 250, min:1, max:1.1f);

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

        healthText.Text = Health.ToString();
        pointsText.Text = Points.ToString();
        heatText.Text = Wave.ToString();

        if (player.currentPowerup != "none" && _powerupUiTimer > 0)
        {
            currentPowerupText.Text = player.currentPowerup;
            currentPowerupText.visible = true;
        }
        else 
        {
            _powerupUiTimer = 2;
            player.currentPowerup = "none";
            currentPowerupText.visible = false;
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