using FluentFTP;
using FluentFtpMT.Logging;
using Guards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentFtpMT
{
	class Program
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		static void Main(string[] args)
		{
			log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

			var config = new WebSettings() {
				User = "webdocuser",
				Password = "1234qweR",
				Url = "localhost",
				//Port = 990,				21 by default
				PathGet = "/",
				IgnoreCertificateError = true,
				Formats = "\\.zip|\\.jpe?g|\\.pdf$",
			};

			const int count = 70;

			Iterate(config, count);
			Log.Info("done single thread");

			// does not work in parallel
			//Log.Info("multithreading");

			//var configs = new[] { config, new WebSettings() { // two configurations for two threads
			//	User = "webdocuser2", // another user 
			//	Password = "1234qweR",
			//	Url = "localhost",
			//	//Port = 990,
			//	PathGet = "/",
			//	IgnoreCertificateError = true,
			//	Formats = "\\.zip|\\.jpe?g|\\.pdf$",
			//} };

			//var tasks = Enumerable.Range(0, 2)
			//	.Select(_ =>
			//	{
			//		Thread.Sleep(300 * _);
			//		return Task.Run(() => Iterate(configs[_], count));
			//		}).ToArray();

			//Task.WaitAll(tasks);
			//Log.Info("done multithreading");

			Console.ReadKey();
		}

		static void Iterate(WebSettings config, int count)
		{
			using (var unit = new FtpUnit(config))
				try
				{
					for (var q = 0; q < count; q++)
					{
						Log.InfoFormat("iterating {0}", q);
						unit.GetFiles(new[] { "183085", "220924", "680605", });
					}
				}
				catch (Exception e)
				{
					Log.ErrorException("failed to iterate", e);
				}
		}
	}

	public class FtpUnit : IDisposable
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Lazy<FtpClient> client;

		public FtpUnit(WebSettings s)
		{
			client = new Lazy<FtpClient>(() => FtpsHelper.CreateClient(s));
		}

		public void Dispose()
		{
			if (client.IsValueCreated)
			{
				Log.Info("ftp client is disposing");
				client.Value.Dispose();
			}
		}

		public void GetFiles(string[] dirNames)
		{
			foreach (var dn in dirNames)
			{
				var files = FtpsHelper.GetDirlisting(client.Value, dn);
				foreach (var fi in files)
					try
					{
						var image = FtpsHelper.DownloadFile(client.Value, fi);
						Guard.ArgumentNotNull(image.Name, nameof(image.Name));
						Guard.ArgumentNotNull(image.Value, nameof(image.Value));
						Log.DebugFormat("downloaded file {0}", image.Name);
					}
					catch (Exception e)
					{
						Log.ErrorException("failed to get file {0}", e, fi.FullName);
					}
			}
		}
	}
}
