using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BraketsEngine;

public static class Globals
{
    #region ENGINE SYSTEMS
    public static GraphicsDevice ENGINE_GraphicsDevice;
    public static SpriteBatch ENGINE_SpriteBatch;
    public static Main ENGINE_Main;
    #endregion

    #region APPLICATION PROPERTIES
    public static string APP_Title = "Breach";
    public static string APP_Version = "indev 0.0.6";
    public static int APP_Width = 512;
    public static int APP_Height = 512;
    public static bool APP_VSync = true;
    public static void LOAD_APP_P()
    {
        Debug.Log("[GLOBALS] Loading application properties...");
        //TODO: Load application properties from JSON File
    }
    #endregion

    #region APPLICATION STATUS
    public static bool STATUS_Loading = false;
    #endregion

    #region GAME
    public static Camera Camera;
    public static GameManager GameManager;
    #endregion

    #region DEBUG
    public static float DEBUG_FPS = 0;
    public static float DEBUG_DT = 0;
    public static bool DEBUG_Overlay;
    public static DebugUI DEBUG_UI;
    #endregion
}