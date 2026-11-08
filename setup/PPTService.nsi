; =============================================================================
;  PPT小工具安装程序
;
;  安装界面：nsSkinEngine 自定义皮肤
;  卸载界面：MUI 现代界面（1.67 以上兼容）
;
;  默认不启用、但以注释形式完整保留的功能：
;    - Addon 目录内附加文件的打包与写入
;    - 更新组件与静默更新服务的安装（资源需自行放入 Addon 目录）
;    - Microsoft .NET Framework 4.5 检测 / 安装 / 重启
;    - 安装目录体积统计（EstimatedSize）
;    - 防火墙白名单规则
;  需要启用时，取消对应注释即可。
; =============================================================================

Unicode true
SetCompressor /SOLID lzma
SetCompressorDictSize 32

!include "MUI.nsh"
!include "WinMessages.nsh"
!include "LogicLib.nsh"
!include "WinVer.nsh"
!include "FileFunc.nsh"

; 皮肤引擎插件使用随包提供的版本，其导出 API 与 NSIS 自带版本不同
!addplugindir "${__FILEDIR__}\Plugins\x86-unicode"

; -----------------------------------------------------------------------------
;  产品定义
; -----------------------------------------------------------------------------
!define PRODUCT_NAME          "PPT小工具"
!define PRODUCT_NAME_EN       "PPTService"
!define PRODUCT_VERSION       "1.0.0.795"
!define PRODUCT_PUBLISHER     "Seewo"
!define PRODUCT_WEB_SITE      "www.seewo.com"
!define PRODUCT_GUID          "{958764e5-bbb2-43c1-1234-8d454abd130a}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_UNINST_KEY    "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_GUID}"
!define PRODUCT_DIR_REGKEY    "Software\Microsoft\Windows\CurrentVersion\App Paths\${PRODUCT_NAME_EN}.exe"
!define APP_REGKEY            "SOFTWARE\Seewo\PPTService"
!define SUPPORT_DIR           "Main\SupportFiles"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
UninstallCaption "${PRODUCT_NAME} 卸载"
BrandingText "${PRODUCT_PUBLISHER}"
OutFile "${__FILEDIR__}\PPTServiceSetup.exe"
Icon "${__FILEDIR__}\Skin\Icons\Install.ico"
UninstallIcon "${__FILEDIR__}\Skin\Icons\Uninstall.ico"

InstallDir "$PROGRAMFILES\Seewo\PPTService"
; 升级时由上一版本的卸载信息定位安装目录
InstallDirRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString"

ShowInstDetails show
ShowUnInstDetails show
RequestExecutionLevel admin

VIProductVersion "${PRODUCT_VERSION}"
VIAddVersionKey /LANG=2052 "ProductName"      "${PRODUCT_NAME_EN}"
VIAddVersionKey /LANG=2052 "CompanyName"      "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=2052 "FileDescription"  "${PRODUCT_NAME} 安装程序"
VIAddVersionKey /LANG=2052 "FileVersion"      "${PRODUCT_VERSION}"
VIAddVersionKey /LANG=2052 "ProductVersion"   "${PRODUCT_VERSION}"
VIAddVersionKey /LANG=2052 "LegalCopyright"   "Copyright (C) 2020 Guangzhou Shirui Electronics Co.,Ltd, All Rights Reserved."
VIAddVersionKey /LANG=2052 "LegalTrademarks"  "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=2052 "OriginalFilename" "PPTServiceSetup.exe"

; -----------------------------------------------------------------------------
;  MUI 现代界面（卸载部分使用）
; -----------------------------------------------------------------------------
!define MUI_ICON   "${__FILEDIR__}\Skin\Icons\Install.ico"
!define MUI_UNICON "${__FILEDIR__}\Skin\Icons\Uninstall.ico"

; 皮肤页面自行处理退出确认，这里不启用 MUI_ABORTWARNING，避免重复弹框。
; MUI 会接管 .onGUIInit / .onUserAbort，皮肤侧的初始化通过下面的挂接点调用。
!define MUI_CUSTOMFUNCTION_GUIINIT SkinGUIInit

