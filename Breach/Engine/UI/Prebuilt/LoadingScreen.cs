using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class LoadingScreen
{
    public static UIScreen loadingScreen;
    public static bool isLoading = false;

    private static TimeSpan minimumLoadingTime = new TimeSpan(0, 0, 2);
    private static DateTime loadStartTime;

    private static bool isInitialized = false;

    public static void Initialize()
    {
        if (isInitialized)
            return;

        loadingScreen = new UIScreen();

        UIText loadingText = new UIText("Loading...");
        loadingText.Tag = "__loading__ui_element";
        loadingText.SetAlign(UIAllign.Center, new Vector2(0));
        loadingText.SetFontSize(36);
        loadingText.Animate(UIAnimation.SmoothFlashing, 750, min: 0.35f);

        loadingScreen.AddElement(loadingText);

        isInitialized = true;
    }

    public static void Show()
    {
        loadingScreen.Show();

        loadStartTime = DateTime.Now;
        isLoading = true;

        Globals.Camera.BackgroundColor = Color.Black;
    }

    public static async Task Hide()
    {
        TimeSpan realLoadingTime = DateTime.Now - loadStartTime;
        TimeSpan waitTime = minimumLoadingTime - realLoadingTime;

        if (waitTime > TimeSpan.Zero)
        {
            await Task.Delay(waitTime);
        }

        loadingScreen.Hide();
        isLoading = false;
    }
}