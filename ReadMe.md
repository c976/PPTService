# PPTServiceUltra

基于 PPTService 1.0.0.795 版本二次开发增强版本

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

## 修改

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
