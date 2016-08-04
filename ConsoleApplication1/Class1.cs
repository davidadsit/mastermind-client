using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PS.Http;

namespace ConsoleApplication1
{
    public interface IJsonHttp
    {
        T Get<T>(string url);
        T Post<T>(string url, object body);
    }

    public class JsonHttp : IJsonHttp
    {
        readonly IHttpClient httpClient;
        readonly IMessageSerializer messageSerializer;

        public JsonHttp(IHttpClient httpClient, IMessageSerializer messageSerializer)
        {
            this.httpClient = httpClient;
            this.messageSerializer = messageSerializer;
        }

        public T Get<T>(string url)
        {
            var httpRequest = new HttpRequest
            {
                Method = HttpMethod.GET,
                Url = url,
            };

            var response = httpClient.Execute(httpRequest);
            return messageSerializer.Deserialize<T>(response.Body);
        }

        public T Post<T>(string url, object body)
        {
            var httpRequest = new HttpRequest
            {
                Method = HttpMethod.POST,
                Url = url,
                Body = messageSerializer.Serialize(body),
            };
            httpRequest.Headers.Add("Content-Type", "application/json");

            var response = httpClient.Execute(httpRequest);
            return messageSerializer.Deserialize<T>(response.Body);
        }
    }

    public interface IMessageSerializer
    {
        string Serialize<T>(T objectToSerialize);
        T Deserialize<T>(string serializedObject);
    }

    public class MessageSerializer : IMessageSerializer
    {
        public string Serialize<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}