Function SkinGUIInit
  ; 内页控件字体与配色（皮肤窗口背后的 NSIS 主窗口）
  GetDlgItem $0 $HWNDPARENT 1037
  CreateFont $1 "宋体" "9" 700
  SendMessage $0 ${WM_SETFONT} $1 0
  SetCtlColors $0 "" 0xFFFFFF
  GetDlgItem $0 $HWNDPARENT 1028
  SetCtlColors $0 /BRANDING ""
  GetDlgItem $0 $HWNDPARENT 1256
  SetCtlColors $0 /BRANDING ""
  SendMessage $0 ${WM_SETTEXT} 0 "STR:${PRODUCT_PUBLISHER} "
FunctionEnd

; 卸载进度窗口：隐藏 MUI 头部条中的标题与副标题文字
!define MUI_CUSTOMFUNCTION_UNGUIINIT un.HideUnHeader

Function un.HideUnHeader
  GetDlgItem $0 $HWNDPARENT 1037
  ShowWindow $0 0
  GetDlgItem $0 $HWNDPARENT 1038
  ShowWindow $0 0
FunctionEnd

!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "SimpChinese"

; 新版安装页面序列（可选）。默认使用自定义皮肤页面，若改用下列页面，
; 请把下方 Page custom / Page instfiles 两行注释掉，并取消这里的注释。
/*
; !define MUI_ABORTWARNING
; !define MUI_FINISHPAGE_RUN "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"
; !insertmacro MUI_PAGE_WELCOME
; !insertmacro MUI_PAGE_DIRECTORY
; !insertmacro MUI_PAGE_INSTFILES
; !insertmacro MUI_PAGE_FINISH
*/

; -----------------------------------------------------------------------------
;  文案（简体中文 2052）
; -----------------------------------------------------------------------------
LangString STR_AppName          2052 "PPT小工具"
LangString STR_MenuFolder       2052 "希沃软件"
LangString STR_SelectDir        2052 "选择要安装 PPT小工具 的文件夹位置: "
LangString STR_ExitConfirm      2052 "你真的要退出安装吗？"
LangString STR_MutexInstaller   2052 "本产品已经有一个安裝向导正在运行！"
LangString STR_MutexUninstaller 2052 "本产品已经有一个卸载向导正在运行！"
LangString STR_Win7Required     2052 "抱歉，本程序只对 Windows7 或者以上版本的系统提供支持。"
LangString STR_NeedAdmin        2052 "请使用管理员权限执行本程序。"
LangString STR_RootPath         2052 "不能将软件安装在驱动器根路径。"
LangString STR_ProcessBusy      2052 "检测到安装路径下有进程尚未退出，忽略此问题可能导致安装异常。$\n是否重试关闭这些进程？"
LangString STR_UninstallConfirm 2052 "你确定要卸载本产品吗？"
LangString STR_UninstallDone    2052 "${PRODUCT_NAME} 已成功地从你的计算机移除。"
LangString STR_InstallingOld    2052 "正在卸载旧版本……"
LangString STR_FrameworkBusy    2052 "正在安装 Microsoft .Net Framework，预计需要五分钟..."
LangString STR_FrameworkOK      2052 "Microsoft .NET Framework 安装成功..."
LangString STR_FrameworkReboot  2052 "Microsoft .NET Framework 安装成功，请重启你的电脑..."
LangString STR_FrameworkFail    2052 "警告：Microsoft .NET Framework 安装失败，你可能需要重新安装..."
LangString STR_FrameworkSkip    2052 "检测到你已安装 Microsoft .NET Framework V$R5，无需再次安装..."
LangString STR_RebootAsk        2052 "需要重启你的电脑以确保程序正常运行，是否立即重启？"
LangString STR_InternalError    2052 "安装包存在一个内部错误。请下载最新版本并重新安装。"

; -----------------------------------------------------------------------------
;  变量
; -----------------------------------------------------------------------------
Var SkinDir          ; $TEMP\Seewo\PPTService
Var SkinPage         ; $SkinDir\2052
Var SkinHandle       ; 皮肤窗口句柄
Var RunOnFinish      ; 完成页「立即运行」勾选状态
Var ProcessDir       ; 进程检查的目标目录
Var OldUninstaller   ; 上一版本的卸载程序路径
Var OldInstallDir    ; 上一版本的安装目录
; 启用 .NET 4.5 检测 / 安装 / 重启流程时，需同时取消下面两行声明的注释
; Var FrameworkVersion ; .NET 版本
; Var NeedReboot       ; 是否需要重启

