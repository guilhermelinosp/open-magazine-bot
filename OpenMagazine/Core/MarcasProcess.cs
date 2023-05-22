using Newtonsoft.Json.Linq;

namespace OpenMagazine.Core
{
	public static class MarcasProcess
	{
		public static async Task AnalyzeJson(string content)
		{
			var jObject = JObject.Parse(content);

			var magazine = $"{jObject["@numero"]} - {jObject["@data"]}";

			jObject.SelectTokens("..@inid").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@sequencia").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@kindcode").ToList().ForEach(t => t.Parent?.Remove());

			foreach (var processo in jObject["processo"]!)
			{


				processo["revista"] = magazine;

				Console.WriteLine(processo);
			}
		}
	}
}