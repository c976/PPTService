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
- PPT打开放映加载界面100%到0%问题：`LoadingView.xaml.cs` 沿用原版的 `StoryBoard.HiddenRootGridImmediatly`，XAML 真实键为 `...Immediately`。补齐拼写虽可消除偶发的「100%→0%」显示瑕疵，但实测会引入概率性卡在 99% 的卡死，故未修复。

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。
