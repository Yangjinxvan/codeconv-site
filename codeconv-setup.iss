; codeconv 安装脚本 (Inno Setup 6/7)
; 四选一版本下拉 + 安装对应版本 .NET

[Setup]
AppId={{B3E0C6D1-4A8F-4C9B-9E2D-1F7A5C6B8D90}}
AppName=codeconv
AppVersion=10.10.0
AppVerName=codeconv 10.10.0
AppPublisher=codeconv
AppPublisherURL=https://codeconv.local
AppSupportURL=https://codeconv.local
DefaultDirName={autopf}\codeconv
DefaultGroupName=codeconv
SetupIconFile=.\image\logo-icon.ico
UninstallDisplayIcon={app}\logo-icon.ico
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=download\install
OutputBaseFilename=setup
LicenseFile=LICENSE
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern
SetupLogging=yes

[Languages]
Name: chinesesimplified; MessagesFile: compiler:Languages\ChineseSimplified.isl

[Messages]
chinesesimplified.WizardSelectComponents=选择版本

[Types]
Name: net10x64; Description: .NET 10 版本 (x64)
Name: net10x86; Description: .NET 10 版本 (x86)
Name: net6x64; Description: .NET 6 版本 (x64)
Name: net6x86; Description: .NET 6 版本 (x86)

[Components]
Name: net10x64; Description: .NET 10 版本 (x64); Types: net10x64
Name: net10x86; Description: .NET 10 版本 (x86); Types: net10x86
Name: net6x64; Description: .NET 6 版本 (x64); Types: net6x64
Name: net6x86; Description: .NET 6 版本 (x86); Types: net6x86

[Tasks]
Name: desktop_icon; Description: 创建桌面快捷方式; GroupDescription: 附加图标
Name: add_to_path; Description: 添加到系统 PATH 环境变量; GroupDescription: 环境变量

[Files]
Source: "net10.0\x64\*"; DestDir: "{app}"; Components: net10x64; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "net10.0\x86\*"; DestDir: "{app}"; Components: net10x86; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "net6.0\x64\*"; DestDir: "{app}"; Components: net6x64; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "net6.0\x86\*"; DestDir: "{app}"; Components: net6x86; Flags: ignoreversion recursesubdirs createallsubdirs
Source: ".\image\logo-icon.ico"; DestDir: "{app}"

[Icons]
Name: "{group}\codeconv"; Filename: "{app}\codeconv.exe"; IconFilename: "{app}\logo-icon.ico"; Components: net10x64 net10x86 net6x64 net6x86; Check: not NoStartMenuSelected
Name: "{autodesktop}\codeconv"; Filename: "{app}\codeconv.exe"; IconFilename: "{app}\logo-icon.ico"; Tasks: desktop_icon; Components: net10x64 net10x86 net6x64 net6x86

[Registry]
Root: HKCU; Subkey: "Software\codeconv"; ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; Flags: uninsdeletekey

[Run]
Filename: "{app}\codeconv.exe"; Description: "启动 codeconv"; Flags: nowait postinstall skipifsilent

[Code]
const
  EnvKey = 'Environment';
  EnvName = 'Path';
  Net10X64Url = 'https://dotnetcli.azureedge.net/dotnet/Runtime/10.0.9/dotnet-runtime-10.0.9-win-x64.exe';
  Net10X86Url = 'https://dotnetcli.azureedge.net/dotnet/Runtime/10.0.9/dotnet-runtime-10.0.9-win-x86.exe';
  Net6X64Url = 'https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x64.exe';
  Net6X86Url = 'https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x86.exe';

var
  NoStartMenuCheck: TNewCheckBox;
  InstallNetCheck: TNewCheckBox;

function NoStartMenuSelected: Boolean;
begin
  Result := NoStartMenuCheck.Checked;
end;

function InstallNetSelected: Boolean;
begin
  Result := InstallNetCheck.Checked;
end;

procedure InitializeWizard;
begin
  NoStartMenuCheck := TNewCheckBox.Create(WizardForm);
  NoStartMenuCheck.Parent := WizardForm.SelectProgramGroupPage;
  NoStartMenuCheck.Caption := '不创建开始菜单快捷方式';
  NoStartMenuCheck.Left := 48;
  NoStartMenuCheck.Top := WizardForm.SelectProgramGroupPage.ClientHeight - 39;
  NoStartMenuCheck.Width := 240;

  InstallNetCheck := TNewCheckBox.Create(WizardForm);
  InstallNetCheck.Parent := WizardForm.SelectComponentsPage;
  InstallNetCheck.Caption := '安装对应版本的 .NET 运行时';
  InstallNetCheck.Left := 48;
  InstallNetCheck.Top := WizardForm.SelectComponentsPage.ClientHeight - 40;
  InstallNetCheck.Width := 280;
  InstallNetCheck.Checked := True;
