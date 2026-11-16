using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyTitle("PPTServiceUltra Collection")]
[assembly: AssemblyDescription("PPT小工具增强版")]
[assembly: AssemblyCompany("The PPTServiceUltra Project")]
[assembly: AssemblyProduct("PPTServiceUltra")]
[assembly: AssemblyCopyright("Copyright © The PPTServiceUltra Project")]
[assembly: AssemblyTrademark("")]

// 与主程序同源：Version.targets 生成的 GitVersionInfo.g.cs（含 git 短 hash）
[assembly: AssemblyVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyFileVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyInformationalVersion(PPTService.GitVersionInfo.InformationalVersion)]
[assembly: AssemblyMetadata("CommitHash", PPTService.GitVersionInfo.Hash)]
