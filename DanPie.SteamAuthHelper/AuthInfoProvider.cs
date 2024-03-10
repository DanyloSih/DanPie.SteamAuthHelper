using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DanPie.SteamAuthHelper;

public class AuthInfoProvider
{
	public const string MaFileExtension = ".maFile";
	
	public async Task<AuthInfo[]> GetAuthInfoFromMaFilesDirectory(string path)
	{
		bool isDirectoryExist = System.IO.Directory.Exists(path);

		if (!isDirectoryExist)
		{
			throw new ArgumentException($"Passed incorrect dirrectory path: \"{path}\"! {Environment.NewLine}"
				+ "Directory not exist!\n Try again: ");
		}
		string[] files = Directory.GetFiles(path);
		
		List<Task<AuthInfo>> authInfoTasks = new List<Task<AuthInfo>>();

		foreach (var maFilePath in files)
		{
			authInfoTasks.Add(GetAuthInfoFromMaFile(maFilePath));	
		}

		return await Task.WhenAll(authInfoTasks);
	}

	public async Task<AuthInfo> GetAuthInfoFromMaFile(string maFilePath)
	{
		string fileExtension = Path.GetExtension(maFilePath);
		if (!fileExtension.Equals(MaFileExtension))
		{
			throw new ArgumentException(
				$"Incorrect file extension passed in {nameof(maFilePath)}: \"{fileExtension}\"! {Environment.NewLine}" + 
				$"Expected: \"{MaFileExtension}\"");
		}

		string fileData = await File.ReadAllTextAsync(maFilePath);
		SteamGuardAccount steamGuardAccount = JsonConvert.DeserializeObject<SteamGuardAccount>(fileData)!;
		AuthInfo authInfo = new AuthInfo();
		string[] maFileInfo = Path.GetFileName(maFilePath).Split(":");
		if(maFileInfo.Length != 2)
		{
			Console.WriteLine($"Incorrect name format of maFile: \"{maFilePath}\"! {Environment.NewLine}" +
				"Expected filename format: \"username:password.maFile\"");
		}

		authInfo.UserName = maFileInfo[0];
		authInfo.Password = maFileInfo[1].Replace(fileExtension, "");
		authInfo.Code = await steamGuardAccount.GenerateSteamGuardCodeAsync();	
		return authInfo;
	}
}