; -----------------------------------------------------------------------------
;  安装日志：$INSTDIR\install.log（与卸载程序同一目录）
;  LogSet / LogText 需要 NSIS_CONFIG_LOG 日志构建的 NSIS（编译器与 Stubs 配套），
;  日志由引擎自动逐指令写入（File / CreateDirectory / Exec / WriteRegStr 等）。
; -----------------------------------------------------------------------------
!macro EasiLog TEXT
  LogText '${TEXT}'
!macroend

; -----------------------------------------------------------------------------
;  页面：安装使用自定义皮肤，卸载使用 MUI 页面
; -----------------------------------------------------------------------------
Page custom SkinPage_Create "" /ENABLECANCEL
Page instfiles "" InstFiles_Show ""

; =============================================================================
;  .onInit —— 环境检查 + 释放皮肤资源
; =============================================================================
Function .onInit
  ; DPI 感知
  System::Call "SHCore::SetProcessDpiAwareness(i 2)i.R0"
  Pop $R0

  SetShellVarContext all

  ; 双 Mutex 防重（Local + Global）
  System::Call 'kernel32::CreateMutex(i 0, i 0, t "Local\PPTService_Installer") i .R7 ?e'
  Pop $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION|MB_TOPMOST "$(STR_MutexInstaller)" /SD IDOK
    Abort
  ${EndIf}
  System::Call 'kernel32::CreateMutex(i 0, i 0, t "Global\PPTService_Installer_Global") i .R7 ?e'
  Pop $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION|MB_TOPMOST "$(STR_MutexUninstaller)" /SD IDOK
    Abort
  ${EndIf}

  ; Windows 7 及以上
  ${IfNot} ${AtLeastWin7}
    MessageBox MB_OK|MB_ICONEXCLAMATION|MB_TOPMOST "$(STR_Win7Required)" /SD IDOK
    Abort
  ${EndIf}

  ; 管理员权限
  UserInfo::GetAccountType
  Pop $0
  ${If} $0 != "Admin"
    MessageBox MB_OK|MB_TOPMOST "$(STR_NeedAdmin)" /SD IDOK
    Abort
  ${EndIf}

  InitPluginsDir

  ; 释放皮肤资源到 $TEMP\Seewo\PPTService\2052 与 \Icons
  ; SetOutPath 使用字面路径，安装程序内记录的目标目录才是完整路径
  StrCpy $SkinDir "$TEMP\Seewo\PPTService"
  StrCpy $SkinPage "$SkinDir\2052"
  SetOutPath "$TEMP\Seewo\PPTService\2052"
  SetOverwrite on
  AllowSkipFiles off
  File "${__FILEDIR__}\Skin\2052\*.*"
  SetOutPath "$TEMP\Seewo\PPTService\Icons"
  File "${__FILEDIR__}\Skin\Icons\*.*"
  AllowSkipFiles on
FunctionEnd

; =============================================================================
;  皮肤页面
; =============================================================================
!macro BindSkinControl CTRL FUNC
  nsSkinEngine::FindControl /NOUNLOAD "${CTRL}"
  Pop $0
  ${If} $0 == 0
    GetFunctionAddress $1 ${FUNC}
    nsSkinEngine::OnControlBindNSISScript /NOUNLOAD "${CTRL}" $1
  ${EndIf}
!macroend

