using System.Collections.Generic;
using Godot;

namespace CarGame.Utils.Debug;

/// <summary>
/// Immediate-mode debug drawing in world space. Everything drawn is shown for one frame.
/// Flat shapes (rings, discs, areas) lie on the world XZ plane.
/// </summary>
public partial class VectorRenderer : Node3D
{
	private const int RING_SEGMENTS = 64;
	private const int DASH_ARC_SEGMENTS = 4;
	private const float DASH_LENGTH = 0.25f;
	private const float ARROW_HEAD_SIZE = 0.3f;
	private const float ARROW_HEAD_ANGLE = 0.5f;
	private const float TICK_SIZE = 0.5f;
	private const float FILL_ALPHA = 0.4f;
	private const int MIN_DASHES = 8;

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

	private struct DebugTriangle
	{
		public Vector3 A;
		public Vector3 B;
		public Vector3 C;
		public Color Color;
	}

	private List<DebugLine> _lines = new List<DebugLine>();
	private List<DebugTriangle> _triangles = new List<DebugTriangle>();
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
					_triangles.Clear();
				}
			}
		}
	}

	public static void DrawVector(Vector3 origin, Vector3 vector, Color color, float scale = 1.0f)
	{
		if (vector.LengthSquared() < 0.0001f) return;
		AddLine(origin, origin + vector * scale, color);
	}

	public static void DrawLine(Vector3 from, Vector3 to, Color color, bool dashed = false)
	{
		if (!dashed)
		{
			AddLine(from, to, color);
			return;
		}

		float length = from.DistanceTo(to);
		if (length <= 0f) return;

		Vector3 direction = (to - from) / length;
		for (float t = 0f; t < length; t += DASH_LENGTH * 2f)
			AddLine(from + direction * t, from + direction * Mathf.Min(t + DASH_LENGTH, length), color);
	}

	public static void DrawArrow(Vector3 from, Vector3 to, Color color, bool dashed = false)
	{
		DrawLine(from, to, color, dashed);

		Vector3 direction = from.DirectionTo(to);
		if (direction == Vector3.Zero) return;

		// Spread the head around the up axis; fall back to another axis for (near-)vertical arrows.
		Vector3 axis = Mathf.Abs(direction.Y) > 0.99f ? Vector3.Right : Vector3.Up;
		Vector3 back = -direction * ARROW_HEAD_SIZE;
		AddLine(to, to + back.Rotated(axis, ARROW_HEAD_ANGLE), color);
		AddLine(to, to + back.Rotated(axis, -ARROW_HEAD_ANGLE), color);
	}

	public static void DrawCircle(Vector3 center, float radius, Color color, bool dashed = false)
	{
		if (radius <= 0f) return;

		if (!dashed)
		{
			DrawArc(center, radius, 0f, Mathf.Tau, RING_SEGMENTS, color);
			return;
		}

		int dashes = Mathf.Max(MIN_DASHES, Mathf.RoundToInt(Mathf.Tau * radius / (DASH_LENGTH * 2f)));
		float step = Mathf.Tau / dashes;
		for (int i = 0; i < dashes; i++)
			DrawArc(center, radius, i * step, i * step + step * 0.5f, DASH_ARC_SEGMENTS, color);
	}

	/// <summary>Two concentric circles marking a min/max distance band.</summary>
	public static void DrawBand(Vector3 center, float min, float max, Color color, bool dashed = false)
	{
		DrawCircle(center, min, color, dashed);
		DrawCircle(center, max, color, dashed);
	}

	/// <summary>Lines at ±min and ±max along X, each spanning ±halfLength along Z.</summary>
	public static void DrawHorizontalBand(Vector3 center, float min, float max, float halfLength, Color color, bool dashed = false)
	{
		foreach (float distance in new[] { min, max })
		{
			if (distance <= 0f) continue;
			foreach (float sign in new[] { -1f, 1f })
			{
				Vector3 x = center + new Vector3(sign * distance, 0f, 0f);
				DrawLine(x + new Vector3(0f, 0f, -halfLength), x + new Vector3(0f, 0f, halfLength), color, dashed);
			}
		}
	}

	public static void DrawTick(Vector3 at, Color color) =>
		AddLine(at + new Vector3(0f, 0f, -TICK_SIZE * 0.5f), at + new Vector3(0f, 0f, TICK_SIZE * 0.5f), color);

	/// <summary>Translucent filled circle.</summary>
	public static void DrawDisc(Vector3 center, float radius, Color color)
	{
		if (radius <= 0f) return;

		Color fill = Fill(color);
		float step = Mathf.Tau / RING_SEGMENTS;
		for (int i = 0; i < RING_SEGMENTS; i++)
			AddTriangle(center, center + OnPlane(i * step, radius), center + OnPlane((i + 1) * step, radius), fill);
	}

	/// <summary>Translucent filled rectangle from <paramref name="corner"/>; <paramref name="size"/> is (x, z).</summary>
	public static void DrawArea(Vector3 corner, Vector2 size, Color color)
	{
		Color fill = Fill(color);
		Vector3 a = corner;
		Vector3 b = corner + new Vector3(size.X, 0f, 0f);
		Vector3 c = corner + new Vector3(size.X, 0f, size.Y);
		Vector3 d = corner + new Vector3(0f, 0f, size.Y);
		AddTriangle(a, b, c, fill);
		AddTriangle(a, c, d, fill);
	}

	private static void DrawArc(Vector3 center, float radius, float startAngle, float endAngle, int segments, Color color)
	{
		float step = (endAngle - startAngle) / segments;
		Vector3 previous = center + OnPlane(startAngle, radius);
		for (int i = 1; i <= segments; i++)
		{
			Vector3 next = center + OnPlane(startAngle + i * step, radius);
			AddLine(previous, next, color);
			previous = next;
		}
	}

	private static Vector3 OnPlane(float angle, float radius) =>
		new(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

	private static Color Fill(Color color)
	{
		color.A *= FILL_ALPHA;
		return color;
	}

	private static void AddLine(Vector3 start, Vector3 end, Color color)
	{
		// Create the instance before checking IsEnabled: it's the node that listens for the toggle key.
		var instance = GetInstance();
		if (instance == null || !IsEnabled) return;

		instance._lines.Add(new DebugLine { Start = start, End = end, Color = color });
	}

	private static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
	{
		var instance = GetInstance();
		if (instance == null || !IsEnabled) return;

		instance._triangles.Add(new DebugTriangle { A = a, B = b, C = c, Color = color });
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
		mat.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		_material = mat;

		_meshInstance.MaterialOverride = _material;
		AddChild(_meshInstance);
	}

	public override void _Process(double delta)
	{
		_mesh.ClearSurfaces();

		if (_triangles.Count > 0)
		{
			_mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

			foreach (var triangle in _triangles)
			{
				_mesh.SurfaceSetColor(triangle.Color);
				_mesh.SurfaceAddVertex(triangle.A);
				_mesh.SurfaceSetColor(triangle.Color);
				_mesh.SurfaceAddVertex(triangle.B);
				_mesh.SurfaceSetColor(triangle.Color);
				_mesh.SurfaceAddVertex(triangle.C);
			}

			_mesh.SurfaceEnd();
			_triangles.Clear();
		}

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
