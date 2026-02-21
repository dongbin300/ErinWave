namespace ErinWave.Frame.Raylibs.Scenes
{
	public abstract class SceneBase : IScene
	{
		protected SceneBase()
		{
			Initialize();
		}

		public void Enter() => OnEnter();
		public void Exit() => OnExit();
		public void Update(float deltaTime) => OnUpdate(deltaTime);
		public void Render() => OnRender();

		void IScene.Initialize() => Initialize();

		protected abstract void OnEnter();
		protected abstract void OnExit();
		protected abstract void OnUpdate(float deltaTime);
		protected abstract void OnRender();
		protected abstract void Initialize();
	}
}