Function SkinPage_Create
  nsSkinEngine::InitNSISSkinEngine /NOUNLOAD "$SkinPage" "Installer.xml" "tab_Page" "false" \
      "${PRODUCT_NAME_EN}" "$SkinDir\Icons\Install.ico" "false"
  Pop $SkinHandle

  nsSkinEngine::InitNSISMessageBox /NOUNLOAD "ExitConfirm.xml" "pnl_ExitConfirm" \
      "lbl_Tip" "btn_Close" "btn_Yes" "btn_No"

  !insertmacro BindSkinControl "btn_QuickInstall"  OnQuickInstall
  !insertmacro BindSkinControl "btn_Setting"       OnSettings
  !insertmacro BindSkinControl "btn_StartInstall"  OnQuickInstall
  !insertmacro BindSkinControl "btn_ChangePath"    OnChangePath
  !insertmacro BindSkinControl "btn_Back"          OnBack
  !insertmacro BindSkinControl "btn_Finish"        OnFinish
  !insertmacro BindSkinControl "btn_Min"           OnMinimize
  !insertmacro BindSkinControl "btn_Close"         OnClose

  nsSkinEngine::SetControlData /NOUNLOAD "edt_Dir" "$INSTDIR" "Text"
  nsSkinEngine::RunNSISSkinEngine /NOUNLOAD
FunctionEnd

Function InstFiles_Show
  nsSkinEngine::FindControl /NOUNLOAD "progress_Install"
  Pop $0
  ${If} $0 == 0
    nsSkinEngine::NSISBindingProgress /NOUNLOAD "progress_Install" "lbl_ProgressPercent"
  ${EndIf}
FunctionEnd

; -----------------------------------------------------------------------------
;  皮肤按钮回调
; -----------------------------------------------------------------------------
Function AdvanceToInstall
  nsSkinEngine::SetControlData /NOUNLOAD "btn_Close" "false" "Visible"
  nsSkinEngine::NSISSendMessage /NOUNLOAD $SkinHandle "WM_NSISNEXT"
  nsSkinEngine::NSISStopMsgLoop /NOUNLOAD
FunctionEnd

Function OnQuickInstall
  nsSkinEngine::GetControlData /NOUNLOAD "edt_Dir" "Text"
  Pop $R0
  ${If} $R0 == ""
    StrCpy $R0 "$INSTDIR"
  ${EndIf}
  ; 去掉尾部反斜杠（盘符根路径除外）
  StrCpy $1 $R0 "" -1
  ${If} $1 == "\"
    StrCpy $R2 $R0 "" -1
    StrCpy $1 $R2 "" -1
    ${If} $1 != ":"
      StrCpy $R0 $R2
    ${EndIf}
  ${EndIf}
  ; 不允许安装到驱动器根路径
  StrCpy $1 $R0 1 1
  StrCpy $2 $R0 2
  ${If} $1 == ":"
  ${AndIf} $2 == "$R0"
    MessageBox MB_OK|MB_ICONEXCLAMATION|MB_TOPMOST "$(STR_RootPath)" /SD IDOK
    Return
  ${EndIf}
  StrCpy $INSTDIR $R0
  Call AdvanceToInstall
FunctionEnd

Function OnSettings
  nsSkinEngine::SetControlData /NOUNLOAD "pnl_Default" "false" "Visible"
  nsSkinEngine::SetControlData /NOUNLOAD "pnl_Setting" "true"  "Visible"
FunctionEnd

Function OnBack
  nsSkinEngine::SetControlData /NOUNLOAD "pnl_Default" "true"  "Visible"
  nsSkinEngine::SetControlData /NOUNLOAD "pnl_Setting" "false" "Visible"
FunctionEnd

Function OnChangePath
  nsSkinEngine::SelectFolderDialog /NOUNLOAD "$(STR_SelectDir)" "$INSTDIR"
  Pop $R0
  ${If} $R0 == "-1"
    Return
  ${EndIf}
  ; 末级不是 PPTService 时自动追加
  StrLen $1 $R0
  IntOp $1 $1 - 11
  ${If} $1 >= 0
    StrCpy $2 $R0 11 $1
  ${Else}
    StrCpy $2 ""
  ${EndIf}
  StrCpy $3 "1"
  ${If} $R0 == "${PRODUCT_NAME_EN}"
    StrCpy $3 "0"
  ${ElseIf} $2 == "\${PRODUCT_NAME_EN}"
    StrCpy $3 "0"
  ${EndIf}
  ${If} $3 == "1"
    StrCpy $R0 "$R0\${PRODUCT_NAME_EN}"
  ${EndIf}
  StrCpy $INSTDIR $R0
  nsSkinEngine::SetControlData /NOUNLOAD "edt_Dir" "$INSTDIR" "Text"
