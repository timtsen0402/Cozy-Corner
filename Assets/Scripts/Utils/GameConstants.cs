using UnityEngine;

public static class GameConstants
{
    // Camera Views
    public static readonly CameraView TitleView = new CameraView(new Vector3(336f, 323f, 364f), Quaternion.Euler(25f, -135f, 0f));
    public static readonly CameraView GameView = new CameraView(new Vector3(-33f, 25f, 0), Quaternion.Euler(30f, 90f, 0));
    public static readonly CameraView SettingView = new CameraView(new Vector3(268.3f, -16f, 74.4f), Quaternion.Euler(0, -134.3f, 0));

    // Flag Positions
    public static readonly Vector3 FlagPosOrange = new Vector3(-15f, 0.61f, 15f);
    public static readonly Vector3 FlagPosGreen = new Vector3(15f, 0.61f, 15f);
    public static readonly Vector3 FlagPosBlue = new Vector3(15f, 0.61f, -15f);
    public static readonly Vector3 FlagPosRed = new Vector3(-15f, 0.61f, -15f);
    public static readonly Vector3 FlagPosDefault = new Vector3(0, 0.61f, 0);

    // Dice Position
    public static readonly Vector3 DiceRotatingPos = new Vector3(0, 7f, 25f);
    public static readonly Vector3 DiceRotation = new Vector3(90f, 0, 90f);
    public static readonly Vector3[] DiceSleepingPositions = new Vector3[]
    {
    new Vector3(-18f, 0, 0),
    new Vector3(-18f, 0, 5f),
    new Vector3(-18f, 0, 10f),
    new Vector3(-18f, 0, -5f),
    new Vector3(-18f, 0, -10f)
    };

    // Player Numbers
    public static int TotalPlayers { get; private set; } = 4;

    // Game Settings

    // HexCode Settings
    public const string DEFAULT_HEX_CODE = "#FFFFFF";

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

public enum TeamState
{
    Player,
    AI_Dumb,
    AI_Peaceful,
    AI_Aggressive
}

public enum Difficulty
{
    Dumb,
    Peaceful,
    Aggressive
}
public enum GameMode
{
    Classic,
    Crazy
}