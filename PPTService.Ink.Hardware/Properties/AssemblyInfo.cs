using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: InternalsVisibleTo("PPTService.Ink")]

[assembly: AssemblyTitle("PPTServiceUltra Ink Hardware")]
[assembly: AssemblyDescription("PPT小工具增强版")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The PPTServiceUltra Project")]
[assembly: AssemblyProduct("PPTServiceUltra")]
[assembly: AssemblyCopyright("Copyright © The PPTServiceUltra Project")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]

// 版本号 / git 短 hash 由仓库根 Version.targets 生成，与主程序完全同源。
[assembly: AssemblyVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyFileVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyInformationalVersion(PPTService.GitVersionInfo.InformationalVersion)]
[assembly: AssemblyMetadata("CommitHash", PPTService.GitVersionInfo.Hash)]
