# PPTServiceUltra

基于 PPTServiceUltra 1.0.0.795 二次开发增强版本

目标框架 Visual Studio 2015 .Net Framework 4.5

## 目录结构

```
\
├── PPTServiceUltra.sln                  解决方案
├── Version\                        公共文件（版本与程序集信息：Version.targets / build.ver / AssemblyInfo.Shared.cs 等）
├── ReadMe.md
├── lib\                            第三方依赖 DLL
├── PPTServiceUltra\                     主程序
├── PPTServiceUltra.Core\                核心库
├── PPTServiceUltra.Collection\          数据集合与访问
├── PPTServiceUltra.Defender\            守护进程
├── PPTServiceUltra.Ink\                 批注实现库
├── PPTServiceUltra.Ink.Core\            批注核心层
├── PPTServiceUltra.Ink.Hardware\        希沃触控 / 智能笔插件（可选）
├── PPTServiceUltra.Ink.Hardware.Core\   触控插件依赖（可选）
├── PPTServiceUltra.Hardware\            USB/HID MCU 硬件插件（可选）
├── PPTServiceUltra.NetShared\           二维码分享插件（可选）
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

- **智能笔**：`PPTServiceUltra.Ink.Hardware.dll`、`PPTServiceUltra.Ink.Hardware.Core.dll` 为可选插件，提供原始触控与智能笔压感能力。
- **MCU**：`PPTServiceUltra.Hardware.dll` 承载 USB/HID MCU 检测与安卓笔禁用/恢复命令；主程序只保留 `McuService` 调用门面，通过反射调用，插件缺失时安全降级为「不操作」。
- **二维码分享**：`PPTServiceUltra.NetShared.dll` 承载二维码分享所需的网络能力，主程序通过插件方式调用。

### 修复的问题

- **开场动画资源名拼写错误**：`LoadingView` 中有三处把动画资源键写成 `StoryBoard.HiddenRootGridImmediatly`（`Immediately` 少了一个 `e`），而 XAML 中实际定义的键名为 `StoryBoard.HiddenRootGridImmediately`。原版因此找不到资源、开场加载界面的隐藏动画不会触发。现已将三处引用统一修正为正确拼写。
- **开场进度条收尾逻辑**：原版进度跑到 100% 后会先把进度归零并清空数据（`Progress = 0` / `ClearData()`），界面会先闪一下 0% 再关闭；而消息循环依赖淡出动画的 `Completed` 回调或定时器来结束，叠加上面的拼写错误使淡出动画从未触发，导致加载界面概率性卡在 99% 不关闭。现改为跑满后直接淡出并隐藏，不再归零闪 0%，并同步结束消息循环（不再依赖动画回调 / 定时器），消除卡在 99% 的现象，同时补上消息循环的 `_frame` 空引用保护。

### 与SeewoIwbAssistant（SIA）相比的优势？

- 高度可以自定义的界面和功能
- 完整的深色浅色主题，以及界面微调
- 占用体积小，程序仅~2MB。
- 基本上实现了SIA的所有功能（除了投屏）
- 自实现批注比SIA的稳定
- 一些功能实现插件化

## 许可协议

本项目仅供个人学习、研究，严禁用于任何商业盈利或非法破解；使用者需自行承担使用本项目可能导致的软件不稳定、数据丢失或被杀毒软件误报等风险，作者不对由此产生的任何直接或间接法律纠纷与损失承担任何责任。