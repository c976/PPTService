using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.SaveInfoManagerInjections;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions.SupportedExtChars;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	public static class AssemblyStorageGeneratedSaveInfoProvider
	{
		public static SaveInfoManager LoadTextEditorAssembly(this SaveInfoManager saveInfoManager)
		{
			Assembly assembly = typeof(AssemblyStorageGeneratedSaveInfoProvider).Assembly;
			IEnumerable<SaveInfoCreatorInfo> saveInfoCreatorInfoList = CollectCompileTimeType().Select(( ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>> t) => new SaveInfoCreatorInfo(t.Item1, t.Item2, t.Item3));
			ManualRegisterAssemblySaveInfoCreatorsInfo creatorsInfo = new ManualRegisterAssemblySaveInfoCreatorsInfo(assembly, saveInfoCreatorInfoList);
			saveInfoManager.RegisterAssemblySaveInfoCreators(ref creatorsInfo);
			return saveInfoManager;
		}

		
		private static ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>[] CollectCompileTimeType()
		{
			return new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>[13]
			{
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(ImageCharSaveInfo), new SaveInfoContractAttribute("ImageCharSaveInfo")
				{
					Description = "图片扩展字符的存储，用于放在文本框里面将图片当成字符"
				}, () => new ImageCharSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(AutoNumberSaveInfo), new SaveInfoContractAttribute("AutoNumber")
				{
					Description = "自动项目符号"
				}, () => new AutoNumberSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(LineSaveInfo), new SaveInfoContractAttribute("LineProperty"), () => new LineSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(MarkerSaveInfo), new SaveInfoContractAttribute("TextMarker")
				{
					Description = "文本项目符号"
				}, () => new MarkerSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(ParagraphSaveInfo), new SaveInfoContractAttribute("TextLine"), () => new ParagraphSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(RichTextSaveInfo), new SaveInfoContractAttribute("RichText"), () => new RichTextSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(RunSaveInfo), new SaveInfoContractAttribute("TextRun"), () => new RunSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(FillEffectSaveInfo), new SaveInfoContractAttribute("TextFill")
				{
					Description = "填充"
				}, () => new FillEffectSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(FrameEffectSaveInfo), new SaveInfoContractAttribute("TextFrame")
				{
					Description = "边框"
				}, () => new FrameEffectSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(HaloEffectSaveInfo), new SaveInfoContractAttribute("TextHalo")
				{
					Description = "发光"
				}, () => new HaloEffectSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(PerspectiveEffectSaveInfo), new SaveInfoContractAttribute("TextPerspective")
				{
					Description = "透视"
				}, () => new PerspectiveEffectSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(ReflectionEffectSaveInfo), new SaveInfoContractAttribute("TextReflection")
				{
					Description = "映像"
				}, () => new ReflectionEffectSaveInfo()),
				new ValueTuple<Type, SaveInfoContractAttribute, Func<SaveInfo>>(typeof(ShadowEffectSaveInfo), new SaveInfoContractAttribute("TextShadow")
				{
					Description = "阴影"
				}, () => new ShadowEffectSaveInfo())
			};
		}
	}
}
