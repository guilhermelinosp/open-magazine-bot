using OpenMagazine.INPI;

namespace OpenMagazine
{
	internal static class Program
	{
		private static async Task Main()
		{
			var start = DateTime.Now;
			Console.WriteLine($"{start} - Application Start");
			try
			{
				//await Magazine.DonwloadFiles();
				//await Magazine.ExtractFiles();
				//await Magazine.ConvertFiles();
				await Magazine.ProcessFiles();
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error Main(): {ex.Message}");
				throw;
			}

			Console.WriteLine($"{DateTime.Now} - Application End \n");
			Console.WriteLine($"{DateTime.Now} - Duration Time: {(DateTime.Now - start).Duration():hh\\:mm\\:ss}");
		}
	}
}

