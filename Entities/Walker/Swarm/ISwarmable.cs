using System;
using Godot;

namespace CarGame.Entities.Enemies.Swarm;

public interface ISwarmable
{
    public event Action<Node3D> OnFreed;
    public Vector3 GetPosition();
    public Transform3D GetGlobalTransform();
}