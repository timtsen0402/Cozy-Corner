using UnityEngine;

public static class GameConstants
{
    // Camera Views
    public static readonly CameraView TitleView = new CameraView(new Vector3(336f, 323f, 364f), Quaternion.Euler(25f, -135f, 0f));
    public static readonly CameraView GameView = new CameraView(new Vector3(-33f, 25f, 0), Quaternion.Euler(30f, 90f, 0));
    public static readonly CameraView SettingView = new CameraView(new Vector3(268.3f, -16f, 74.4f), Quaternion.Euler(0, -134.3f, 0));

    // Player Numbers
    public static int TotalPlayers { get; private set; } = 4;

    // Game Settings

    // HexCode Settings


    // Initialize method to be called at the start of the game
    public static void Initialize()
    {
        // Load any dynamic values here, if needed
    }
}

// Keep the CameraView struct definition
public struct CameraView
{
    public Vector3 Position;
    public Quaternion Rotation;

    public CameraView(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}