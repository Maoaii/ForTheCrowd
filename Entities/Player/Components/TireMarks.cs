using Godot;
using CarGame.Systems.Blackboards;
using System.Collections.Generic;

namespace CarGame.Entities.Components;

public struct WheelGroundContact
{
    public Vector3 Position;
    public Vector3 Normal;
    public bool IsGrounded;
    public float DriftFactor;
    public float Age;
    public Color Color;
}

public class TrailSegment
{
    public float GroundOffset = 0.01f;
    public float TextureWorldLength = 3.0f;

    private List<WheelGroundContact> _points = new List<WheelGroundContact>();
    private ImmediateMesh _mesh;
    private MeshInstance3D _meshInstance;
    private float _width;
    private float _maxLifetime;

    public bool IsFullyExpired { get; private set; }

    private float _textureWorldLength;
    private float _groundOffset;

    public TrailSegment(Node3D parent, float width, float maxLifetime, Material material, float groundOffset, float textureWorldLength)
    {
        _width = width;
        _maxLifetime = maxLifetime;
        _groundOffset = groundOffset;
        _textureWorldLength = textureWorldLength;
        
        _mesh = new ImmediateMesh();
        _meshInstance = new MeshInstance3D
        {
            Mesh = _mesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };
        parent.AddChild(_meshInstance);
        _meshInstance.GlobalTransform = Transform3D.Identity;
    }

    public void AddPoint(WheelGroundContact point)
    {
        _points.Add(point);
    }

    public void Update(double delta)
    {
        float dt = (float)delta;
        if (dt > 0.1f)
        {
            dt = 0.1f;
        }

        for (int i = _points.Count - 1; i >= 0; i--)
        {
            var p = _points[i];
            p.Age += dt;
            _points[i] = p;

            if (p.Age >= _maxLifetime)
            {
                _points.RemoveAt(i);
            }
        }

        if (_points.Count < 2)
        {
            IsFullyExpired = true;
            _mesh.ClearSurfaces();
            return;
        }

        IsFullyExpired = false;
        RebuildMesh();
    }

    private void RebuildMesh()
    {
        _mesh.ClearSurfaces();

        if (_points.Count < 2)
        {
            return;
        }

        _mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        int numPoints = _points.Count;
        float cumulativeDistance = 0f;

        Vector3[] leftPositions = new Vector3[numPoints];
        Vector3[] rightPositions = new Vector3[numPoints];
        Color[] colors = new Color[numPoints];
        Vector2[] uvsLeft = new Vector2[numPoints];
        Vector2[] uvsRight = new Vector2[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            if (i > 0)
            {
                cumulativeDistance += _points[i].Position.DistanceTo(_points[i - 1].Position);
            }

            Vector3 dir = GetTravelDirection(i);
            Vector3 side = dir.Cross(_points[i].Normal);
            if (side != Vector3.Zero)
                side = side.Normalized();

            Vector3 basePosition = _points[i].Position + _points[i].Normal * _groundOffset;
            leftPositions[i] = basePosition - side * (_width * 0.5f);
            rightPositions[i] = basePosition + side * (_width * 0.5f);

            float u = cumulativeDistance / _textureWorldLength;
            float ageRatio = Mathf.Clamp(_points[i].Age / _maxLifetime, 0f, 1f);
            
            // Simple EaseOutQuad mapping
            float alpha = 1f - (1f - (1f - ageRatio) * (1f - ageRatio)); 
            
            colors[i] = new Color(_points[i].Color, _points[i].Color.A * alpha);
            
            uvsLeft[i] = new Vector2(u, 0f);
            uvsRight[i] = new Vector2(u, 1f);
        }

        for (int i = 0; i < numPoints - 1; i++)
        {
            // Triangle 1
            _mesh.SurfaceSetColor(colors[i]);
            _mesh.SurfaceSetUV(uvsLeft[i]);
            _mesh.SurfaceAddVertex(leftPositions[i]);

            _mesh.SurfaceSetColor(colors[i + 1]);
            _mesh.SurfaceSetUV(uvsLeft[i + 1]);
            _mesh.SurfaceAddVertex(leftPositions[i + 1]);

            _mesh.SurfaceSetColor(colors[i]);
            _mesh.SurfaceSetUV(uvsRight[i]);
            _mesh.SurfaceAddVertex(rightPositions[i]);

            // Triangle 2
            _mesh.SurfaceSetColor(colors[i]);
            _mesh.SurfaceSetUV(uvsRight[i]);
            _mesh.SurfaceAddVertex(rightPositions[i]);

            _mesh.SurfaceSetColor(colors[i + 1]);
            _mesh.SurfaceSetUV(uvsLeft[i + 1]);
            _mesh.SurfaceAddVertex(leftPositions[i + 1]);

            _mesh.SurfaceSetColor(colors[i + 1]);
            _mesh.SurfaceSetUV(uvsRight[i + 1]);
            _mesh.SurfaceAddVertex(rightPositions[i + 1]);
        }

        _mesh.SurfaceEnd();
    }

