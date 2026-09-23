using CarGame.Systems.Blackboards;
using Godot;
using System;

public partial class CircularProgressBar : MeshInstance3D
{
	[Export(PropertyHint.Range, "0,1,0.01")] private float _showBarTime;
	[Export(PropertyHint.Range, "0,1,0.01")] private float _hideBarTime;
	[Export(PropertyHint.Range, "0,1,0.01")] private float _barDisappearTime;
	[Export(PropertyHint.Range, "0,1,0.1")] private float _yellowThreshold;
	[Export(PropertyHint.Range, "0,1,0.1")] private float _redThreshold;
 	 
	[ExportGroup("References")]
	[Export] private BlackboardComponent _blackboard;
	[Export] private TextureProgressBar _progressBar;


	private Tween _showTween;
	private bool _depleted = false;
	private bool _barVisible = false;
	public override void _Ready()
	{
		_progressBar.Value = _progressBar.MaxValue;
		_progressBar.Modulate = _progressBar.Modulate with { A = 0.0f };
	}

	public override void _Process(double delta)
	{
		float fuelFraction = _blackboard.Kinematics.BoostFuelFraction;
		
		bool shouldShowBar = fuelFraction < 1.0f;
		if (_barVisible != shouldShowBar)
		{
			_barVisible = shouldShowBar;

			_showTween?.Kill();
			_showTween = CreateTween();
			
			if (!_barVisible)
				_showTween.TweenInterval(_barDisappearTime);
			
			_showTween.TweenProperty(
				_progressBar,
				"modulate:a",
				shouldShowBar ? 1.0f : 0.0f,
				shouldShowBar ? _showBarTime : _hideBarTime
			);
		}

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
