using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;

[assembly: AssemblyTitle("PPTServiceUltra")]
[assembly: AssemblyDescription("PPT小工具增强版")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The PPTServiceUltra Project")]
[assembly: AssemblyProduct("PPTServiceUltra")]
[assembly: AssemblyCopyright("Copyright © The PPTServiceUltra Project")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
// 版本号与 git 短 hash 由 Version.targets 在每次构建时写入 Properties\GitVersionInfo.g.cs。
// 非 git 仓库 / 仓库尚无 commit / git 不可用时，hash 为 "unknown"。
// 程序集标识版本也跟随全局版本号，不再固定 1.0.0.0。
[assembly: AssemblyVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyFileVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyInformationalVersion(PPTService.GitVersionInfo.InformationalVersion)]
[assembly: AssemblyMetadata("CommitHash", PPTService.GitVersionInfo.Hash)]
