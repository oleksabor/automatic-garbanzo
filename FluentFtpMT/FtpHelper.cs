using FluentFTP;
using FluentFtpMT.Logging;
using Guards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentFtpMT
{
	public static class FtpsHelper
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		
		public static FtpListItem[] GetDirlisting(FtpClient cln, string formPath)
		{
			var files = cln.GetListing(formPath);
			if (files.Length == 0)
			{
				Log.WarnFormat("no folder listing result on path:{0}", formPath);
				// https://github.com/robinrodricks/FluentFTP/issues/723
				Thread.Sleep(500);
				files = cln.GetListing(formPath);
			}

			return files.ToArray();
		}

		public static Image DownloadFile(FtpClient cln, FtpListItem listItem)
		{
				if (listItem.Type == FtpFileSystemObjectType.File)
				{
					cln.Download(out byte[] memStream, listItem.FullName, 0);
					Log.DebugFormat("downloaded image:{0} {1}MB", listItem.FullName, memStream.Length / 1024 / 1024);
					//counter.Inc(Counters.ftpImagesPerSecond);

					return new Image(listItem.Name, memStream);
				}
				else
					throw new ArgumentException("not a file");
		}

		public static SslProtocols GetProtocol(string code)
		{
			switch (code?.ToLower())
			{
				case "tls11": return SslProtocols.Tls11;
				case "tls": return SslProtocols.Tls; // does not work on azure ftps
				default: return SslProtocols.Tls12;
			}
		}

		public static FtpClient CreateClient(WebSettings s)
		{
			var tlsprotocol = GetProtocol(s.TlsProtocolVersion);
			try
			{
				Guard.ArgumentNotNullOrEmpty(s.Url, nameof(s.Url));
				Guard.ArgumentNotNullOrEmpty(s.User, nameof(s.User));
				Guard.ArgumentNotNullOrEmpty(s.Password, nameof(s.Password));

				var cln = new FtpClient(s.Url, s.Port, s.User, s.Password);
				cln.EncryptionMode = FtpEncryptionMode.Explicit;

				Log.InfoFormat("new ftp client to {0}:{1}", s.Url, s.Port);

				cln.SslProtocols = tlsprotocol;
				cln.ValidateCertificate += (c, e) => OnValidateCertificate(c, e, s.IgnoreCertificateError);
#if DEBUG
				cln.ReadTimeout = 5 * 1000;
#endif

#if !FtpLog
				cln.OnLogEvent += (l, m) => { };
#endif
				cln.SocketPollInterval = 1000;
				cln.Connect();

				//counter.Inc(Counters.ftpConnectionsPerSecond);
				return cln;
			}
			catch (Exception)
			{
				Log.ErrorFormat($"failed to connect with site:{s.Url}, user:{s.User}, port:{s.Port}, tls:{tlsprotocol}");
				throw;
			}
		}

		static bool _certificateErrorLogged;

		static void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e, bool ignoreCertificateError)
		{
			if (e.PolicyErrors != SslPolicyErrors.None && !_certificateErrorLogged)
			{
				Log.WarnFormat("failed to check ftps certificate, policyErrors:{0}", e.PolicyErrors);
				_certificateErrorLogged = true;
			}
			if (e.PolicyErrors == SslPolicyErrors.None || ignoreCertificateError)
				e.Accept = true;
		}
	}
}
