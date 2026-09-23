using CarGame.Systems.Blackboards;
using Godot;
using System;

public partial class SpeedLines : CanvasLayer
{
	[Export] private ColorRect _colorRect;

	private static SpeedLines _instance;
    public static SpeedLines Instance => _instance;

	private static readonly StringName IntensityParam = "intensity";
	private ShaderMaterial _material;

    public override void _Ready()
    {
        base._Ready();
		_instance = this;
		_material = (ShaderMaterial)_colorRect.Material;
		SetIntensity(0.0f);
    }

	public void SetIntensity(float intensity)
	{
		_material.SetShaderParameter(IntensityParam, intensity);
		Visible = intensity > 0.001f;
	}
}
