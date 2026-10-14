; Inno Setup script. Build with installer/build-installer.ps1 (publishes first).
#define AppName "Looking For Group"
#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

[Setup]
AppId={{5D2B7E14-93C0-4F6A-8E21-7A4C1B9D3F58}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Scott Chipman
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=..\dist
OutputBaseFilename=LookingForGroup-Setup-{#AppVersion}
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
LicenseFile=..\LICENSE
WizardStyle=modern
UninstallDisplayIcon={app}\LookingForGroup.exe

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; Flags: unchecked

[Files]
; self-contained publish: the .NET runtime is inside, nothing else to install
Source: "..\dist\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\LookingForGroup.exe"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\LookingForGroup.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\LookingForGroup.exe"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
