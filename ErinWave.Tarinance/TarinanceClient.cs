using ErinWave.Tarinance.Models;

using Newtonsoft.Json;

namespace ErinWave.Tarinance
{
    public class TarinanceClient
    {
        private static readonly string TarinanceBaseApiUrl = "https://trn.erin2.xyz/api/";
        private static HttpClient client { get; set; } = new HttpClient();

        public static async Task<T> GetAsync<T>(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonStringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    T? data = JsonConvert.DeserializeObject<T>(jsonStringResult);
                    return data ?? default!;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch
            {
                throw;
            }
        }

        public static IEnumerable<TarinanceCoin> GetNowPrice()
        {
            var result = GetAsync<IEnumerable<TarinanceCoin>>(client, TarinanceBaseApiUrl + "nowprice.php");
            result.Wait();

            return result.Result;
        }
    }
}
