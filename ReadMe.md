# PPTService

基于 PPTService 1.0.0.795 版本反编译的代码

> 本副本用于二次开发，**功能正常优先**，不再要求 100% 还原原版逻辑。原 6 个 `Cvte.*` 类库已按调用闭包整合进 `PPTService.Core` 单一程序集，命名空间统一到 `PPTService.Core.*`，不再生成或依赖任何 CVTE 程序集。

目标框架 Visual Studio 2015 .Net Framework 4.5

## 目录结构

```
\
├── PPTService.sln
├── Version.targets             共享版本号 / git hash 生成目标（三个项目共用）
├── build.number                全局 build 号（每次成功构建后 +1）
├── ReadMe.md
├── lib\                        第三方依赖（非 CVTE 库）
├── PPTService\                 主程序（WPF）
├── PPTService.Core\            整合后的核心库（单一程序集）
├── PPTService.Collection\
├── .github\workflows\          GitHub Actions 编译 / 发布
└── setup\                       NSIS 安装脚本（含构建生成的 Version.nsh）
```

## 特殊修改

### 反混淆与源码重建

原版 8 个程序集中有 4 个被 **SmartAssembly 7.5.1.4370** 混淆：`Cvte.Core.cc.dll`、`Cvte.Pipe.Http.dll`、`Cvte.WebService.dll`、`Cvte.Windows.cc.dll`。直接反编译会产出依赖元数据 token 的 `MemberRefsProxy` 空壳、被合并进同一个 `dynamic` 字段的多个属性，以及不可打印的类型名与命名空间。这样编译虽能通过，运行行为却不正确（二维码分享等路径会失败）。

处理方式：

1. `de4dot -p sa` 反混淆，还原代理调用与加密字符串；
2. `ilspycmd -p -lv CSharp6` 重新生成项目源码，沿用各 csproj 的 `LangVersion 6`，不改构建配置。

