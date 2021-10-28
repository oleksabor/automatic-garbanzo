### FTPS download test

This test is made using the `FluentFtp` client Nuget package.

The idea is to bother FTP server and to do

* check folder exists (FTPS fails if GetListing is executed for missing folder)
* get folder listing
* download each file

this is repeated for 4 folder: three that exists and one that does not

Here is test folders structure

```
183085
  Mzc3w_4nTee1.jpg
  WdZ6redPXm7j.jpg
220924
  IsG_rEuAu_c7.jpg
  sLnR08KGR8SE.pdf
680605
  7sfK7ZKzNu_u.jpg
  Pg07DIZEJ6mv.jpg

```

files are about 1MB each 

Here are the `Iterate` worker method and `GetFiles` implementation:

``` csharp
static void Iterate(WebSettings config, int count)
{
	using (var unit = new FtpUnit(config))
		try
		{
			for (var q = 0; q < count; q++)
			{
				Log.InfoFormat("iterating {0}", q);
				unit.GetFiles(new[] { "183085", "220924", "680605", "notexists" });
			}
		}
		catch (Exception e)
		{
			Log.ErrorException("failed to iterate", e);
		}
}

public void GetFiles(string[] dirNames)
{
	foreach (var dn in dirNames)
	{
		try
		{
			var files = FtpsHelper.GetDirlisting(client.Value, dn);
			Wait();
			foreach (var fi in files)
				try
				{
					var image = FtpsHelper.DownloadFile(client.Value, fi);
					Guard.ArgumentNotNull(image.Name, nameof(image.Name));
					Guard.ArgumentNotNull(image.Value, nameof(image.Value));
					Log.DebugFormat("downloaded file {0}", image.Name);
					Wait();
				}
				catch (Exception e)
				{
					Log.ErrorException("failed to get file {0}", e, fi.FullName);
				}
		}
		catch (Exception ed)
		{
			Log.ErrorException("failed to get dir {0}", ed, dn);
			ResetConnection();
		}
	}
}
```