using Newtonsoft.Json.Linq;

namespace OpenMagazine.Core
{
	public static class DesenhosProcess
	{
		public static async Task AnalyzeJson(string content)
		{
			var jObject = JObject.Parse(content);

			var magazine = $"{jObject["@numero"]} - {jObject["@dataPublicacao"]}";

			jObject.SelectTokens("..@inid").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@sequencia").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@kindcode").ToList().ForEach(t => t.Parent?.Remove());




			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("concessao")).ToList()
				.ForEach(property => property.Replace(new JProperty("data-concessao", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("comentario")).ToList()
				.ForEach(property => property.Replace(new JProperty("texto-complementar", property.Value)));





			foreach (var despacho in jObject["despacho"]!)
			{
				despacho["codigo"] = $"{despacho["codigo"]} - {despacho["titulo"]}"; despacho["titulo"]?.Parent?.Remove();

				if (despacho["texto-complementar"] is JObject comentario) despacho["texto-complementar"] = comentario["#text"];


				if (despacho["processo-patente"] is JObject processo)
				{
					if (processo["numero"] is JObject numero) processo["numero"] = numero["#text"];

					if (processo["data-deposito"] is JObject dataDeposito) processo["data-deposito"] = dataDeposito["#text"];

					if (processo["data-concessao"] is JObject concessao) processo["data-concessao"] = concessao["data"];

					if (processo["titulo"] is JObject titulo) processo["titulo"] = titulo["#text"];

					if (processo["data-registro-prorrogacao"] is JObject dataRegistroProrrogacao) processo["data-registro-prorrogacao"] = dataRegistroProrrogacao["#text"];

					if (processo["publicacao-nacional"] is JObject publicacaoNacional) processo["publicacao-nacional"] = publicacaoNacional["data-rpi"];





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

					if (processo["procurador-lista"] is JObject procuradorLista)
					{
						var procurador = procuradorLista["procurador"];

						if (procurador is JArray)
						{
							processo["procurador-lista"] = procurador;
						}
						else if (procurador is JObject)
						{
							processo["procurador-lista"] = new JArray(procurador);
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

					if (processo["classificacao-nacional-lista"] is JObject classificacaoCacionalLista)
					{
						var classificacaoCacional = classificacaoCacionalLista["classificacao-nacional"];

						if (classificacaoCacional is JArray)
						{
							processo["classificacao-nacional-lista"] = classificacaoCacional;
						}
						else if (classificacaoCacional is JObject)
						{
							processo["classificacao-nacional-lista"] = new JArray(classificacaoCacional);
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

					if (processo["classificacao-nacional-lista"] is JArray classificacaoNacionalListaArray)
					{
						foreach (var classificacaoNacionaObj in classificacaoNacionalListaArray)
						{
							classificacaoNacionaObj["classificacao"] = classificacaoNacionaObj["#text"];
							classificacaoNacionaObj["#text"]?.Parent?.Remove();
						}
					}

				}

				despacho["revista"] = magazine;

				//Console.WriteLine(despacho);
			}
		}
	}
}