FunctionEnd

Function OnFinish
  nsSkinEngine::GetControlData /NOUNLOAD "checkbox_RunNow" "Checked"
  Pop $RunOnFinish
  ${If} $RunOnFinish == 1
    Exec 'explorer "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"'
  ${EndIf}
  SendMessage $HWNDPARENT 0x0111 1 0
  nsSkinEngine::HideNSISSkinEngine
FunctionEnd

Function OnMinimize
  nsSkinEngine::MinNSISSkinEngine /NOUNLOAD
FunctionEnd

Function OnClose
  nsSkinEngine::ShowNSISMessageBox /NOUNLOAD "" "$(STR_ExitConfirm)"
  Pop $0
  ${If} $0 == 1
    nsSkinEngine::ExitNSISSkinEngine "True"
  ${EndIf}
FunctionEnd

; =============================================================================
;  安装前准备：升级流程 + 进程重试循环
; =============================================================================

; 结束目标目录中的进程；一次未清理干净则等待 1 秒重试，并允许用户选择继续重试
Function KillProcessesInDirectory
  Push $0
KPD_Retry:
  Cvte.ProcessKiller::KillProcessesByDirectory /NOUNLOAD "$ProcessDir"
  Pop $0
  ${If} $0 == 0
    Goto KPD_Done
  ${EndIf}
  Sleep 1000
  Cvte.ProcessKiller::KillProcessesByDirectory /NOUNLOAD "$ProcessDir"
  Pop $0
  ${If} $0 == 0
    Goto KPD_Done
  ${EndIf}
  MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "$(STR_ProcessBusy)" /SD IDCANCEL \
      IDRETRY KPD_Retry IDCANCEL KPD_Done
KPD_Done:
  Pop $0
FunctionEnd

Function PrepareInstall
  DetailPrint "正在进行安装前的准备工作……"

  ; 读取上一版本的卸载信息，若存在则先静默卸载旧版本
  ReadRegStr $OldUninstaller ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString"
  ${If} $OldUninstaller != ""
    DetailPrint "$(STR_InstallingOld)"
    nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "$(STR_InstallingOld)" "text"

    ${GetParent} "$OldUninstaller" $OldInstallDir
    ${If} $OldInstallDir != ""
    ${AndIf} ${FileExists} "$OldInstallDir\*.*"
      ; 旧目录中仍有进程时循环重试
      StrCpy $ProcessDir "$OldInstallDir"
      Call KillProcessesInDirectory
      ExecWait '"$OldUninstaller" /S _?=$OldInstallDir' $0
      RMDir /r "$OldInstallDir"
    ${EndIf}
  ${EndIf}

  ; 当前安装目录中仍有进程时循环重试
  StrCpy $ProcessDir "$INSTDIR"
  Call KillProcessesInDirectory
FunctionEnd

