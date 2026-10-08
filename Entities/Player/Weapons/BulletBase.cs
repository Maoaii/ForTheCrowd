using Godot;
using System;

public abstract partial class BulletBase : Area3D
{
    public abstract void SetDirection(Vector3 direction, float speed);

    public abstract void SetLifetime(float lifetime);
}