end;

function GetRuntimeUrl: String;
begin
  if WizardIsComponentSelected('net10x64') then
    Result := Net10X64Url
  else
    if WizardIsComponentSelected('net10x86') then
      Result := Net10X86Url
    else
      if WizardIsComponentSelected('net6x64') then
        Result := Net6X64Url
      else
        Result := Net6X86Url;
end;

function RuntimeVersionPrefix: String;
begin
  if WizardIsComponentSelected('net10x64') or WizardIsComponentSelected('net10x86') then
    Result := '10.0'
  else
    Result := '6.0';
end;

function IsDotNetRuntimeInstalled: Boolean;
var
  Versions: array of String;
  i: Integer;
  SubKey: String;
begin
  Versions := ['10.0.9', '10.0.10', '10.0.11', '6.0.36', '6.0.35', '6.0.34', '6.0.33'];
  Result := False;
  for i := 0 to GetArrayLength(Versions) - 1 do
  begin
    if Pos(RuntimeVersionPrefix, Versions[i]) = 1 then
    begin
      SubKey := 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.NETCore.App\' + Versions[i];
      if RegKeyExists(HKLM, SubKey) then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

procedure DownloadAndInstallRuntime;
var
  Url: String;
  Exe: String;
  Code: Integer;
begin
  if not InstallNetSelected then
    Exit;
  if IsDotNetRuntimeInstalled then
  begin
    MsgBox('检测到系统已安装对应版本的 .NET 运行时，跳过下载安装。', mbInformation, MB_OK);
    Exit;
  end;
  Url := GetRuntimeUrl;
  Exe := ExpandConstant('{tmp}\dotnet-runtime.exe');
  Code := 0;
  Exec('powershell.exe', '-NoProfile -Command "(New-Object System.Net.WebClient).DownloadFile(''' + Url + ''', ''' + Exe + ''')"', '', SW_HIDE, ewWaitUntilIdle, Code);
  if Code <> 0 then
  begin
    MsgBox('错误：.NET 运行时下载失败，请检查网络后重试，或手动安装对应版本的 .NET 运行时。', mbError, MB_OK);
    Exit;
  end;
  if not FileExists(Exe) then
  begin
    MsgBox('错误：.NET 运行时下载文件缺失，请检查网络后重试。', mbError, MB_OK);
    Exit;
  end;

  ShellExec('', Exe, '/install /quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, Code);
  if Code <> 0 then
    MsgBox('警告：.NET 运行时安装未完全成功（退出码 ' + IntToStr(Code) + '），请手动检查。', mbError, MB_OK);
end;

function PathRootKey: Integer;
begin
  if IsAdminLoggedOn then
    Result := HKLM
  else
    Result := HKCU;
end;

function PathSubKey: String;
begin
  if IsAdminLoggedOn then
    Result := 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment'
  else
    Result := EnvKey;
end;

procedure AddPathEntry(const Dir: String);
var
  Root: Integer;
  Paths: String;
begin
  Root := PathRootKey;
  if not RegQueryStringValue(Root, PathSubKey, EnvName, Paths) then
    Paths := '';
  if Trim(Paths) = '' then
    Paths := Dir
  else if Pos(';' + Dir + ';', ';' + Paths + ';') = 0 then
    Paths := Paths + ';' + Dir;
  RegWriteExpandStringValue(Root, PathSubKey, EnvName, Paths);
end;

procedure RemovePathEntry(const Dir: String);
var
  Root: Integer;
  Paths: String;
begin
  Root := PathRootKey;
  if RegQueryStringValue(Root, PathSubKey, EnvName, Paths) then
  begin
    Paths := ';' + Paths + ';';
    StringChangeEx(Paths, ';' + Dir + ';', ';', True);
    while Pos(';;', Paths) > 0 do
      StringChangeEx(Paths, ';;', ';', True);
    Paths := Trim(Paths);
    if Copy(Paths, 1, 1) = ';' then
      Delete(Paths, 1, 1);
    if Length(Paths) > 0 then
      if Paths[Length(Paths)] = ';' then
        SetLength(Paths, Length(Paths) - 1);
    RegWriteExpandStringValue(Root, PathSubKey, EnvName, Paths);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if WizardIsTaskSelected('add_to_path') then
      AddPathEntry(ExpandConstant('{app}'));
    if InstallNetSelected then
      DownloadAndInstallRuntime;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
    RemovePathEntry(ExpandConstant('{app}'));
end;
