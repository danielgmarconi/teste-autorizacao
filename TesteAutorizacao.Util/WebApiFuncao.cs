using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using TesteAutorizacao.Model.Response;

namespace TesteAutorizacao.Util
{
    public class WebApiFuncao
    {
        public class HeaderSet
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public HeaderSet(string name, string value)
            { 
                Name = name;
                Value = value;
            }
        }

        public static MetodoResponse Post(IHttpClientFactory httpClientFactory, string url, object data = null, List<HeaderSet> headerList = null)
        {
            HttpClient client = httpClientFactory.CreateClient(string.Empty);
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            if(headerList != null)
                foreach(var header in headerList)
                    message.Headers.Add(header.Name, header.Value);
            message.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(url);
            HttpResponseMessage response = client.SendAsync(message).Result;
            return JsonConvert.DeserializeObject<MetodoResponse>(response.Content.ReadAsStringAsync().Result);

        }
    }
}
   