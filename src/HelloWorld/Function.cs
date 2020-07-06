using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Newtonsoft.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class Function
    {

        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {

            var location = await GetCallingIP();
            var body = new Dictionary<string, string>
            {
                { "message", "hello world" },
                { "location", location }
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}

namespace Book
{
    public class StringIntegerBoolean
    {
        public void HandlerString(string s)
        {
            Console.WriteLine($"Hello, {s}");
        }

        public Boolean HandlerBoolean(bool input)
        {
            return !input;
        }

        public Boolean HandlerInt(int input)
        {
            return input > 100;
        }
    }

    public class ListLambda
    {
        public List<int> HandlerList(List<int> input)
        {
            return input.Select(x => x + 100).ToList();
        }

        public Dictionary<string, string> HandlerDictionary(
            Dictionary<string, string> input)
        {
            var newDictionary = new Dictionary<string, string>();
            input.ToList().ForEach(p => newDictionary.Add("New Map -> " + p.Key, p.Value));
            return newDictionary;
        }

        public Dictionary<string, Dictionary<string, int>> HandlerNestedCollection(List<Dictionary<string, int>> input)
        {
            var newDictionary = new Dictionary<string, Dictionary<string, int>>();
            var numbers = Enumerable.Range(0, input.Count);

            numbers.ToList().ForEach(n => newDictionary.Add("Nested at position " + n, input.ElementAt(n)));

            return newDictionary;
        }


    }

    public class PocoLambda
    {
        public PocoResponse HandlerPoco(PocoInput input)
        {
            Console.WriteLine("Id:" + input.Id);
            Console.WriteLine("Name:" + input.Name);
            Console.WriteLine("BirthDate:" + input.BirthDate.ToShortDateString());
            return new PocoResponse { ExitCode = 0, StatusCode="SUCCESS", TimeStamp=DateTime.Now};
        } 
    }

    public class PocoInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class PocoResponse
    {
        public int ExitCode { get; set; }
        public string StatusCode { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
