using ErinWave.DirectEx.Enums;
using ErinWave.DirectEx.Objects;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace ErinWave.DirectEx
{
	public class GameManager
	{
		static WindowRenderTarget renderTarget = default!;
		static Random random = new();
		public static List<GameObject> Objects = [];
		public static GameObject Me => Objects.Find(x => x.Type == GameObjectType.Me) ?? default!;

		public static void Init(WindowRenderTarget renderTarget)
		{
			GameManager.renderTarget = renderTarget;
			InitializeGameObjects();
		}

		public static void InitializeGameObjects()
		{
			Objects.Add(new GameObject(GameObjectType.Me, 30, 70, 24, 24, 10f));
			Objects.Add(new GameObject(GameObjectType.Wall, 0, 200, 1000, 50));
			Objects.Add(new GameObject(GameObjectType.Wall, 80, 150, 1000, 50));
			Objects.Add(new GameObject(GameObjectType.Wall, 160, 100, 1000, 50));
		}

		public static void Render()
		{
			foreach (var obj in Objects)
			{
				if (obj.Type != GameObjectType.Wall)
				{
					obj.NextFrame();
				}

				foreach (var otherObj in Objects)
				{
					obj.ProcessCollision(otherObj);
				}
				FillGameObject(obj);
			}

			renderTarget.DrawText($"VX: {Me.Velocity.X}\nVY: {Me.Velocity.Y}",
				new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Verdana", 12),
				new RawRectangleF(0, 0, 200, 100), new SolidColorBrush(renderTarget, new RawColor4(1, 1, 1, 1)));
		}

		private static void FillGameObject(GameObject obj)
		{
			var color = new RawColor4(1, 1, 1, 1);
			var roundedRectangle = new RoundedRectangle()
			{
				RadiusX = 4,
				RadiusY = 4,
				Rect = new RawRectangleF(obj.Position.X, obj.Position.Y, obj.Position.X + obj.Width, obj.Position.Y + obj.Height)
			};
			var brush = new SolidColorBrush(renderTarget, color);
			renderTarget.FillRoundedRectangle(roundedRectangle, brush);
		}
	}
}
