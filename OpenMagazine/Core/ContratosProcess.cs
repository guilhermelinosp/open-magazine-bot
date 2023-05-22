using Newtonsoft.Json.Linq;

namespace OpenMagazine.Core
{
	public static class ContratosProcess
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

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("nomeCompleto")).ToList()
				.ForEach(property => property.Replace(new JProperty("nome-completo", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("cedentes")).ToList()
				.ForEach(property => property.Replace(new JProperty("cedente-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("cessionarias")).ToList()
				.ForEach(property => property.Replace(new JProperty("cessionaria-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("peticoes")).ToList()
				.ForEach(property => property.Replace(new JProperty("peticao-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("dataProtocolo")).ToList()
				.ForEach(property => property.Replace(new JProperty("data-protocolo", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("certificados")).ToList()
				.ForEach(property => property.Replace(new JProperty("certificado-lista", property.Value)));



			foreach (var despacho in jObject["despacho"]!)
			{
				despacho["codigo"] = $"{despacho["codigo"]} - {despacho["titulo"]}"; despacho["titulo"]?.Parent?.Remove();
				if (despacho["texto-complementar"] is JObject comentario) despacho["texto-complementar"] = comentario["text"];


				if (despacho["processo-contrato"] is JObject processo)
				{
					if (processo["numero"] is JObject numero) processo["numero"] = numero["#text"];

					if (processo["data-protocolo"] is JObject dataProtocolo) processo["data-protocolo"] = dataProtocolo["#text"];





					// Object to Array
					if (processo["cedente-lista"] is JObject cedentes)
					{
						var cedente = cedentes["cedente"];

						if (cedente is JArray)
						{
							processo["cedente-lista"] = cedente;
						}
						else if (cedente is JObject)
						{
							processo["cedente-lista"] = new JArray(cedente);
						}
					}

					if (processo["cessionaria-lista"] is JObject cessionarias)
					{
						var cessionaria = cessionarias["cessionaria"];

						if (cessionaria is JArray)
						{
							processo["cessionaria-lista"] = cessionaria;
						}
						else if (cessionaria is JObject)
						{
							processo["cessionaria-lista"] = new JArray(cessionaria);
						}
					}

					if (processo["peticao-lista"] is JObject peticoes)
					{
						var peticao = peticoes["peticao"];

						if (peticao is JArray)
						{
							processo["peticao-lista"] = peticao;
						}
						else if (peticao is JObject)
						{
							processo["peticao-lista"] = new JArray(peticao);
						}
					}

					if (processo["certificado-lista"] is JObject certificados)
					{
						var certificado = certificados["certificado"];

						if (certificado is JArray)
						{
							processo["certificado-lista"] = certificado;
						}
						else if (certificado is JObject)
						{
							processo["certificado-lista"] = new JArray(certificado);
						}
					}






					// Intern Array
					if (processo["peticao-lista"] is JArray peticoesArray)
					{
						foreach (var peticoesObj in peticoesArray)
						{
							if (peticoesObj["numero"] is JObject peticoesNumero) peticoesObj["numero"] = peticoesNumero["#text"];
							if (peticoesObj["data-protocolo"] is JObject peticoesDataProtocoloro) peticoesObj["data-protocolo"] = peticoesDataProtocoloro["#text"]?.ToString();
							if (peticoesObj["requerente"] is JObject requerente) peticoesObj["requerente"] = requerente["nome-completo"]!["#text"]?.ToString();
						}
					}

					if (processo["cedente-lista"] is JArray cedentesArray)
					{
						foreach (var cedentesObj in cedentesArray)
						{
							if (cedentesObj["nome-completo"] is JObject cedentesNomeCompleto) cedentesObj["nome-completo"] = cedentesNomeCompleto["#text"];
							if (cedentesObj["endereco"] is JObject cedentesEndereco) cedentesObj["endereco"] = cedentesEndereco["pais"]!["nome"]!["#text"];
						}
					}

					if (processo["cessionaria-lista"] is JArray cessionariasArray)
					{
						foreach (var cessionariasObj in cessionariasArray)
						{
							if (cessionariasObj["nome-completo"] is JObject cessionariasNomeCompleto) cessionariasObj["nome-completo"] = cessionariasNomeCompleto["#text"];
							if (cessionariasObj["endereco"] is JObject cessionariasEndereco) cessionariasObj["endereco"] = cessionariasEndereco["pais"]!["nome"]!["#text"];
							if (cessionariasObj["setor"] is JObject cessionariasSetor) cessionariasObj["setor"] = cessionariasSetor["#text"];
						}
					}

					if (processo["certificado-lista"] is JArray certificadosArray)
					{
						foreach (var certificadosObj in certificadosArray)
						{
							if (certificadosObj["numero"] is JObject certificadosNumero) certificadosObj["numero"] = certificadosNumero["#text"];
							if (certificadosObj["naturezaDocumento"] is JObject certificadosNaturezaDocumento) certificadosObj["naturezaDocumento"] = certificadosNaturezaDocumento["#text"];
							if (certificadosObj["textoObjeto"] is JObject certificadosTextoObjeto) certificadosObj["textoObjeto"] = certificadosTextoObjeto["#text"];
							if (certificadosObj["siglaCategoria"] is JObject certificadosSiglaCategoria) certificadosObj["siglaCategoria"] = certificadosSiglaCategoria["#text"];
							if (certificadosObj["descricaoMoeda"] is JObject certificadosDescricaoMoeda) certificadosObj["descricaoMoeda"] = certificadosDescricaoMoeda["#text"];
							if (certificadosObj["valorContrato"] is JObject certificadosValorContrato) certificadosObj["valorContrato"] = certificadosValorContrato["#text"];
							if (certificadosObj["formaPagamento"] is JObject certificadosFormaPagamento) certificadosObj["formaPagamento"] = certificadosFormaPagamento["#text"];
							if (certificadosObj["prazoContrato"] is JObject certificadosPrazoContrato) certificadosObj["prazoContrato"] = certificadosPrazoContrato["#text"];
							if (certificadosObj["prazoVigenciaPI"] is JObject certificadosPrazoVigenciaPi) certificadosObj["prazoVigenciaPI"] = certificadosPrazoVigenciaPi["#text"];
							if (certificadosObj["observacao"] is JObject certificadosObservacao) certificadosObj["observacao"] = certificadosObservacao["#text"];
						}
					}
				}

				despacho["revista"] = magazine;

				//Console.WriteLine(despacho);
			}
		}
	}
}
