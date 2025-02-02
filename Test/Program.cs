﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Downloader;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly string repoUrl = "https://api.github.com/repos/Round-Studio/Round.NET.AvaloniaApp.EncodeCalculator/releases/latest";
    private static readonly string currentVersion = "AutoPublish-2025-01-24.15-47-041"; // 当前版本号

    static async Task Main(string[] args)
    {
        try
        {
            // 添加User-Agent头
            client.DefaultRequestHeaders.Add("User-Agent", "YourApp-Name");

            // 获取最新版本信息
            string jsonResponse = await client.GetStringAsync(repoUrl);
            JObject releaseInfo = JObject.Parse(jsonResponse);

            string latestVersion = releaseInfo["tag_name"].ToString();
            Console.WriteLine($"最新版本: {latestVersion}");

            // 检查是否为当前版本
            if (latestVersion == currentVersion)
            {
                Console.WriteLine("当前版本已是最新版本，无需更新。");
            }
            else
            {
                Console.WriteLine("发现新版本，正在下载...");
                await DownloadInstaller(releaseInfo);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
            Console.WriteLine("请检查以下可能的问题：");
            Console.WriteLine("1. 确保网络连接正常，可以访问GitHub API。");
            Console.WriteLine("2. 检查链接是否正确：{0}", repoUrl);
            Console.WriteLine("3. 如果问题仍然存在，请稍后重试或联系GitHub支持。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }

    private static async Task DownloadInstaller(JObject releaseInfo)
    {
        // 获取系统架构
        string architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        string installerName = GetInstallerName(architecture);

        if (string.IsNullOrEmpty(installerName))
        {
            Console.WriteLine("未找到适合当前系统架构的安装程序。");
            return;
        }

        // 查找对应的安装文件
        JArray assets = releaseInfo["assets"] as JArray;
        JObject installerAsset = assets?.FirstOrDefault(asset => asset["name"].ToString() == installerName) as JObject;

        if (installerAsset != null)
        {
            string downloadUrl = installerAsset["browser_download_url"].ToString();
            Console.WriteLine($"正在下载: {installerName}");
            await DownloadFileAsync(downloadUrl, installerName);
            Console.WriteLine("下载完成！");
        }
        else
        {
            Console.WriteLine("未找到对应的安装文件。");
        }
    }

    private static string GetInstallerName(string architecture)
    {
        switch (architecture)
        {
            case "arm64":
                return "Round.NET.AvaloniaApp.EncodeCalculator.Desktop.win.arm64.installer.exe";
            case "x64":
                return "Round.NET.AvaloniaApp.EncodeCalculator.Desktop.win.x64.installer.exe";
            case "x86":
                return "Round.NET.AvaloniaApp.EncodeCalculator.Desktop.win.x86.installer.exe";
            default:
                return null;
        }
    }

    private static async Task DownloadFileAsync(string url, string fileName)
    {
        try
        {
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                ChunkCount = 8,
                MaximumBytesPerSecond = 1024 * 1024 * 2,
                MaxTryAgainOnFailover = 5,
                MaximumMemoryBufferBytes = 1024 * 1024 * 50,
                ParallelDownload = true,
                ParallelCount = 4,
                Timeout = 1000,
                ClearPackageOnCompletionWithFailure = true,
                MinimumSizeOfChunking = 1024,
                ReserveStorageSpaceBeforeStartingDownload = true
            };

            var downloader = new DownloadService(downloadOpt);
            downloader.DownloadProgressChanged += (sender, args) =>
            {
                Console.WriteLine($"{args.ProgressPercentage}%");
            };

            await downloader.DownloadFileTaskAsync(url, fileName);
            Console.WriteLine($"文件已成功下载到: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"下载失败: {ex.Message}");
        }
    }
}