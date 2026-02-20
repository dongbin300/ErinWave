using ErinWave.Frame.Raylibs.Entities;

namespace ErinWave.Frame.Raylibs.Systems
{
	public class SpawnSystem<T>(Func<float, float> intervalFunc, Func<T> factory) where T : EntityBase
	{
		private float elapsedTime = 0f;
		private float spawnTimer = 0f;

		public T? Update(float delta)
		{
			elapsedTime += delta;
			spawnTimer += delta;

			float currentInterval = intervalFunc(elapsedTime);

			if (spawnTimer >= currentInterval)
			{
				spawnTimer = 0f;
				return factory();
			}

			return null;
		}
	}
}
