using Newtonsoft.Json.Linq;

namespace OpenMagazine.Core
{
	public static class PatentesProcess
	{
		public static async Task AnalyzeJson(string content)
		{
			var jObject = JObject.Parse(content);

			var magazine = $"{jObject["@numero"]} - {jObject["@dataPublicacao"]}";

			jObject.SelectTokens("..@inid").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@sequencia").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@kindcode").ToList().ForEach(t => t.Parent?.Remove());



			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("comentario")).ToList()
				.ForEach(property => property.Replace(new JProperty("texto-complementar", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("concessao")).ToList()
				.ForEach(property => property.Replace(new JProperty("data-concessao", property.Value)));



			foreach (var despacho in jObject["despacho"]!)
			{

				despacho["codigo"] = $"{despacho["codigo"]} - {despacho["titulo"]}"; despacho["titulo"]?.Parent?.Remove();
				if (despacho["texto-complementar"] is JObject comentario) despacho["texto-complementar"] = comentario["#text"];


				if (despacho["processo-patente"] is JObject processo)
				{
					if (processo["numero"] is JObject numero) processo["numero"] = numero["#text"];

					if (processo["publicacao-nacional"] is JObject publicacaoNacional) processo["publicacao-nacional"] = publicacaoNacional["data-rpi"];

					if (processo["titulo"] is JObject titulo) processo["titulo"] = titulo["#text"];

					if (processo["data-concessao"] is JObject concessao) processo["data-concessao"] = concessao["data"];

					if (processo["data-deposito"] is JObject dataDeposito) processo["data-deposito"] = dataDeposito["#text"];

					if (processo["data-prioridade"] is JObject dataPrioridade) processo["data-prioridade"] = dataPrioridade["#text"];

					if (despacho["processo-patente"]?["data-publicacao"] is JObject dataPublicacao) processo["data-publicacao"] = dataPublicacao["#text"];





					if (processo["titular-lista"] is JObject titularLista)
					{
						var titular = titularLista["titular"];

						if (titular is JArray)
						{
							processo["titular-lista"] = titular;
						}
						else if (titular is JObject)
						{
							processo["titular-lista"] = new JArray(titular);
						}
					}

					if (processo["inventor-lista"] is JObject inventorLista)
					{
						var inventor = inventorLista["inventor"];
						if (inventor is JArray)
						{
							processo["inventor-lista"] = inventor;
						}
						else if (inventor is JObject)
						{
							processo["inventor-lista"] = new JArray(inventor);
						}
					}

					if (processo["classificacao-internacional-lista"] is JObject classificacaoInternacionalLista)
					{
						var classificacaoInternacional = classificacaoInternacionalLista["classificacao-internacional"];

						if (classificacaoInternacional is JArray)
						{
							processo["classificacao-internacional-lista"] = classificacaoInternacional;
						}
						else if (classificacaoInternacional is JObject)
						{
							processo["classificacao-internacional-lista"] = new JArray(classificacaoInternacional);
						}
					}

					if (processo["prioridade-unionista-lista"] is JObject prioridadeUnionistaLista)
					{
						var prioridadeUnionista = prioridadeUnionistaLista["prioridade-unionista"];
						if (prioridadeUnionista is JArray)
						{
							processo["prioridade-unionista-lista"] = prioridadeUnionista;
						}
						else if (prioridadeUnionista is JObject)
						{
							processo["prioridade-unionista-lista"] = new JArray(prioridadeUnionista);
						}
					}

					if (processo["prioridade-interna-lista"] is JObject prioridadeinternalista)
					{
						var prioridadeinterna = prioridadeinternalista["prioridade-interna"];
						if (prioridadeinterna is JArray)
						{
							processo["prioridade-interna-lista"] = prioridadeinterna;
						}
						else if (prioridadeinterna is JObject)
						{
							processo["prioridade-interna-lista"] = new JArray(prioridadeinterna);
						}
					}





					if (processo["titular-lista"] is JArray titularArray)
					{
						foreach (var titularObj in titularArray)
						{
							if (titularObj["endereco"]!["pais"] is JObject pais && pais.TryGetValue("sigla", out var sigla))
							{
								if (titularObj["endereco"]!["uf"] != null)
								{
									titularObj["endereco"] = $"{sigla}/{titularObj["endereco"]!["uf"]}";
								}
								else
								{
									titularObj["endereco"] = $"{sigla}";
								}
							}
						}
					}

					if (processo["classificacao-internacional-lista"] is JArray classificacaoInternacionalArray)
					{
						foreach (var classificacaoInternacionalObj in classificacaoInternacionalArray)
						{
							if (classificacaoInternacionalObj is JObject classificacaoInternacional)
							{
								classificacaoInternacional["classificacao"] = $"{classificacaoInternacional["#text"]?.ToString()} ({classificacaoInternacional["@ano"]?.ToString()})";

								classificacaoInternacional["@ano"]?.Parent?.Remove();
								classificacaoInternacional["#text"]?.Parent?.Remove();
							}
						}
					}

					if (processo["prioridade-unionista-lista"] is JArray prioridadeLista)
					{
						foreach (var prioridadeObj in prioridadeLista)
						{
							prioridadeObj["pais-prioridade"] = $"{prioridadeObj["sigla-pais"]?["#text"]}";
							prioridadeObj["numero-prioridade"] = $"{prioridadeObj["numero-prioridade"]?["#text"]}";
							prioridadeObj["data-prioridade"] = $"{prioridadeObj["data-prioridade"]?["#text"]}";

							prioridadeObj["sigla-pais"]?.Parent?.Remove();
						}
					}
				}

				despacho["revista"] = magazine;

				//Console.WriteLine(despacho);
			}
		}
	}
}