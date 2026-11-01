using System.Collections.Generic;
using Godot;

namespace CarGame.Entities.Enemies.Swarm;

[GlobalClass]
public partial class SwarmManager : Node3D
{
	// --- Properties ---
	[Export] public ulong CalculateSeparationInterval = 10;
	[Export] public float SeparationDistance = 1.5f;
	[Export(PropertyHint.Layers3DPhysics)]
	public uint TargetLayers { get; set; }

	public int EntityCount => _swarmEntities.Count;

	// --- Fields ---
	private readonly List<ISwarmable> _swarmEntities = new();
	private readonly Dictionary<ISwarmable, Vector3> _cachedSeparationDirections = new();
	private readonly Dictionary<ISwarmable, Vector3> _forces = new();
	private readonly Dictionary<ISwarmable, int> _counts = new();

	private double _currentTime = -1.0;

	// --- Public Methods ---
	public void RegisterEntity(ISwarmable entity)
	{
		if (!_swarmEntities.Contains(entity))
		{
			_swarmEntities.Add(entity);
			entity.OnFreed += HandleOnFreed;
		}
	}

	public void RemoveEntity(ISwarmable entity)
	{
		if (_swarmEntities.Remove(entity))
		{
			_cachedSeparationDirections.Remove(entity);
			_forces.Remove(entity);
			_counts.Remove(entity);
			entity.OnFreed -= HandleOnFreed;
		}
	} 

	public Vector3 GetSeparationDirection(ISwarmable entity)
	{
		return _cachedSeparationDirections.TryGetValue(entity, out Vector3 direction) 
			? direction 
			: Vector3.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.GetPhysicsFrames() % CalculateSeparationInterval == 0)
		{
			RecalculateSeparationDirections();
		}
		else
		{
			foreach (ISwarmable entity in _cachedSeparationDirections.Keys)
			{
				_cachedSeparationDirections[entity] = _cachedSeparationDirections[entity].Lerp(Vector3.Zero, (float)delta * 10.0f);
			}
		}
	}

	// --- Private Methods ---
	private void RecalculateSeparationDirections()
	{
		_cachedSeparationDirections.Clear();
		_forces.Clear();
		_counts.Clear();

		foreach (ISwarmable ent in _swarmEntities)
		{
			_forces[ent] = Vector3.Zero; 
			_counts[ent] = 0;
		}

		SphereShape3D shape = new SphereShape3D { Radius = SeparationDistance};
		PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D
		{
			Shape = shape,
			CollisionMask = (uint)TargetLayers,
			CollideWithAreas = true,
			CollideWithBodies = false
		};

		for (int i = 0; i < _swarmEntities.Count; i++)
		{
			ISwarmable entity1 = _swarmEntities[i];
			query.Transform = entity1.GetGlobalTransform();
			var results = GetWorld3D().DirectSpaceState.IntersectShape(query);
	
			foreach (var result in results)
			{
				if (result["collider"].AsGodotObject() is not ISwarmable entity2 || entity2 == entity1) continue;
				int j = _swarmEntities.IndexOf(entity2);
				if (j <= i) continue;
				
				Vector3 away = entity1.GetPosition() - entity2.GetPosition();

				float dist = away.Length();
				if (dist < SeparationDistance && dist > 0.0f)
				{   
					float strength = 1.0f - dist / SeparationDistance;
					Vector3 pairForce = away / dist * strength;

					_forces[entity1] += pairForce;
					_forces[entity2] -= pairForce;
					_counts[entity1]++;
					_counts[entity2]++;
				}
			}
		}

		foreach (var e in _swarmEntities)
		{
			_cachedSeparationDirections[e] = _counts[e] > 0 
				? _forces[e] / _counts[e] 
				: Vector3.Zero;
		}
	}

	private void HandleOnFreed(Node3D entity)
	{
		if (entity is ISwarmable swarmable)
		{
			RemoveEntity(swarmable);
		}
	}
}
