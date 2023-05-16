using Newtonsoft.Json;

namespace OpenMagazine.Models
{
	internal class PatenteModel
	{
		[JsonProperty("revista")]
		public Patente? Revista { get; set; }
	}

	internal class Patente
	{
		[JsonProperty("despacho")]
		public object[]? Despacho { get; set; }

		[JsonProperty("numero")]
		public string? Numero { get; set; }

		[JsonProperty("diretoria")]
		public string? Diretoria { get; set; }

		[JsonProperty("dataPublicacao")]
		public string? DataPublicacao { get; set; }
	}
}