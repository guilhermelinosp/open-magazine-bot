using OpenMagazine.INPI;

namespace OpenMagazine
{
	internal static class Program
	{
		private static async Task Main()
		{
			Console.WriteLine($"{DateTime.Now} - Start");
			try
			{
				await Magazine.DonwloadFiles();
				await Magazine.ExtractFiles();
				await Magazine.ConvertFiles();
				await Magazine.ProcessFiles();
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error Main(): {ex.Message}");
				throw;
			}
			Console.WriteLine($"{DateTime.Now} - End");
		}
	}
}