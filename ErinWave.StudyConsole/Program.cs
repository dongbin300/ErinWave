namespace ErinWave.StudyConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool[] bools = { true, false, true, true, true, true, false };


            //int count = 0;
            //for(int i = 0; i < bools.Length; i++)
            //{
            //    if (bools[i] == true)
            //    {
            //        count++;
            //    }
            //}

            Console.WriteLine(bools.Count(x => x));
        }
    }
}