; =============================================================================
;  安装主体
; =============================================================================
Section "MainSection" SEC01
  CreateDirectory "$INSTDIR"
  Delete "$INSTDIR\install.log"
  LogSet on
  !insertmacro EasiLog 'EasiSetup: "BeforeInstall" Entered...'
  Call PrepareInstall
  !insertmacro EasiLog 'EasiSetup: "BeforeInstall" Left...'
  !insertmacro EasiLog 'EasiSetup: Files Entered...'

  DetailPrint "正在解压文件……"
  nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在解压文件……" "text"

  ; --- 根目录：配置文件 ---
  !insertmacro EasiLog 'EasiSetup: 安装程序文件...'
  SetOutPath "$INSTDIR"
  SetOverwrite on
  File "${__FILEDIR__}\Config.ini"

  ; ---------------------------------------------------------------------------
  ;  附加文件（默认不启用）
  ;  把 Addon 目录内的全部文件与子目录按原有结构打包进安装程序，
  ;  安装时写入 $INSTDIR，不包含 Addon 这一层目录名。
  ;  需要启用时，取消下面两行注释（.keep 只是目录占位符，不写入）。
  ; ---------------------------------------------------------------------------
  ; SetOutPath "$INSTDIR"
  ; File /r /x ".keep" "${__FILEDIR__}\Addon\*.*"

  ; ---------------------------------------------------------------------------
  ;  更新组件（默认不安装；需要时取消注释）
  ;  资源位于 Addon\ 目录，默认不提供，需要时自行放入。
  ; ---------------------------------------------------------------------------
  ; File "${__FILEDIR__}\Addon\LibEasiUpdate.dll"
  ; File "${__FILEDIR__}\Addon\Myou.dll"
  ; File "${__FILEDIR__}\Addon\Update.config"
  ; File "${__FILEDIR__}\Addon\Updater.exe"
  ; File "${__FILEDIR__}\Addon\app-package.json"
  ; File "${__FILEDIR__}\Addon\msvcp120.dll"
  ; File "${__FILEDIR__}\Addon\msvcr120.dll"

  ; --- Main：解决方案 Release 编译产物（排除 pdb） ---
  SetOutPath "$INSTDIR\Main"
  File /x "*.pdb" "${__FILEDIR__}\..\PPTService\bin\Release\*.*"

  ; ---------------------------------------------------------------------------
  ;  随包分发的支持文件（默认不安装；需要时取消注释）
  ; ---------------------------------------------------------------------------
  ; SetOutPath "$INSTDIR\${SUPPORT_DIR}"
  ; File "${__FILEDIR__}\Addon\Main\SupportFiles\EasiUpdateSetup.exe"
  ; File "${__FILEDIR__}\Addon\Main\SupportFiles\dotnetfx45_full_x86_x64.exe"

  ; ---------------------------------------------------------------------------
  ;  Microsoft .NET Framework 4.5 检测 / 安装 / 重启（默认不启用）
  ;  启用时还需：取消上方 SupportFiles 的 File 注释，以及变量区
  ;  FrameworkVersion / NeedReboot 两行 Var 声明的注释。
  ; ---------------------------------------------------------------------------
  ; !insertmacro EasiLog 'EasiSetup: ThirdPackages Entered...'
  ; !insertmacro EasiLog 'EasiSetup: 安装 Microsoft .Net 4.5 ...'
  ;
  ;
  ; nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在安装 .Net Framework 4.5，预计需要五分钟..." "text"
  ; StrCpy $NeedReboot "false"
  ;
  ; ; 读取已安装的 .NET 版本
  ; ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Install"
  ; ${If} $0 == 1
  ;   ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ;   ${If}   $0 >= 394802
  ;     StrCpy $FrameworkVersion "4.6.2"
  ;   ${ElseIf} $0 >= 394254
  ;     StrCpy $FrameworkVersion "4.6.1"
  ;   ${ElseIf} $0 >= 393295
  ;     StrCpy $FrameworkVersion "4.6.0"
  ;   ${ElseIf} $0 >= 379893
  ;     StrCpy $FrameworkVersion "4.5.2"
  ;   ${ElseIf} $0 >= 378675
  ;     StrCpy $FrameworkVersion "4.5.1"
  ;   ${ElseIf} $0 >= 378389
  ;     StrCpy $FrameworkVersion "4.5.0"
  ;   ${Else}
  ;     StrCpy $FrameworkVersion "0.0.0"
  ;   ${EndIf}
  ; ${Else}
  ;   ; 依次回退检查 v3.5 / v3.0 / v2.0 / v1.1 / v1.0
  ;   ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Install"
  ;   StrCpy $FrameworkVersion "3.5"
  ;   ${If} $0 != 1
  ;     ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "InstallSuccess"
  ;     StrCpy $FrameworkVersion "3.0"
  ;     ${If} $0 != 1
  ;       ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Install"
  ;       StrCpy $FrameworkVersion "2.0.50727.832"
  ;       ${If} $0 != 1
  ;         ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" "Install"
  ;         StrCpy $FrameworkVersion "1.1.4322.573"
  ;         ${If} $0 != 1
  ;           ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v1.0" "Install"
  ;           StrCpy $FrameworkVersion "1.0.3705.0"
  ;           ${If} $0 != 1
  ;             StrCpy $FrameworkVersion "0.0.0"
  ;           ${EndIf}
  ;         ${EndIf}
  ;       ${EndIf}
  ;     ${EndIf}
  ;   ${EndIf}
  ; ${EndIf}
  ;
  ; ${If} $FrameworkVersion == "0.0.0"
  ;   DetailPrint "$(STR_FrameworkBusy)"
  ;   ExecWait '"$INSTDIR\${SUPPORT_DIR}\dotnetfx45_full_x86_x64.exe" /q /norestart' $0
  ;   ${If} $0 == 0
  ;     DetailPrint "$(STR_FrameworkOK)"
  ;   ${ElseIf} $0 == 3010
  ;     DetailPrint "$(STR_FrameworkReboot)"
  ;     StrCpy $NeedReboot "true"
  ;   ${Else}
  ;     DetailPrint "$(STR_FrameworkFail)"
  ;     MessageBox MB_OK|MB_ICONEXCLAMATION|MB_TOPMOST "$(STR_FrameworkFail)" /SD IDOK
  ;   ${EndIf}
  ; ${Else}
  ;   StrCpy $R5 "$FrameworkVersion"
  ;   DetailPrint "$(STR_FrameworkSkip)"
  ; ${EndIf}
  ;
  ; ${If} $NeedReboot == "true"
  ;   MessageBox MB_YESNO|MB_ICONQUESTION|MB_TOPMOST "$(STR_RebootAsk)" /SD IDNO IDNO SkipReboot
  ;   Reboot
  ; SkipReboot:
  ; ${EndIf}
  ;
  ; ---------------------------------------------------------------------------
  ;  静默更新服务（默认不安装；需要时取消注释）
  ; ---------------------------------------------------------------------------
  ; !insertmacro EasiLog 'EasiSetup: 安装静默更新服务...'
  ; SetOutPath "$INSTDIR\${SUPPORT_DIR}"
  ; ExecWait '"$INSTDIR\${SUPPORT_DIR}\EasiUpdateSetup.exe" /S' $0
  ; ${If} $0 == 2
  ;   MessageBox MB_OK "$(STR_InternalError)"
  ; ${EndIf}
  ; !insertmacro EasiLog 'EasiSetup: ThirdPackages Left...'

  !insertmacro EasiLog 'EasiSetup: Files Left...'

  ; --- 快捷方式 / 注册表 ---
  !insertmacro EasiLog 'EasiSetup: "ShortCuts" Entered...'
  Call CreateShortcuts
  !insertmacro EasiLog 'EasiSetup: "ShortCuts" Left...'
  !insertmacro EasiLog 'EasiSetup: "Registry" Entered...'
  Call WriteRegistry
  !insertmacro EasiLog 'EasiSetup: "Registry" Left...'

  ; --- 卸载程序 ---
  !insertmacro EasiLog 'EasiSetup: "AfterInstall" Entered...'
  DetailPrint "正在生成卸载程序……"
  nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在生成卸载程序……" "text"
  CreateDirectory "$INSTDIR"
  !insertmacro EasiLog 'EasiSetup: 发布卸载程序...'
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  System::Call "shell32.dll::SHChangeNotify(i, i, i, i) v (0x08000000, 0, 0, 0)"

  DetailPrint "正在进行最后的安装步骤……"
  nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在进行最后的安装步骤……" "text"
  !insertmacro EasiLog 'EasiSetup: "AfterInstall" Left...'
  LogSet off
