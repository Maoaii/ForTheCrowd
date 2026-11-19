using System;
using Godot;

[Tool]
[GlobalClass]
public partial class AimingModifier : Resource
{
	[Export] public bool InheritOwnerVelocity = true;
	[Export] public bool AimAhead = false;

	[ExportGroup("Direction Variation")]
	[Export] public bool AimDirectionVariation = false;
	[Export(PropertyHint.Range, "0.0,1.0,0.01, suffix:ratio")] public float DirectionVariationRatio = 0.0f;
	[Export(PropertyHint.Range, "0.0,45.0,0.1, suffix:m")] public float DirectionVariationMeters = 0.0f;
}
