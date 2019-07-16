using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Threading;

using Newtonsoft.Json;

using CoxAutomotive.ProgrammingChallenge.Models;

namespace CoxAutomotive.ProgrammingChallenge
{
    class Program
    {
        private static readonly string API_BASE_URL = "http://api.coxauto-interview.com/api";
        private static readonly HttpClient client;
        private static readonly ConcurrentQueue<VehicleResponse> vehicleResponses;
        private static readonly ConcurrentDictionary<int, DealersResponse> dealerResponses;
        private static readonly SemaphoreSlim semaphore;

        static Program()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("aapplication/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Cox Automotive ProgrammingChallenge");

            vehicleResponses = new ConcurrentQueue<VehicleResponse>();
            dealerResponses = new ConcurrentDictionary<int, DealersResponse>();
            semaphore = new SemaphoreSlim(1, 1);
        }

        public static async Task Main(string[] args)
        { 
            Console.WriteLine("Getting dealers and vehicles ...");

            Stopwatch stopwatch = Stopwatch.StartNew();
            var datasetIdResponse = await ApiGetAsync<DatasetIdResponse>($"{API_BASE_URL}/datasetId");
            stopwatch.Stop();
            Console.WriteLine($"DatasetId: {datasetIdResponse.DatasetId}");
            Console.WriteLine($"*** Time Elapsed: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            var vehicleIdsResponse = await ApiGetAsync<VehicleIdsResponse>($"{API_BASE_URL}/{datasetIdResponse.DatasetId}/vehicles");
            stopwatch.Stop();
            Console.WriteLine($"VehicleIds: {vehicleIdsResponse.ToJson()}");
            Console.WriteLine($"*** Time Elapsed: {stopwatch.ElapsedMilliseconds} ms");

            
            Console.WriteLine($"Getting vehicles and dealers ...");
            stopwatch.Restart();
            ProcessParallel(datasetIdResponse.DatasetId, vehicleIdsResponse);
            stopwatch.Stop();
            Console.WriteLine($"*** Time Elapsed: {stopwatch.ElapsedMilliseconds} ms");

            var answer = AnswerFromResponse();

            var answerResponse = await ApiPostAnswerAsync(datasetIdResponse.DatasetId, answer);
            Console.WriteLine($"AnswerResponse: {answerResponse.ToJson()}");
        }

        private static async Task VehicleAndDealerAsync(string datasetId, int vehicleId)
        {
            var vehicleResponse = await ApiGetAsync<VehicleResponse>($"{API_BASE_URL}/{datasetId}/vehicles/{vehicleId}");
            vehicleResponses.Enqueue(vehicleResponse);
            Console.WriteLine($"VehicleResponse: {vehicleResponse.ToJson()}");

            await semaphore.WaitAsync();
            try
            {
                if (vehicleResponse.DealerId.HasValue && !dealerResponses.ContainsKey(vehicleResponse.DealerId.Value))
                {
                    var dealersResponse = await ApiGetAsync<DealersResponse>($"{API_BASE_URL}/{datasetId}/dealers/{vehicleResponse.DealerId.Value}");
                    Console.WriteLine($"DealersResponse: {dealersResponse.ToJson()}");
                    dealerResponses.TryAdd(vehicleResponse.DealerId.Value, dealersResponse);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static void ProcessParallel(string datasetId, VehicleIdsResponse vehicleIdsResponse)
        {
            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new ConcurrentQueue<Exception>();

            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(vehicleIdsResponse.VehicleIds, vehicleId =>
            {
                if (vehicleId.HasValue)
                {
                    try
                    {
                        VehicleAndDealerAsync(datasetId, vehicleId.Value).Wait();
                    }
                    // Store the exception and continue with the loop.                    
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            });

            // Throw the exceptions here after the loop completes.
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        private static Answer AnswerFromResponse()
        {
            var dealers = dealerResponses.ToDictionary(
                kv => kv.Key,
                kv => new DealerAnswer(kv.Value.DealerId, kv.Value.Name, new List<VehicleAnswer>()));
            foreach(var vehicleResponse in vehicleResponses)
            {
                if (vehicleResponse.DealerId.HasValue)
                {
                    DealerAnswer dealerAnswer;
                    dealers.TryGetValue(vehicleResponse.DealerId.Value, out dealerAnswer);
                    if (dealerAnswer != null)
                    {
                        dealerAnswer.Vehicles.Add(
                            new VehicleAnswer(
                                vehicleResponse.VehicleId,
                                vehicleResponse.Year,
                                vehicleResponse.Make,
                                vehicleResponse.Model));
                    }
                }
            }

            return new Answer(dealers.Values.ToList());
        }

        private static async Task<AnswerResponse> ApiPostAnswerAsync(string datasetId, Answer answer)
        {
            var answerJson = answer.ToJson();
            var httpContent = new StringContent(answerJson, Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync($"{API_BASE_URL}/{datasetId}/answer", httpContent))
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode == false)
                {
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }

                return JsonConvert.DeserializeObject<AnswerResponse>(content);
            }
        }

        private static async Task<TResponse> ApiGetAsync<TResponse>(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await client.SendAsync(request))
            {

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode == false)
                {
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }

                return JsonConvert.DeserializeObject<TResponse>(content);
               
            }
        }
    }

    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }
}
