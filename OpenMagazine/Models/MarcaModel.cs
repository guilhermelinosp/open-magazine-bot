using Newtonsoft.Json;

namespace OpenMagazine.Models
{
	internal class MarcaModel
	{
		[JsonProperty("revista")]
		public Marca? Revista { get; set; }
	}

	internal class Marca
	{
		[JsonProperty("processo")]
		public object[]? Processo { get; set; }

		[JsonProperty("numero")]
		public string? Numero { get; set; }

		[JsonProperty("data")]
		public string? Data { get; set; }
	}
}