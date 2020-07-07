using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Book
{
    public class WeatherEventLambda
    {

        private static readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        private static string TableName =
            Environment.GetEnvironmentVariable("LOCATIONS_TABLE");
        public class WeatherEvent
        {
            public string LocationName { get; set; }
            public double Temperature { get; set; }
            public long Timestamp { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        public async Task<APIGatewayProxyResponse> HandlerWeatherEvent(APIGatewayProxyRequest request, ILambdaContext functionContext)
        {
            var weatherEvent = JsonConvert.DeserializeObject<WeatherEvent>(request.Body);
            Console.WriteLine("LocationName is...");
            Console.WriteLine(weatherEvent.LocationName);
            Console.WriteLine(weatherEvent.Temperature);
            Console.WriteLine(weatherEvent.Longitude);
            Console.WriteLine(weatherEvent.Latitude);

            var context = new DynamoDBContext(client);
            var dbrequest = new PutItemRequest { TableName = TableName };
            dbrequest.Item = new Dictionary<string, AttributeValue>()
            {
                { "locationName", new AttributeValue { S = weatherEvent.LocationName} },
                { "Temperature", new AttributeValue { N = weatherEvent.Temperature.ToString(CultureInfo.InvariantCulture)} },
                { "Timestamp", new AttributeValue { N = weatherEvent.Timestamp.ToString(CultureInfo.InvariantCulture)} },
                { "Longitude", new AttributeValue { N = weatherEvent.Longitude.ToString(CultureInfo.InvariantCulture)} },
                { "Latitude", new AttributeValue { N = weatherEvent.Latitude.ToString(CultureInfo.InvariantCulture)} },
            };

            await client.PutItemAsync(dbrequest);
            return new APIGatewayProxyResponse() { StatusCode = 200, Body = $"Weather event recorded for {weatherEvent.LocationName}" };
        }
    }

}
