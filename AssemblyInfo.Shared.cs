using System.Reflection;
using System.Runtime.InteropServices;

// 全部项目共用的程序集元数据，统一以主程序 PPTService 的信息为标准。
// 各项目通过 <Compile Include="..\AssemblyInfo.Shared.cs" /> 引入本文件；
// 仅有的特殊差异（ThemeInfo / Guid / InternalsVisibleTo / XmlnsDefinition 等）
// 保留在各自项目的 AssemblyInfo.cs 中。
[assembly: AssemblyTitle("PPTServiceUltra")]
[assembly: AssemblyDescription("PPT小工具增强版")]
[assembly: AssemblyCompany("The PPTServiceUltra Project")]
[assembly: AssemblyProduct("PPTServiceUltra")]
[assembly: AssemblyCopyright("Copyright © The PPTServiceUltra Project")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]

// 版本号与 git 短 hash 由仓库根 Version.targets 在每次构建时写入各项目自己的
// Properties\GitVersionInfo.g.cs（namespace 统一为 PPTService），所有项目完全同源。
// 非 git 仓库 / 无 commit / git 不可用时 hash 为 "unknown"。
[assembly: AssemblyVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyFileVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyInformationalVersion(PPTService.GitVersionInfo.InformationalVersion)]
[assembly: AssemblyMetadata("CommitHash", PPTService.GitVersionInfo.Hash)]
