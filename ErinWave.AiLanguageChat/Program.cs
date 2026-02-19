using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace ErinWave.AiLanguageChat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string url = "http://localhost:11434/api/generate";
            string model = "gpt-oss:20b";

            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            string systemPromptA = "너는 모험심 넘치는 탐험가 AI, 대답을 모험적이고 호기심 가득하게 한다.";
            string systemPromptB = "너는 논리적이고 호기심 많은 과학자 AI, 사실 기반으로 설명하고 호기심을 자극한다.";

            string conversation = "";
            string botAInput = "안녕! 오늘은 어떤 새로운 것을 발견할 수 있을까?";

            for (int turn = 0; turn < 10; turn++)
            {
                Console.WriteLine($"\n[Bot A]: {botAInput}");
                string botAResponse = await GenerateResponseStream(client, url, model, systemPromptA, conversation + $"\nBot A: {botAInput}");
                conversation += $"\nBot A: {botAResponse}";

                Console.WriteLine($"\n[Bot B]: {botAResponse}");
                string botBResponse = await GenerateResponseStream(client, url, model, systemPromptB, conversation + $"\nBot B:");
                conversation += $"\nBot B: {botBResponse}";

                botAInput = botBResponse;
            }

            Console.WriteLine("\n=== 대화 종료 ===");
        }

        static async Task<string> GenerateResponseStream(HttpClient client, string url, string model, string rolePrompt, string prompt)
        {
            var payload = new
            {
                model = model,
                prompt = rolePrompt + "\n" + prompt,
                stream = true
            };

            string json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            StringBuilder sb = new StringBuilder();
            char[] buffer = new char[1024];

            while (!reader.EndOfStream)
            {
                int read = await reader.ReadAsync(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    string chunk = new string(buffer, 0, read);
                    Console.Write(chunk);
                    sb.Append(chunk);
                }
            }

            Console.WriteLine();
            return sb.ToString().Trim();
        }
    }
}
