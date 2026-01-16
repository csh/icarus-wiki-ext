#define AppVersion "0.0.1.0"
#define AppName "ICARUS Wiki Search"
#define ExtName "IcarusWikiExtension"

[Setup]
AppId=973f6a8b-d581-444f-bef5-436e5980f6cf
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=csh
DefaultDirName={autopf}\IcarusWikiExtension
OutputDir=bin\Release\installer
OutputBaseFilename=IcarusWikiExtension-Setup-{#AppVersion}
Compression=lzma
SolidCompression=yes
MinVersion=10.0.19041
DisableProgramGroupPage=yes
LicenseFile=../LICENSE
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\Release\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\DISPLAY_NAME"; Filename: "{app}\IcarusWikiSearchExt.exe"

[Registry]
Root: HKA; Subkey: "SOFTWARE\Classes\CLSID\cea3bf6c-8e49-4946-a84f-126afc9bf41b"; ValueData: "{#ExtName}"
Root: HKA; Subkey: "SOFTWARE\Classes\CLSID\cea3bf6c-8e49-4946-a84f-126afc9bf41b\LocalServer32"; ValueData: "{app}\{#ExtName}.exe -RegisterProcessAsComServer"