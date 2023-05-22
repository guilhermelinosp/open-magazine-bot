using Newtonsoft.Json.Linq;

namespace OpenMagazine.Core
{
	public static class ProgramasProcess
	{
		public static async Task AnalyzeJson(string content)
		{
			var jObject = JObject.Parse(content);

			var magazine = $"{jObject["@numero"]} - {jObject["@dataPublicacao"]}";

			jObject.SelectTokens("..@inid").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@sequencia").ToList().ForEach(t => t.Parent?.Remove());
			jObject.SelectTokens("..@kindcode").ToList().ForEach(t => t.Parent?.Remove());





			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("linguagemLista")).ToList()
				.ForEach(property => property.Replace(new JProperty("linguagem-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("dataCriacao")).ToList()
				.ForEach(property => property.Replace(new JProperty("data-criacao", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("campoAplicacaoLista")).ToList()
				.ForEach(property => property.Replace(new JProperty("campo-aplicacao-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("tipoProgramaLista")).ToList()
				.ForEach(property => property.Replace(new JProperty("tipo-programa-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("titularLista")).ToList()
				.ForEach(property => property.Replace(new JProperty("titular-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("criadorLista")).ToList()
				.ForEach(property => property.Replace(new JProperty("criador-lista", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("nome")).ToList()
				.ForEach(property => property.Replace(new JProperty("nome-completo", property.Value)));

			jObject.Descendants().OfType<JProperty>().Where(p => p.Name.Contains("comentario")).ToList()
				.ForEach(property => property.Replace(new JProperty("texto-complementar", property.Value)));





			foreach (var despacho in jObject["despacho"]!)
			{
				despacho["codigo"] = $"{despacho["codigo"]} - {despacho["titulo"]}"; despacho["titulo"]?.Parent?.Remove();


				if (despacho["texto-complementar"] is JObject comentario) despacho["texto-complementar"] = comentario["#text"];

				if (despacho["processo-programa"] is JObject processo)
				{
					if (processo["numero"] is JObject numero) processo["numero"] = numero["#text"];
					if (processo["titulo"] is JObject titulo) processo["titulo"] = titulo["#text"];
					if (processo["data-criacao"] is JObject dataCriacao) processo["data-criacao"] = dataCriacao["#text"];



					// Object to Array
					if (processo["campo-aplicacao-lista"] is JObject campoAplicacaoLista)
					{
						var campoAplicacao = campoAplicacaoLista["campoAplicacao"];

						if (campoAplicacao is JArray)
						{
							processo["campo-aplicacao-lista"] = campoAplicacao;
						}
						else if (campoAplicacao is JObject)
						{
							processo["campo-aplicacao-lista"] = new JArray(campoAplicacao);
						}
					}

					if (processo["linguagem-lista"] is JObject linguagemLista)
					{
						var linguagem = linguagemLista["linguagem"];

						if (linguagem is JArray)
						{
							processo["linguagem-lista"] = linguagem;
						}
						else if (linguagem is JObject)
						{
							processo["linguagem-lista"] = new JArray(linguagem);
						}
					}

					if (processo["tipo-programa-lista"] is JObject tipoProgramaLista)
					{
						var tipoPrograma = tipoProgramaLista["tipoPrograma"];

						if (tipoPrograma is JArray)
						{
							processo["tipo-programa-lista"] = tipoPrograma;
						}
						else if (tipoPrograma is JObject)
						{
							processo["tipo-programa-lista"] = new JArray(tipoPrograma);
						}
					}

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

					if (processo["criador-lista"] is JObject criadorLista)
					{
						var criador = criadorLista["criador"];

						if (criador is JArray)
						{
							processo["criador-lista"] = criador;
						}
						else if (criador is JObject)
						{
							processo["criador-lista"] = new JArray(criador);
						}
					}




					if (processo["campo-aplicacao-lista"] is JArray campoAplicacaoListaArray)
					{
						foreach (var campoAplicacaoListaObj in campoAplicacaoListaArray)
						{
							if (campoAplicacaoListaObj["codigo"] is JObject campoAplicacaoListaCodigo) campoAplicacaoListaObj["codigo"] = campoAplicacaoListaCodigo["#text"];
						}
					}

					if (processo["tipo-programa-lista"] is JArray tipoProgramaListaArray)
					{
						foreach (var tipoProgramaListaObj in tipoProgramaListaArray)
						{
							if (tipoProgramaListaObj["codigo"] is JObject tipoProgramaListaCodigo) tipoProgramaListaObj["codigo"] = tipoProgramaListaCodigo["#text"];
						}
					}

					if (processo["linguagem-lista"] is JArray linguagemListaArray)
					{
						foreach (var linguagemListaObj in linguagemListaArray)
						{
							linguagemListaObj["linguagem"] = linguagemListaObj["#text"];
							linguagemListaObj["#text"]?.Parent?.Remove();
						}
					}
				}




				despacho["revista"] = magazine;

				//Console.WriteLine(despacho);
			}
		}
	}
}
