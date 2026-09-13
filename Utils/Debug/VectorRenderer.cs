using Godot;
using System.Collections.Generic;

namespace CarGame.Utils.Debug;

public partial class VectorRenderer : Node3D
{
    private static VectorRenderer _instance;
    private ImmediateMesh _mesh;
    private MeshInstance3D _meshInstance;
    private Material _material;
    
    private struct DebugLine
    {
        public Vector3 Start;
        public Vector3 End;
        public Color Color;
    }
    
    private List<DebugLine> _lines = new List<DebugLine>();
    public static bool IsEnabled = false;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.Keycode == Key.Key0)
            {
                IsEnabled = !IsEnabled;
                if (!IsEnabled && _mesh != null)
                {
                    _mesh.ClearSurfaces();
                    _lines.Clear();
                }
            }
        }
    }

    public static void DrawVector(Vector3 origin, Vector3 vector, Color color, float scale = 1.0f)
    {
        var instance = GetInstance();
        if (instance == null || !IsEnabled) return;

        if (vector.LengthSquared() < 0.0001f) return;
        
        instance._lines.Add(new DebugLine
        {
            Start = origin,
            End = origin + vector * scale,
            Color = color
        });
    }

    private static VectorRenderer GetInstance()
    {
        if (_instance == null)
        {
            var mainLoop = Engine.GetMainLoop() as SceneTree;
            if (mainLoop == null || mainLoop.Root == null) return null;
            
            _instance = new VectorRenderer();
            mainLoop.Root.AddChild(_instance);
            _instance.Name = "DebugVectorRenderer";
        }
        return _instance;
    }

    public override void _Ready()
    {
        _mesh = new ImmediateMesh();
        _meshInstance = new MeshInstance3D();
        _meshInstance.Mesh = _mesh;
        _meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        
        StandardMaterial3D mat = new StandardMaterial3D();
        mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        mat.VertexColorUseAsAlbedo = true;
        mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        _material = mat;
        
        _meshInstance.MaterialOverride = _material;
        AddChild(_meshInstance);
    }

    public override void _Process(double delta)
    {
        _mesh.ClearSurfaces();

        if (_lines.Count > 0)
        {
            _mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

            foreach (var line in _lines)
            {
                _mesh.SurfaceSetColor(line.Color);
                _mesh.SurfaceAddVertex(line.Start);
                _mesh.SurfaceSetColor(line.Color);
                _mesh.SurfaceAddVertex(line.End);
            }

            _mesh.SurfaceEnd();
            _lines.Clear();
        }
    }
    
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}