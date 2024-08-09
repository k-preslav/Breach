using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BraketsEngine;

public class Main : Game
{
    public List<Sprite> Sprites;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private GameManager _gameManager;
    private DebugUI _debugUi;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "PreloadContent";
        IsMouseVisible = true;

        this.Exiting += OnExit;
    }

    protected override void Initialize()
    {
        Debug.Log("Calling Initialize()", this);

        _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
        _gameManager = new GameManager();

        Globals.LOAD_APP_P();
        IsFixedTimeStep = false;

        Debug.Log("Applying application properties...", this);
        Window.Title = Globals.APP_Title;
        _graphics.PreferredBackBufferWidth = Globals.APP_Width;
        _graphics.PreferredBackBufferHeight = Globals.APP_Height;
        _graphics.SynchronizeWithVerticalRetrace = Globals.APP_VSync;
        _graphics.ApplyChanges();

        Globals.ENGINE_Main = this;
        Globals.ENGINE_GraphicsDevice = _graphics.GraphicsDevice;
        Globals.ENGINE_SpriteBatch = _spriteBatch;

        _debugUi = new DebugUI();
        _debugUi.Initialize(this);

        if (Debugger.IsAttached)
            Globals.DEBUG_Overlay = true;

        new Camera(Vector2.Zero);

        this.Sprites = new List<Sprite>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Debug.Log("Loading content...", this);
        _gameManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Input.GetState();

        if (Input.IsPressed(Keys.F3))
            Globals.DEBUG_Overlay = !Globals.DEBUG_Overlay;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Globals.DEBUG_DT = dt;
        Globals.DEBUG_FPS = 1 / dt;

        _gameManager.Update(dt);
        Globals.Camera.CalculateMatrix();

        if (Globals.STATUS_Loading)
            return;
        
        ParticleManager.Update();
        foreach (var sp in Sprites.ToList())
        {
            sp.Update(dt);
            sp.UpdateRect();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue); 
        
        // ------- Game Layer -------
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Globals.Camera.TranslationMatrix);
        
        if (Globals.STATUS_Loading)
        {
            _spriteBatch.End();
            return;   
        };

        var sortedSprites = Sprites.OrderBy(sp => sp.Layer).ToList();
        foreach (var sp in sortedSprites)
        {
            sp.Draw();
        }
        _spriteBatch.End();
        
        // ------- UI Layer ------- 
        _spriteBatch.Begin();
        _gameManager.TempRendrer();
        _debugUi.DrawOverlay(_spriteBatch, 0.25f);
        _debugUi.DrawWindows(gameTime);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    public void AddSprite(Sprite sp) => Sprites.Add(sp);
    public void RemoveSprite(Sprite sp) => Sprites.Remove(sp);

    private void OnExit(object sender, EventArgs e)
    {
        Debug.Log("Calling OnExit()");
        _gameManager.Stop();
    }
}
