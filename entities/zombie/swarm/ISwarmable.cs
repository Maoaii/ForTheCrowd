using System;
using Godot;

namespace CarGame.Entities.Enemies.Swarm;

public interface ISwarmable
{
    public event Action<Node3D> OnFreed;
    public Godot.Vector3 GetPosition();
}