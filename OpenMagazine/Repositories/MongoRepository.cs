using MongoDB.Bson;
using MongoDB.Driver;

namespace OpenMagazine.Repositories
{
	public static class MongoRepository
	{
		public static async Task InsertDocumentsAsync(IEnumerable<BsonDocument> bsonDocuments, string fileName)
		{
			try
			{
				var client = new MongoClient(Config.ConnectionString);
				var database = client.GetDatabase("OpenDatabase");
				var collection = database.GetCollection<BsonDocument>(fileName.Contains("Patente") ? "MagazinePatentes" : "MagazineMarcas");

				await collection.InsertManyAsync(bsonDocuments);
				Console.WriteLine($"File '{fileName}' inserted successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error inserting file: {ex.Message}");
				throw;
			}
		}
	}
}