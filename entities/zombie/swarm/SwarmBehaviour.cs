using System.Collections.Generic;
using Godot;

namespace CarGame.Entities.Enemies.Swarm;

public class SwarmBehaviour
{
    // --- Constants ---
    private const float SeparationDistance = 1.5f;

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

    public Vector3 GetSeparationDirection(ISwarmable entity, double newTime)
    {
        if (newTime == _currentTime) 
        {
            return _cachedSeparationDirections.TryGetValue(entity, out Vector3 cachedDirection) 
                ? cachedDirection 
                : Vector3.Zero;
        }

        RecalculateSeparationDirections(newTime);

        return _cachedSeparationDirections.TryGetValue(entity, out Vector3 direction) 
            ? direction 
            : Vector3.Zero;
    }

    // --- Private Methods ---
    private void RecalculateSeparationDirections(double newTime)
    {
        _currentTime = newTime;
        _cachedSeparationDirections.Clear();
        _forces.Clear();
        _counts.Clear();

        foreach (var ent in _swarmEntities)
        {
            _forces[ent] = Vector3.Zero; 
            _counts[ent] = 0;
        }

        for (int i = 0; i < _swarmEntities.Count; i++)
        {
            var entity1 = _swarmEntities[i];
            for (int j = i + 1; j < _swarmEntities.Count; j++)
            {
                var entity2 = _swarmEntities[j];
                Vector3 distanceVector = entity1.GetPosition() - entity2.GetPosition();
                
                // Optimize by checking squared distance first to avoid unnecessary Sqrt calculations
                float distanceSq = distanceVector.LengthSquared();
                if (distanceSq >= SeparationDistance * SeparationDistance || distanceSq < 0.000001f) 
                    continue;

                float distance = Mathf.Sqrt(distanceSq);

                Vector3 weighted = distanceVector / (distance * distance);
                _forces[entity1] += weighted;
                _forces[entity2] -= weighted;  
                _counts[entity1]++;
                _counts[entity2]++;
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