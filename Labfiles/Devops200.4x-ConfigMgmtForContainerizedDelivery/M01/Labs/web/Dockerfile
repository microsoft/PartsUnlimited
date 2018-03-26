FROM microsoft/aspnet

# # Download and extract BlogEngine.NET project files
RUN powershell -Command \
    $ErrorActionPreference = 'Stop'; \
	Invoke-WebRequest -Method Get -Uri https://blogengine.codeplex.com/downloads/get/1566660 -OutFile c:\BlogEngineNETSrc.zip ; \
	Expand-Archive -Path c:\BlogEngineNETSrc.zip -DestinationPath c:\inetpub\wwwroot\blogengine ; \
	Remove-Item c:\BlogEngineNETSrc.zip -Force

# Configure Web App
COPY buildapp.ps1 C:/
RUN powershell.exe C:\buildapp.ps1

#FIX DNS issues currently in Windows Containers
RUN powershell set-itemproperty -path 'HKLM:\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters' -Name ServerPriorityTimeLimit -Value 0 -Type DWord 

# Overwrite Web.config file so that web service points to db service
COPY Web.config C:/inetpub/wwwroot/blogengine
EXPOSE 80

