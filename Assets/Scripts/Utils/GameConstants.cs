using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static List<Team> AllTeams { get; private set; } = new List<Team>();

    // Team
    public const int OrangeNumber = 1;
    public const int GreenNumber = 2;
    public const int BlueNumber = 3;
    public const int RedNumber = 4;

    // Piece
    public static readonly Quaternion PieceRotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));

    // Camera Views
    public static readonly CameraView TitleView = new CameraView(new Vector3(336f, 323f, 364f), Quaternion.Euler(25f, -135f, 0f));
    public static readonly CameraView GameView = new CameraView(new Vector3(-33f, 25f, 0), Quaternion.Euler(37.147f, 90f, 0));
    public static readonly CameraView SettingView = new CameraView(new Vector3(268.3f, -16f, 74.4f), Quaternion.Euler(0, -134.3f, 0));

    // Flag Positions
    public static readonly Vector3 FlagPosOrange = new Vector3(-15f, 0.61f, 15f);
    public static readonly Vector3 FlagPosGreen = new Vector3(15f, 0.61f, 15f);
    public static readonly Vector3 FlagPosBlue = new Vector3(15f, 0.61f, -15f);
    public static readonly Vector3 FlagPosRed = new Vector3(-15f, 0.61f, -15f);
    public static readonly Vector3 FlagPosDefault = new Vector3(0, 0.61f, 0);

    // Dice Settings
    public const float RotatingThreshold1 = 5f;
    public const float RotatingThreshold2 = 10f;
    public const float HeightThreshold = -10f;
    public static readonly Vector3 DiceRotatingPos = new Vector3(0, 7f, 25f);
    public static readonly Vector3 DiceRotation = new Vector3(90f, 0, 90f);
    public static readonly Vector3[] DiceSleepingPositions = new Vector3[]
    {
    new Vector3(-18f, 0, 10f),
    new Vector3(-18f, 0, 5f),
    new Vector3(-18f, 0, 0),
    new Vector3(-18f, 0, -5f),
    new Vector3(-18f, 0, -10f)
    };
    public const float VelocityThreshold = 0.01f;
    public const float AngularVelocityThreshold = 0.01f;
    public const float SettleTime = 0.1f;

    // Player Numbers
    public static int TotalPlayers { get; private set; } = 4;

    // HexCode Settings
    public const string DEFAULT_HEX_CODE = "#FFFFFF";
    public const string ORANGE_HEX_CODE = "#FF8C00";
    public const string GREEN_HEX_CODE = "#228B22";
    public const string BLUE_HEX_CODE = "#1E90FF";
    public const string RED_HEX_CODE = "#CD5C5C";

    // layer
    public static bool IsInHomeLayer(this GameObject go)
    {
        return go.layer == 6;
    }
    public static bool IsInStartLayer(this GameObject go)
    {
        return go.layer == 7;
    }
    public static bool IsInPathLayer(this GameObject go)
    {
        return go.layer == 8;
    }
    public static bool IsInEndLayer(this GameObject go)
    {
        return go.layer == 9;
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
    // for every mode
    Player,
    AI_Dumb,
    // for classic mode
    AI_Peaceful,
    AI_Aggressive,
    // for crazy mode
    AI_Orange,
    AI_Green,
    AI_Blue,
    AI_Red
}

public enum GameMode
{
    Classic,
    Crazy
}

