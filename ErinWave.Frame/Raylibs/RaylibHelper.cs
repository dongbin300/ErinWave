
using Raylib_cs;

namespace ErinWave.Frame.Raylibs
{
	public class RaylibHelper
	{
		public static void Init(int screenWidth, int screenHeight, int fps, string title = "", ConfigFlags configFlags = ConfigFlags.ResizableWindow)
		{
			Raylib.SetConfigFlags(configFlags);
			Raylib.InitWindow(screenWidth, screenHeight, title);
			Raylib.SetTargetFPS(fps);
		}
	}
}
