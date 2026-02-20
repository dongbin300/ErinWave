namespace ErinWave.Frame.Raylibs.Scenes
{
	public class SceneManager
	{
		private IScene? _currentScene;

		public IScene? CurrentScene => _currentScene;

		public void ChangeScene(IScene newScene)
		{
			_currentScene?.Exit();
			_currentScene = newScene;
			_currentScene.Enter();
		}

		public void Update(float deltaTime)
		{
			_currentScene?.Update(deltaTime);
		}

		public void Render()
		{
			_currentScene?.Render();
		}
	}
}
