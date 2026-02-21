using Raylib_cs;

namespace ErinWave.Frame.Raylibs.Audio
{
	public static class AudioManager
	{
		private static readonly Dictionary<string, Sound> _sounds = [];

		public static void Load(string key, string path)
		{
			if (_sounds.ContainsKey(key))
				return;

			_sounds[key] = Raylib.LoadSound(path);
		}

		public static void Play(string key)
		{
			if (_sounds.TryGetValue(key, out var sound))
			{
				Raylib.PlaySound(sound);
			}
		}

		public static void SetVolume(string key, float volume)
		{
			if (_sounds.TryGetValue(key, out var sound))
			{
				Raylib.SetSoundVolume(sound, volume);
			}
		}

		public static void Dispose()
		{
			foreach (var sound in _sounds.Values)
			{
				Raylib.UnloadSound(sound);
			}

			_sounds.Clear();
		}
	}
}
