using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace BraketsEngine;

public class Input
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;

    public static KeyboardState GetState()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        return currentKeyState;
    }

    public static bool IsDown(Keys key)
    {
        return currentKeyState.IsKeyDown(key);
    }

    public static bool IsPressed(Keys key)
    {
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }

    public static Vector2 GetMousePosition()
    {
        Point p = Mouse.GetState().Position;
        return new Vector2(p.X, p.Y);
    }
}