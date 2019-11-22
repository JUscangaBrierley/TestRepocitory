@echo off
nuget install Brierley.Cicd.Client.DeployRemoteAssembly -NonInteractive -ExcludeVersion -OutputDirectory "%~dp0cicdtools" -Version "1.0.0" || exit /b
"%~dp0cicdtools\Brierley.Cicd.Client.DeployRemoteAssembly\tools\5.4.0\Brierley.Cicd.Client.DeployRemoteAssembly.exe" "%~dp0DeployRemoteAssembly.yml" "%~1" || exit /b