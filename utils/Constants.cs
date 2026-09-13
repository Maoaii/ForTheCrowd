using Godot;

public static class Constants
{
    public const int WINDOW_WIDTH = 1920;
    public const int WINDOW_HEIGHT = 1080;
    public const int PIXELATED_WIDTH = 1920 / 4;
    public const int PIXELATED_HEIGHT = 1080 / 4;
    public const bool FULLSCREEN = false;


    public const bool MOUSE_VISIBLE = true;
    public const bool MOUSE_INVERTED = true;
    public const float MOUSE_SENSITIVITY = 0.003f;


    public const float CAMERA_LAG_FACTOR = -12f;
    public const float CAMERA_DISTANCE = 35.0f;
    public const float CAMERA_FOV = Mathf.Pi / 4.0f;
    public const float CAMERA_FOV_KICK = (Mathf.Pi / 4.0f) * 1.03f;
    public const float CAMERA_FOV_KICK_TIME_UP_S = 0.15f;
    public const float CAMERA_FOV_KICK_TIME_DOWN_S = 0.35f;
    public const float CAMERA_NEAR = 0.1f;
    public const float CAMERA_FAR = 1000.0f;
    public const float CAMERA_YAW_MIN = Mathf.Pi / 6.0f;
    public const float CAMERA_YAW_MAX = (Mathf.Pi / 2.0f) - 0.1f;

    public const float LANDING_FOV_KICK = (Mathf.Pi / 4.0f) * 1.03f;
    public const float LANDING_FOV_KICK_TIME_UP_S = 0.05f;
    public const float LANDING_FOV_KICK_TIME_DOWN_S = 0.3f;

    // Tricks
    public const float OLLIE_SCORE = 10.0f;
    public const float KICKFLIP_SCORE = 100.0f;
    public const float BACK_MANUAL_SCORE = 50.0f;
    public const float SHOVE_IT_SCORE = 75.0f;
}