using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net.Sockets;
using static Qdrant.Client.Grpc.Conditions;

//docker run -p 6333:6333 - p 6334:6334 ^
//-v C:\\Users\\LEnriquez\\luiscocoenriquezvector\\qdrant_storage:/ qdrant / storage:z ^
//qdrant / qdrant

// The C# client uses Qdrant's gRPC interface
var client = new QdrantClient("localhost", 6334);

//var channel = QdrantChannel.ForAddress("https://localhost:6334", new ClientConfiguration
//{
//    ApiKey = "<api key>",
//    CertificateThumbprint = "<certificate thumbprint>"
//});
//var grpcClient = new QdrantGrpcClient(channel);
//var client = new QdrantClient(grpcClient);

//create a new Collection
await client.CreateCollectionAsync("my_collection1",
    new VectorParams { Size = 4, Distance = Distance.Cosine });

// generate some vectors
var random = new Random();
var points = Enumerable.Range(1, 4).Select(i => new PointStruct
{
    Id = (ulong)i,
    Vectors = Enumerable.Range(1, 4).Select(_ => (float)random.NextDouble()).ToArray(),
    Payload =
  {
    ["color"] = "red",
    ["rand_number"] = i % 10
  }
}).ToList();

var updateResult = await client.UpsertAsync("my_collection1", points);

Console.WriteLine(points.ToString());

//Search for similar vectors
var queryVector = Enumerable.Range(1, 4).Select(_ => (float)random.NextDouble()).ToArray();

// return the 5 closest points
var points1 = await client.SearchAsync(
  "my_collection1",
  queryVector,
  limit: 4);

Console.WriteLine(points1.ToString());

//Search for similar vectors with filtering condition

// return the 5 closest points where rand_number >= 3
var points2 = await client.SearchAsync(
  "my_collection1",
  queryVector,
  filter: Range("rand_number", new Qdrant.Client.Grpc.Range { Gte = 3 }),
  limit: 4);

Console.WriteLine(points2.ToString());