SectionEnd

; -----------------------------------------------------------------------------
;  注册表
; -----------------------------------------------------------------------------
Function WriteRegistry
  DetailPrint "正在写注册表……"
  nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在写注册表……" "text"

  ; 卸载信息（App Paths 与控制面板）
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_DIR_REGKEY}" "" \
      "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName"     "$(STR_AppName)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion"  "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher"       "${PRODUCT_PUBLISHER}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout"    "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon"     "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"

  ; ---------------------------------------------------------------------------
  ;  安装目录体积统计（EstimatedSize，单位 KB）——默认不写入；需要时取消注释
  ; ---------------------------------------------------------------------------
  ; ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  ; WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "EstimatedSize" "$0"

  WriteRegStr HKLM "${APP_REGKEY}" "type"    "1"
  WriteRegStr HKLM "${APP_REGKEY}" "path"    "$INSTDIR"
  WriteRegStr HKLM "${APP_REGKEY}" "version" "${PRODUCT_VERSION}"
  WriteRegStr HKLM "${APP_REGKEY}" "code"    "${PRODUCT_GUID}"
  WriteRegStr HKLM "${APP_REGKEY}" "ExePath" "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"
  WriteRegStr HKLM "${APP_REGKEY}" "admin"   "0"

  ; 开机自启
  WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "${PRODUCT_NAME_EN}" \
      "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"

  ; ---------------------------------------------------------------------------
  ;  防火墙白名单（默认不启用；同时取消卸载段中的对应注释）
  ; ---------------------------------------------------------------------------
  ; !insertmacro EasiLog 'EasiSetup: "FireWall" Entered...'
  ; DetailPrint "正在添加防火墙规则……"
  ; nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在添加防火墙规则……" "text"
  ; Cvte.FireWall::AddFireWallWhiteListByDirectory "$INSTDIR"
  ; !insertmacro EasiLog 'EasiSetup: "FireWall" Left...'