    private Vector3 GetTravelDirection(int i)
    {
        Vector3 direction;
        if (i == 0)
            direction = _points[i + 1].Position - _points[i].Position;
        else if (i == _points.Count - 1)
            direction = _points[i].Position - _points[i - 1].Position;
        else
            direction = _points[i + 1].Position - _points[i - 1].Position;

        return direction.Normalized();
    }

    public void Destroy()
    {
        _meshInstance.QueueFree();
    }
}

public partial class TireMarks : Node3D
{
    [Export] public BlackboardComponent Blackboard;
    [ExportGroup("Mesh Properties")]
    [Export] public float GroundOffset = 0.01f;
    [Export] public float TextureWorldLength = 3.0f;
    
    [Export] public Vector3 LocalOffset;

    [ExportGroup("Blood Effect")]
    [Export] public float BloodDuration = 2.0f; 
    [Export] public Color BloodColor = new Color(0.59f, 0.04f, 0.04f); // 150, 10, 10
    
    private List<TrailSegment> _segments = new List<TrailSegment>();
    private TrailSegment _activeSegment; 
    private Vector3 _lastSampledPos;
    private bool _wasEmitting;
    private float _noDriftCooldown;
    private Color _currentColor = Colors.Black;
    private float _bloodTimer = 0f;
    
    private Material _trailMaterial;
    
    public override void _Ready()
    {
        StandardMaterial3D mat = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
        };

        _trailMaterial = mat;

        if (Blackboard != null)
        {
            Blackboard.Events.OnZombieRunOver += TriggerBloodEffect;
        }
    }

    public override void _ExitTree()
    {
        if (Blackboard != null)
        {
            Blackboard.Events.OnZombieRunOver -= TriggerBloodEffect;
        }
    }

    private void TriggerBloodEffect()
    {
        _bloodTimer = BloodDuration;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Blackboard == null) return;
        
        float dt = (float)delta;
        
        bool isDrifting = Blackboard.Kinematics.DriftFactor > Blackboard.MovementConfig.TrailDriftThreshold && Blackboard.Input.MoveInput.X != 0f;

        if (_bloodTimer > 0f)
        {
            _bloodTimer -= dt;
            float t = Mathf.Clamp(_bloodTimer / BloodDuration, 0f, 1f);
            
            Color targetColor = isDrifting ? Colors.Black : new Color(0f, 0f, 0f, 0f);
            _currentColor = targetColor.Lerp(BloodColor, t);
        }
        else
        {
            _currentColor = Colors.Black;
        }

        Vector3 upDir = Vector3.Up; // Or blackboard.Owner.Transform.Basis.Y
        Vector3 wheelWorldPos = Blackboard.Owner.GlobalTransform * LocalOffset;
        
        // Raycast down to find ground
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(wheelWorldPos + Vector3.Up, wheelWorldPos + Vector3.Down * 2.0f);
        var result = spaceState.IntersectRay(query);
        
        Vector3 groundPoint = wheelWorldPos;
        bool isGrounded = Blackboard.State.IsOnGround;
        
        if (result.Count > 0)
        {
            groundPoint = (Vector3)result["position"];
        }
        
        bool shouldEmitTrail = (isDrifting || _bloodTimer > 0f) && isGrounded;

        if (!isGrounded)
        {
            _activeSegment = null;
            _wasEmitting = false;
            _noDriftCooldown = 0f;
        }
        else
        {
            if (shouldEmitTrail)
            {
                if (!_wasEmitting)
                {
                    _activeSegment = new TrailSegment(GetTree().CurrentScene as Node3D ?? this, Blackboard.MovementConfig.TrailWidth, Blackboard.MovementConfig.TrailMaxLifetime, _trailMaterial, GroundOffset, TextureWorldLength);
                    _segments.Add(_activeSegment);

                    _activeSegment.AddPoint(new WheelGroundContact
                    {
                        Position = groundPoint,
                        Normal = upDir,
                        IsGrounded = isGrounded,
                        DriftFactor = Blackboard.Kinematics.DriftFactor,
                        Age = 0f,
                        Color = _currentColor
                    });
                    _lastSampledPos = groundPoint;
                    _wasEmitting = true;
                }
                
                _noDriftCooldown = Blackboard.MovementConfig.TrailCooldown; 
            }
            else if (_wasEmitting)
            {
                _noDriftCooldown -= dt;
                if (_noDriftCooldown <= 0f)
                {
                    _activeSegment = null; 
                    _wasEmitting = false;
                }
            }
        }

        if (_activeSegment != null
            && isGrounded
            && shouldEmitTrail
            && _lastSampledPos.DistanceTo(groundPoint) >= Blackboard.MovementConfig.TrailSampleDistance)
        {
            _activeSegment.AddPoint(new WheelGroundContact
            {
                Position = groundPoint,
                Normal = upDir,
                IsGrounded = isGrounded,
                DriftFactor = Blackboard.Kinematics.DriftFactor,
                Age = 0f,
                Color = _currentColor
            });
            _lastSampledPos = groundPoint;
        }

        for (int i = _segments.Count - 1; i >= 0; i--)
        {
            _segments[i].Update(delta);
            if (_segments[i].IsFullyExpired && _segments[i] != _activeSegment)
            {
                _segments[i].Destroy();
                _segments.RemoveAt(i);
            }
        }
    }
}