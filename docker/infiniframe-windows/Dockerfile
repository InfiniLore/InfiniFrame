# escape=`
FROM node:22-windowsservercore-ltsc2022 AS node
FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022

SHELL ["powershell", "-NoLogo", "-NoProfile", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ENV PLAYWRIGHT_BROWSERS_PATH=C:\\ms-playwright `
    NUGET_PACKAGES=C:\\.nuget\\packages

COPY --from=node "C:\\Program Files\\nodejs" "C:\\Program Files\\nodejs"

RUN $machinePath = [Environment]::GetEnvironmentVariable('Path', 'Machine'); `
    [Environment]::SetEnvironmentVariable('Path', $machinePath + ';C:\Program Files\nodejs', 'Machine'); `
    node --version; `
    npm --version

RUN npx --yes playwright install chromium

WORKDIR C:\\work
