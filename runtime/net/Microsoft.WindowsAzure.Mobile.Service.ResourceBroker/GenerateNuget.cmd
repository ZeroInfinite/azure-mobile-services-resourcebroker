@echo off
if not exist bin\Nugets md bin\Nugets

echo .
echo Nuget will be placed in the bin\Nugets folder
echo .

..\..\..\tools\Nuget.exe pack Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.csproj -Prop Configuration=Release -OutputDirectory bin\Nugets