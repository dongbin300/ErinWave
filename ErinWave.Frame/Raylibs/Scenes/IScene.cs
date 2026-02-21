namespace ErinWave.Frame.Raylibs.Scenes
{
	public interface IScene
	{
		void Initialize();
		void Enter();
		void Exit();
		void Update(float deltaTime);
		void Render();
	}
}