去混淆后的替身名按语义重命名：`ns0..ns9` 目录合并为各模块的 `Internal\`，命名空间现统一为 `PPTService.Core.*`，11 个类型改为可读名（如 `PictureUploadRequest`、`AccountSettings`、`SignatureUrlEncoder`、`UserEntity`、`DataContractJsonHelper`）。de4dot 解密后失去调用者的 SA 字符串加密辅助类（`SmartAssembly.*`、8 个空 `AttributeN`、字符串解密表）已删除。

### 编译兼容处理

以下均为让反编译源码在 .NET Framework 4.5 / C# 6 下通过编译所必需的最小修补，不改变语义：

| 类别 | 处理 |
|---|---|
| COM Interop | 4 个事件接口（`EApplication_Event`、`MasterEvents_Event`、`PresEvents_Event`、`SldEvents_Event`）补源接口 IID 的 `[Guid]`，配合 `[ComImport]` 解决 `CS0596` |
| ILSpy 元数据特性 | 删除 C# 无法表达的 `[NullableContext]` / `[Nullable]` / `[TupleElementNames]` / `[Embedded]` |
| C# 6 语法 | `stackalloc` 改为局部变量声明初始化；struct 内 lambda 不能捕获 `this` 成员（`CS1673`），在 `ConfiguratorExtensions.cs` 2 处先复制到局部变量；修正 goto 控制流导致的 `CS0165`；property setter 中误写的 `text` 改回 `value` |
| IL 不可表达结构 | 仅返回类型不同的重载、与 enclosing type 同名的成员、泛型参数与包含类型同名等，采用重命名并同步修正调用点 |
| ILSpy 误判 | `PictureUploadRequest` / `AttachmentUploadRequest` 的 `FileKey` 原 IL 是 `.override`，被反编译成显式接口实现，改回 `public override` |
| 缺失类型 | 新增 `ComponentModel\DebuggingProperties.cs`（本副本路径）：原版 `Cvte.Net.dll` 有指向 `[Cvte.Core.cc]Cvte.ComponentModel.DebuggingProperties::get_IsDebug()` 的 TypeRef，但随包分发的 `Cvte.Core.cc.dll` 已不含该类型。|

### 单程序集整合（本副本）

- 以 `PPTService` / `PPTService.Collection` 为入口做类型级调用闭包分析，从原 6 个 `Cvte.*` 项目的 595 个 `.cs` 中取出可达部分，再用编译错误迭代补齐扩展方法、注解、被间接引用的类型，最终 **152 个文件**合并为新项目 `PPTService.Core`。
- 所有 CVTE 命名空间统一到 `PPTService.Core` 前缀下（保留原子层级，如 `Cvte.Windows` → `PPTService.Core.Windows`），主程序只需把 `using` 换成新命名空间；`.sln` 与各 `.csproj` 的 `Cvte.*` 项目引用已全部移除，`Cvte.*.dll` 不再生成。
- `lib\` 下的 `Newtonsoft.Json` / `linq2db` 等为第三方依赖，保留；`setup\Plugins\` 下的 `Cvte.ProcessKiller.dll`、`Cvte.FireWall.dll` 是 NSIS 插件（非 CVTE 类库），保留。
- `PPTService.Core\` 内源码按命名空间扁平化分目录（如 `Net.WebServices.EventReport\`、`Windows.Threading\`、`WebService.Internal\`），目录名与 `PPTService.Core.*` 命名空间一一对应，路径中不再含 `Cvte.` 字样；`csproj` 的 `Compile` 路径已同步更新。

### 运行行为修正

- LinqToDB 表名：`UserEntity` 是没有 `TableAttribute` 的实体，`GetTable<T>()` 默认以类名作表名。重命名后显式加 `[Table("Class26")]` 锁定原映射，避免 `User.db` 查表失败。
- 进度环收尾：`LoadingView.xaml.cs` 在跑到 100% 时不再执行 `Progress = 0` / `ClearData()`（原版会闪一下 0%），改为修正资源键为 `StoryBoard.HiddenRootGridImmediately` 并直接 `LoadingScreen<LoadingView>.Current.Hide()`；同时同步结束 `DispatcherFrame` 并保留 30ms 兜底，消除概率性卡在 99% 的卡死。

### 动态构建版本号与界面调整

- **全局版本号（build 号每次编译自动 +1）**：逻辑集中在仓库根 `Version.targets`，由 `PPTService`、`PPTService.Core`、`PPTService.Collection` 三个 csproj 共同导入。仓库根 `build.number` 保存**本次构建使用的 build 号**，完整版本 = `VersionPrefix`（`1.0.0`）+ build 号，例如 `1.0.0.799`。构建**成功后**由主程序项目（`IsVersionOwner=true`）把 `build.number` `+1` 写回，供下一次构建使用。递增放在构建之后，是为了让同一次构建（含 `/m` 并行、项目依赖顺序）三个项目都读到同一个值；失败的构建不消耗版本号。三个 csproj 均设 `DisableFastUpToDateCheck=true`，避免 VS 快速更新检查跳过某个项目造成版本落后一次构建（代价是每次 Build 都会重新编译）。
- **三个程序集同源**：`Version.targets` 在每个项目的 `Properties\GitVersionInfo.g.cs` 写入相同的 `ProductVersion` / `Hash` / `InformationalVersion` 常量（namespace `PPTService`，各程序集各自 internal）。三个 `AssemblyInfo.cs` 的 `AssemblyVersion` / `AssemblyFileVersion` / `AssemblyInformationalVersion` 全部引用这些常量，并加 `[AssemblyMetadata("CommitHash", ...)]`。所以 `PPTService.exe` 与两个 dll（`PPTService.Core.dll`、`PPTService.Collection.dll`）不再固定 `1.0.0.0`，文件版本跟随全局版本号，产品版本都带 git 短 hash，例如 `1.0.0.799 (a1b2c3d4)`。
- **git hash**：构建时执行 `git rev-parse --short=8 HEAD` 注入 8 位短 hash，**非 git 仓库 / 无 commit / git 不可用**统一回退 `unknown`。三个项目各保留一份 `unknown` 默认生成文件，脱离 git 也能直接编译；`AssemblyInfo.cs` 也不再写死 `1.0.0.795 (37e1afca)`。
- **NSIS 同源**：主程序项目同时生成 `setup\Version.nsh`（`!define PRODUCT_VERSION`），`setup\PPTService.nsi` 用 `!include "${__FILEDIR__}\Version.nsh"` 引入；安装包 `Name` / `VIProductVersion` / 注册表 `DisplayVersion` 与程序集版本一致，加载环文字、工具栏弹窗也读同一份常量。
- **开关**：`-p:SkipVersionIncrement=true` 本次不递增、`-p:BuildNumberOverride=900` 强制本次版本且不改 `build.number`、`-p:GitShortHashOverride=xxxx` 指定 hash、`-p:GitWorkingDirectory=<路径>` 指定 git 探测目录。目标带 `DesignTimeBuild != true` 条件，避免 VS 设计时构建误递增。只使用 MSBuild 4.0 / VS2015 支持的任务参数（`WriteOnlyWhenDifferent` 是 15+ 新增参数，在 VS2015 会报「任务不支持该参数」，已移除）。
- **CI 发布**：`.github\workflows\build.yml` 在编译后读取 `setup\Version.nsh`（原版仓库回退到 `PPTService.nsi` 里的 `PRODUCT_VERSION`）得到版本号，Release 的 **tag / 标题 / 说明都使用该版本号**，不再使用 `latest` 滚动 tag；同名版本重跑时改为覆盖已有 Release 的安装包附件。`build.number` 的改动需要提交回仓库才能持续递增，否则 CI 每次都产出同一个版本（此时 tag 相同，走覆盖逻辑）；也可在 CI 传 `-p:BuildNumberOverride=<run number>`。
- 加载转圈（`LoadingView.xaml`）底部的版权文字改为显示 `GitVersionInfo.InformationalVersion`。
- 工具栏原「希沃白板5推荐下载」按钮改为弹出当前版本号（`MessageBox`）；`View_\RecommendationWindow.xaml` 与 `.xaml.cs` 已从项目移除，csproj 的 Compile / Page 条目同步删除。项目自带的 `MessageWindow` 会在 2 秒后自动关闭，不适合展示版本号，故未采用。

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。
