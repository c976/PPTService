# PPTServiceUltra

基于 PPTService 1.0.0.795 二次开发增强版本

目标框架 Visual Studio 2015 .Net Framework 4.5

## 目录结构

```
\
├── PPTService.sln                  解决方案
├── Version\                        公共文件（版本与程序集信息：Version.targets / build.ver / AssemblyInfo.Shared.cs 等）
├── ReadMe.md
├── lib\                            第三方依赖 DLL
├── PPTService\                     主程序
├── PPTService.Core\                核心库
├── PPTService.Collection\          数据集合与访问
├── PPTService.Defender\            守护进程
├── PPTService.Ink\                 批注实现库
├── PPTService.Ink.Core\            批注核心层
├── PPTService.Ink.Hardware\        希沃硬件插件（可选）
├── PPTService.Ink.Hardware.Core\   硬件插件依赖（可选）
├── PPTService.NetShared\           二维码分享插件（可选）
├── setup\                          NSIS 安装程序
└── .github\workflows\              Github Action build.yml（编译）/ release.yml（发布）
```

## 功能与改进

### 批注与教学

- **桌面批注白板**：内置自实现批注层，可在 Office 原生批注与自实现批注间切换；批注层始终悬浮在 PPT 放映窗口之上，支持「附着放映窗口」与「全局置顶」两种模式。
- **批注工具**：笔色 / 粗细可调、橡皮滑动清屏、二维码图片保存；批注随 PPT 翻页自动切换对应页笔迹。

### 工具栏与交互

- **工具栏收起 / 拉起**：结束放映按钮旁可收起整个工具栏（设置中可开关），收起后仅保留翻页按钮，屏幕底部居中出现拉起按钮；新放映与放映结束自动恢复展开态。
- **放大镜**：关灯 / 关闭按钮位于窗口底部居中横排并显示文字。
- **界面设置**：隐藏功能栏、翻页按钮位置、自动淡化、主题、工具栏透明度、缩略图开关、触控拖动排序等均可实时设置。
- **分辨率适配**：适配 1366×768、4K 100% 与 250% 缩放，工具栏列宽、图标与弹窗按分辨率自动调整。

### 插件化

- **希沃硬件**：`PPTService.Ink.Hardware.dll`、`PPTService.Ink.Hardware.Core.dll` 为可选插件，希沃硬件优化。
- **二维码分享**：`NetShared.dll` 承载二维码分享所需的网络能力，主程序通过插件方式调用。

## 基于原版修复的问题

在 1.0.0.795 基础上开发时，修正了原版遗留的缺陷：

- **开场动画资源名拼写错误**：`LoadingView` 中有三处把动画资源键写成 `StoryBoard.HiddenRootGridImmediatly`（`Immediately` 少了一个 `e`），而 XAML 中实际定义的键名为 `StoryBoard.HiddenRootGridImmediately`。原版因此找不到资源、开场加载界面的隐藏动画不会触发。现已将三处引用统一修正为正确拼写。

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。