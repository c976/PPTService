# PPTService

基于 PPTService 1.0.0.795 版本反编译的代码

目标框架 Visual Studio 2015 .Net Framework 4.5

## 目录结构

```
\
├── PPTService.sln
├── ReadMe.md
├── lib\                        
├── PPTService\                 
├── Cvte.Core.cc\
├── Cvte.Mcu\
├── Cvte.Net\
├── Cvte.Pipe.Http\             
├── Cvte.WebService\            
├── Cvte.Windows.cc\
├── PPTService.Collection\
└── setup\
```

## 特殊修改

> 除本节列出的处理外，源码尽量保持反编译 / 去混淆输出原样，不改写业务逻辑。

### 反混淆与源码重建

原版 8 个程序集中有 4 个被 **SmartAssembly 7.5.1.4370** 混淆：`Cvte.Core.cc.dll`、`Cvte.Pipe.Http.dll`、`Cvte.WebService.dll`、`Cvte.Windows.cc.dll`。直接反编译会产出依赖元数据 token 的 `MemberRefsProxy` 空壳、被合并进同一个 `dynamic` 字段的多个属性，以及不可打印的类型名与命名空间。这样编译虽能通过，运行行为却不正确（二维码分享等路径会失败）。

处理方式：

1. `de4dot -p sa` 反混淆，还原代理调用与加密字符串；
2. `ilspycmd -p -lv CSharp6` 重新生成项目源码，沿用各 csproj 的 `LangVersion 6`，不改构建配置。

去混淆后的替身名按语义重命名：`ns0..ns9` 目录合并为 `Internal\`，命名空间统一为 `Cvte.<项目>.Internal`，11 个类型改为可读名（如 `PictureUploadRequest`、`AccountSettings`、`SignatureUrlEncoder`、`UserEntity`、`DataContractJsonHelper`）。de4dot 解密后失去调用者的 SA 字符串加密辅助类（`SmartAssembly.*`、8 个空 `AttributeN`、字符串解密表）已删除。

### 编译兼容处理

以下均为让反编译源码在 .NET Framework 4.5 / C# 6 下通过编译所必需的最小修补，不改变语义：

| 类别 | 处理 |
|---|---|
| COM Interop | 4 个事件接口（`EApplication_Event`、`MasterEvents_Event`、`PresEvents_Event`、`SldEvents_Event`）补源接口 IID 的 `[Guid]`，配合 `[ComImport]` 解决 `CS0596` |
| ILSpy 元数据特性 | 删除 C# 无法表达的 `[NullableContext]` / `[Nullable]` / `[TupleElementNames]` / `[Embedded]` |
| C# 6 语法 | `stackalloc` 改为局部变量声明初始化；struct 内 lambda 不能捕获 `this` 成员（`CS1673`），在 `ConfiguratorExtensions.cs` 2 处先复制到局部变量；修正 goto 控制流导致的 `CS0165`；property setter 中误写的 `text` 改回 `value` |
| IL 不可表达结构 | 仅返回类型不同的重载、与 enclosing type 同名的成员、泛型参数与包含类型同名等，采用重命名并同步修正调用点 |
| ILSpy 误判 | `PictureUploadRequest` / `AttachmentUploadRequest` 的 `FileKey` 原 IL 是 `.override`，被反编译成显式接口实现，改回 `public override` |
| 缺失类型 | 新增 `Cvte.Core.cc\Cvte.ComponentModel\DebuggingProperties.cs`：`Cvte.Net.dll` 有指向 `[Cvte.Core.cc]Cvte.ComponentModel.DebuggingProperties::get_IsDebug()` 的 TypeRef，但随包分发的 `Cvte.Core.cc.dll` 已不含该类型。|

### 运行行为修正

- LinqToDB 表名：`UserEntity` 是没有 `TableAttribute` 的实体，`GetTable<T>()` 默认以类名作表名。重命名后显式加 `[Table("Class26")]` 锁定原映射，避免 `User.db` 查表失败。
- 进度环 100% 归零：`LoadingView.xaml.cs` 的资源键原写作 `StoryBoard.HiddenRootGridImmediatly`，而 XAML 真实键是 `...Immediately`（0 秒动画，`Completed` 回调结束 `PushFrame`）。键不匹配导致动画从不触发，只剩 `DispatcherTask.Start(30.0, …)` 的 30ms 兜底；期间 `ClearData()` 已把界面写成 `0%`，WPF 抢到一帧就看到「100%→0% 再关闭」，因此表现为概率性问题。这是原版代码问题，这是额外修复。

### 已知差异与构建注意

- 这 4 个类库是重新编译产物，与原版 DLL 二进制不同；`smethod_N` / `method_N` 等方法名仍是反编译残留，未做重命名。
- `Cvte.Pipe.Http\Internal\Md5Helper.cs` 当前无引用，但属业务工具类，保留。
- 主项目 `PPTService` 是老式 WPF 工程，请**用 Visual Studio 的 MSBuild 构建**；`dotnet build` 不生成 XAML 的 `.g.cs`，会报 52 个 `IComponentConnector` 相关 `CS0535`（工具差异，非源码错误）。
- 曾怀疑重编译的 `Cvte.Core.cc.dll` 导致进度环归零，实测该现象是概率性竞态，与 DLL 版本无关，无需替换 DLL 或修改工程引用。

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。