FunctionEnd

; -----------------------------------------------------------------------------
;  快捷方式
; -----------------------------------------------------------------------------
Function CreateShortcuts
  DetailPrint "正在生成快捷方式……"
  nsSkinEngine::SetControlData /NOUNLOAD "lbl_Detail" "正在生成快捷方式……" "text"
  SetShellVarContext all
  SetOutPath "$INSTDIR"
  CreateDirectory "$SMPROGRAMS\$(STR_MenuFolder)\$(STR_AppName)"
  CreateShortCut "$SMPROGRAMS\$(STR_MenuFolder)\$(STR_AppName)\$(STR_AppName).lnk" \
      "$INSTDIR\Main\${PRODUCT_NAME_EN}.exe"
FunctionEnd

; =============================================================================
;  卸载（MUI 现代界面）
; =============================================================================
Section "Uninstall"
  SetShellVarContext all
  ; 详细日志：引擎逐指令写入窗口详情区与 $INSTDIR\install.log
  LogSet on

  ; 卸载前结束主程序进程（不在界面与日志中输出）。
  ; 按进程名结束，避免升级流程以 /S _?= 调用时误杀 $INSTDIR 内的卸载器自身。
  Cvte.ProcessKiller::KillProcessesByName /NOUNLOAD "${PRODUCT_NAME_EN}.exe"
  Pop $0
  Sleep 500

  ; ---------------------------------------------------------------------------
  ;  删除防火墙白名单规则（安装时启用白名单后，取消此注释）
  ; ---------------------------------------------------------------------------
  ; Cvte.FireWall::DeleteFireWallWhiteListByDirectory "$INSTDIR"

  Delete "$SMPROGRAMS\$(STR_MenuFolder)\$(STR_AppName)\$(STR_AppName).lnk"
  RMDir "$SMPROGRAMS\$(STR_MenuFolder)\$(STR_AppName)"
  RMDir "$SMPROGRAMS\$(STR_MenuFolder)"

  DeleteRegValue HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "${PRODUCT_NAME_EN}"
  DeleteRegKey HKLM "${APP_REGKEY}"
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_DIR_REGKEY}"

  Delete "$INSTDIR\Uninstall.exe"
  Delete "$INSTDIR\Config.ini"
  RMDir /r "$INSTDIR\Main"
  RMDir /r "$INSTDIR\${SUPPORT_DIR}"
  RMDir /r "$INSTDIR"

  ; 结束日志并删除 install.log，最后移除安装目录
  LogSet off
  Delete "$INSTDIR\install.log"
  RMDir "$INSTDIR"

  ; 停留在卸载窗口，用户点「关闭」后才弹出完成提示
  SetAutoClose false
SectionEnd

Function un.onInit
  ; 升级流程以 /S 调用卸载器：跳过确认框，静默完成卸载
  IfSilent SilentUninstall 0
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(STR_UninstallConfirm)" IDYES +2
  Abort
  SilentUninstall:
FunctionEnd

Function un.onUninstSuccess
  IfSilent SilentDone 0
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(STR_UninstallDone)"
  SilentDone:
FunctionEnd
