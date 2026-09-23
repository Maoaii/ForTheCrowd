using CarGame.Systems.Blackboards;
using Godot;
using System;

public partial class FuelProgressBar : MeshInstance3D
{
	[Export(PropertyHint.Range, "0,1,0.01")] private float _showBarTime;
	[Export(PropertyHint.Range, "0,1,0.01")] private float _hideBarTime;
	[Export(PropertyHint.Range, "0,1,0.01")] private float _barDisappearTime;
	[Export(PropertyHint.Range, "0,1,0.1")] private float _yellowThreshold;
	[Export(PropertyHint.Range, "0,1,0.1")] private float _redThreshold;
 	 
	[ExportGroup("References")]
	[Export] private BlackboardComponent _blackboard;
	[Export] private TextureProgressBar _progressBar;

	private bool _depleted = false;
	public override void _Ready()
	{
		_progressBar.Value = _progressBar.MaxValue;
	}

	public override void _Process(double delta)
	{
		float fuelFraction = _blackboard.Kinematics.BoostFuelFraction;

		if (!_depleted)
			_depleted = fuelFraction <= 0;

		if (_depleted && fuelFraction > _blackboard.MovementConfig.BoostRestartThreshold)
			_depleted = false;

		if (_depleted)
			_progressBar.TintProgress = Colors.DimGray;
		else 
			if (fuelFraction < _redThreshold)
				_progressBar.TintProgress = Colors.Red;
			else if (fuelFraction < _yellowThreshold)
				_progressBar.TintProgress = Colors.Yellow;
			else
				_progressBar.TintProgress = Colors.Green;

		_progressBar.Value = fuelFraction * _progressBar.MaxValue;
	}
}
