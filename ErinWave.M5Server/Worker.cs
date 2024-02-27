namespace ErinWave.M5Server
{
	public class Worker
	{
		Thread? thread;

		public Worker()
		{

		}

		public virtual void Initialize(Action threadAction)
		{
			thread = new Thread(new ThreadStart(threadAction));
		}

		public virtual void Start()
		{
			if (thread?.ThreadState != ThreadState.Running)
			{
				thread?.Start();
			}
		}

		public virtual void Exit()
		{
			if (thread?.ThreadState != ThreadState.Running)
			{
				thread?.Interrupt();
			}
		}
	}
}
