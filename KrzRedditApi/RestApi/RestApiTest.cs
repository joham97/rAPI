using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi
{
    public class RestApiTest
    {

        private string local_host_address;

        public RestApiTest(string local_host_address)
        {
            this.local_host_address = local_host_address;
        }

        static async Task<string> Get(string url)
        {
            using (var client = new HttpClient())
            {
                using (var r = await client.GetAsync(new Uri(url)))
                {
                    string result = await r.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }

        public async Task<string> Get(string path, Dictionary<string, string> parameters)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var restpath = local_host_address + path;

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        KeyValuePair<string, string> pair = parameters.ElementAt(i);
                        restpath += ((i == 0)?"?":"&") + pair.Key + "=" + pair.Value;
                    }
                }
                var response_message = await client.GetAsync(restpath).ConfigureAwait(false); ;

                var response = await response_message.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response_message.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new Exception("Request failed");
                }
            }
        }

        public async Task<string> Post<T>(string path, Dictionary<string, string> parameters, T data)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(data);

                var restpath = local_host_address + path;

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        KeyValuePair<string, string> pair = parameters.ElementAt(i);
                        restpath += ((i == 0) ? "?" : "&") + pair.Key + "=" + pair.Value;
                    }
                }

                var response_message = await client.PostAsync(restpath, new StringContent(json, Encoding.UTF8, "application/json"));

                var response = await response_message.Content.ReadAsStringAsync();

                if (response_message.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new Exception("Request failed");
                }
            }
        }

        public async Task<string> Put<T>(string path, Dictionary<string, string> parameters, T data)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(data);

                var restpath = local_host_address + path;

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        KeyValuePair<string, string> pair = parameters.ElementAt(i);
                        restpath += ((i == 0) ? "?" : "&") + pair.Key + "=" + pair.Value;
                    }
                }

                var response_message = await client.PutAsync(restpath, new StringContent(json, Encoding.UTF8, "application/json"));

                var response = await response_message.Content.ReadAsStringAsync();

                if (response_message.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new Exception("Request failed");
                }
            }
        }

    }
}
