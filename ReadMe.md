# PPTService

基于 PPTService 1.0.0.795 版本反编译的代码

目标框架 Visual Studio 2015 .Net Framework 4.5

## 目录结构

```
PPTService.Decompiled\
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
└── PPTService.Collection\
```

## 特殊修改

### 为让反编译输出能在 C# 6 下编译，做了以下处理：

- **COM Interop**：4 个事件接口（`EApplication_Event`、`MasterEvents_Event`、`PresEvents_Event`、`SldEvents_Event`）补上源接口 IID 的 `[Guid]`，与 `[ComImport]` 一同使用，解决 `CS0596`。
- **ILSpy 元数据特性**：删除 610 处无法用 C# 表达的 `[NullableContext]` / `[Nullable]` / `[TupleElementNames]` / `[Embedded]`。
- **C# 6 语法**：`stackalloc` 改为局部变量声明初始化；struct 内 lambda 通过局部变量捕获 display class；修正 goto 控制流导致的 `CS0165`；property setter 中 ILSpy 误写的 `text` 改回 `value`。
- **缺失类型**：新增 `Cvte.Core.cc\Cvte.ComponentModel\DebuggingProperties.cs`，补齐 `Cvte.Net.dll` 的 TypeRef 指向但当前 `Cvte.Core.cc.dll` 已不定义的类型。
- **IL 不可表达结构**：对仅返回类型不同的重载、与 enclosing type 同名的成员、泛型参数与包含类型同名等问题，采用重命名或条件编译排除，并同步修正调用点。

### 关于 SmartAssembly 混淆

`Cvte.Pipe.Http` 与 `Cvte.WebService` 原程序集使用 SmartAssembly 混淆。其 `HouseOfCards` / `MemberRefsProxy` / `Strings` 依赖**元数据 token 与运行时方法句柄**动态解析成员和还原字符串。重新编译后 token 必然变化，因此这部分无法从反编译文本原样重建。

处理方式：

- 原始不可编译的基础设施源码**完整保留**，但用条件编译包裹，不参与生成（这两个符号均未在任何项目中定义）：

  | 文件 | 符号 |
  |---|---|
  | `Cvte.Pipe.Http\-\-.cs` | `SA_ORIGINAL_DECOMPILED` |
  | `Cvte.Pipe.Http\Cvte.Pipe.Http\HttpPipe.cs` | `SA_ORIGINAL_DISPLAY_CLASSES`（仅 5 个未引用的 display class） |
  | `Cvte.WebService\-\-.cs` | `SA_ORIGINAL_DECOMPILED` |
  | `Cvte.WebService\SmartAssembly.HouseOfCards\MemberRefsProxy.cs` | `SA_ORIGINAL_DECOMPILED` |
  | `Cvte.WebService\SmartAssembly.HouseOfCards\Strings.cs` | `SA_ORIGINAL_DECOMPILED` |

- 生成编译兼容层替代：
  - `Cvte.WebService\SA_Compat.cs`（156 行）：`SA_Proxy`（强返回类型 + `params object[]` 静态方法，避免 `dynamic` 引发的 `CS1977` / `CS1963` / `CS1973`）、`SA_Helpers`、`SA_Attr<T>`、实体 shim 类型。
  - `Cvte.Pipe.Http\-\-.cs` 的 `#else` 分支：`SA_Attr<T>` / `SA_Json` / `SA_Md5`。
  - `HttpPipe.cs` 的 `SA_Execute<SA_TReq, SA_TRes>`、`HttpUtility.cs` 的 `SA_HtmlCallback` 等重命名，避开标识符冲突。

> **限制：兼容层是编译级桩，不是运行时还原。** `SA_Proxy` 的 73 个方法中有 68 处返回 `default(...)` 或 `null`。因此「能够编译通过」不等于「运行行为与原程序集一致」：字符串解密、成员代理相关路径在运行时可能返回空值。若目标是获得可运行的程序，需要对这两个程序集另做去混淆处理（例如先用 de4dot 处理 SmartAssembly，再重新反编译），或直接引用原始 DLL。

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。
