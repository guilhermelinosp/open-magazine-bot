using Newtonsoft.Json;
using OpenMagazine.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.IO.Compression;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace OpenMagazine.INPI
{
	public static class Magazine
	{
		public static async Task DonwloadFiles()
		{
			try
			{
				var options = new EdgeOptions();
				options.AddUserProfilePreference("download.default_directory", Config.DataStorage);
				options.AddUserProfilePreference("download.prompt_for_download", false);
				options.AddUserProfilePreference("disable-popup-blocking", "true");
				options.AddArgument("--headless");

				using var driver = new EdgeDriver(options);

				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/rpi/");
				await Task.Delay(500);
				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/txt/RM{driver.FindElement(By.XPath(@"/html/body/div[4]/div/table[1]/tbody/tr[2]/td[1]")).Text}.zip"); // MARCAS
				await Task.Delay(500);
				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/txt/P{driver.FindElement(By.XPath(@"/html/body/div[4]/div/table[1]/tbody/tr[2]/td[1]")).Text}.zip"); // PATENTES
				await Task.Delay(500);
				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/txt/PC{driver.FindElement(By.XPath(@"/html/body/div[4]/div/table[1]/tbody/tr[2]/td[1]")).Text}.zip"); // PROGRAMA DE COMPUTADOR
				await Task.Delay(500);
				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/txt/CT{driver.FindElement(By.XPath(@"/html/body/div[4]/div/table[1]/tbody/tr[2]/td[1]")).Text}.zip"); // CONTRATOS DE TECNOLOGIA
				await Task.Delay(500);
				driver.Navigate().GoToUrl($"http://revistas.inpi.gov.br/txt/DI{driver.FindElement(By.XPath(@"/html/body/div[4]/div/table[1]/tbody/tr[2]/td[1]")).Text}.zip"); // DESENHOS INDUSTRIAIS
				await Task.Delay(1000);

				driver.Quit();
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error DonwloadFiles(): {ex.Message}");
			}
		}

		public static async Task ExtractFiles()
		{
			try
			{
				foreach (var zipFilePath in Directory.EnumerateFiles(Config.DataStorage, "*.zip"))
				{
					var extractPath = Path.Combine(Config.DataStorage, Path.GetFileNameWithoutExtension(zipFilePath));

					using (var zipArchive = ZipFile.OpenRead(zipFilePath))
					{
						zipArchive.ExtractToDirectory(extractPath);
					}

					File.Delete(zipFilePath);

					foreach (var xmlFilePath in Directory.GetFiles(extractPath, "*.xml"))
					{
						var newFilePath = Path.Combine(Config.DataStorage, Path.GetFileNameWithoutExtension(zipFilePath) + ".xml");

						if (File.Exists(newFilePath))
						{
							File.Delete(newFilePath);
						}

						File.Move(xmlFilePath, newFilePath);

						await Task.Delay(500);
					}

					Directory.Delete(extractPath, true);
				}
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error ExtractFiles(): {ex.Message}");
			}
		}

		public static async Task ConvertFiles()
		{
			try
			{
				foreach (var xmlFilePath in Directory.GetFiles(Config.DataStorage, "*.xml"))
				{
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(xmlFilePath);

					var jsonFile = JsonConvert.SerializeXmlNode(xmlDocument.SelectSingleNode("revista"), Formatting.Indented, true);

					var jsonFilePath = Path.ChangeExtension(xmlFilePath, ".json");

					if (File.Exists(jsonFilePath))
					{
						File.Delete(jsonFilePath);
					}

					await File.WriteAllTextAsync(jsonFilePath, jsonFile);

					await Task.Delay(500);

					File.Delete(xmlFilePath);

				}
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error ConvertFiles(): {ex.Message}");
			}
		}

		public static async Task ProcessFiles()
		{
			try
			{
				foreach (var jsonFilePath in Directory.GetFiles(Config.DataStorage, "*.json"))
				{
					if (!File.Exists(jsonFilePath))
					{
						continue;
					}

					var content = await File.ReadAllTextAsync(jsonFilePath);
					var fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

					if (fileName.Contains("RM2"))
					{
						await MarcasProcess.AnalyzeJson(content);
					}
					else if (fileName.Contains("P2"))
					{
						await PatentesProcess.AnalyzeJson(content);
					}
					else if (fileName.Contains("DI2"))
					{
						await DesenhosProcess.AnalyzeJson(content);
					}
					else if (fileName.Contains("PC2"))
					{
						await ProgramasProcess.AnalyzeJson(content);
					}
					else if (fileName.Contains("CT2"))
					{
						await ContratosProcess.AnalyzeJson(content);
					}
					else
					{
						await Console.Error.WriteLineAsync($"{DateTime.Now} - Warning: Unrecognized file name - {fileName}");
					}
				}
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync($"{DateTime.Now} - Error ProcessFiles(): {ex.Message}");
			}
		}
	}
}