# escape=`
FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022

SHELL ["powershell", "-NoLogo", "-NoProfile", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ENV PLAYWRIGHT_BROWSERS_PATH=C:\\ms-playwright `
    NUGET_PACKAGES=C:\\.nuget\\packages

RUN $index = Invoke-RestMethod 'https://nodejs.org/dist/index.json'; `
    $nodeVersion = ($index | Where-Object { $_.version -like 'v24.*' } | Select-Object -First 1).version; `
    if (-not $nodeVersion) { throw 'Failed to resolve latest Node v24 version.' }; `
    $msiName = "node-$($nodeVersion)-x64.msi"; `
    $msiUrl = "https://nodejs.org/dist/$($nodeVersion)/$msiName"; `
    Invoke-WebRequest -Uri $msiUrl -OutFile C:\node.msi; `
    Start-Process msiexec.exe -ArgumentList '/i C:\node.msi /qn /norestart' -Wait; `
    Remove-Item C:\node.msi -Force

RUN $machinePath = [Environment]::GetEnvironmentVariable('Path', 'Machine'); `
    if ($machinePath -notlike '*C:\Program Files\nodejs*') { `
      [Environment]::SetEnvironmentVariable('Path', $machinePath + ';C:\Program Files\nodejs', 'Machine') `
    }; `
    $env:Path = [Environment]::GetEnvironmentVariable('Path', 'Machine'); `
    node --version; `
    npm --version

RUN npx --yes playwright install chromium

WORKDIR C:\\work
