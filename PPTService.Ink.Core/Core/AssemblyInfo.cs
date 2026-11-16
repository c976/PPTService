using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "PPTService.Ink.Core.Windows.Localization.Markup")]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: AssemblyCompany("广州视睿电子科技有限公司 (Guangzhou Shirui Electronics Co.,Ltd)")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © 2014-2020 Guangzhou Shirui Electronics Co.,Ltd, All Rights Reserved.")]
[assembly: AssemblyDescription("Integrated CoreLib sources required by PPTService.Ink")]
// 版本号 / git 短 hash 由仓库根 Version.targets 生成，与主程序完全同源。
[assembly: AssemblyFileVersion(PPTService.GitVersionInfo.ProductVersion)]
[assembly: AssemblyInformationalVersion(PPTService.GitVersionInfo.InformationalVersion)]
[assembly: AssemblyMetadata("CommitHash", PPTService.GitVersionInfo.Hash)]
[assembly: AssemblyProduct("PPTService.Ink.Core")]
[assembly: AssemblyTitle("PPTService.Ink.Core")]
[assembly: AssemblyVersion(PPTService.GitVersionInfo.ProductVersion)]
