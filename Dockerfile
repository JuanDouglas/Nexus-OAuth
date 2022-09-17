FROM mcr.microsoft.com/windows/servercore/iis AS finish
LABEL version="1.0" desciption="Nexus OAuth Api"

# This script installs SQL Server Express 2019.
RUN powershell function Install-SQLServerExpress2019 { \
    Write-Host "Downloading SQL Server Express 2019..." \
    $Path = $env:TEMP \
    $Installer = "SQL2019-SSEI-Expr.exe" \
    $URL = "https://go.microsoft.com/fwlink/?linkid=866658" \
    Invoke-WebRequest $URL -OutFile $Path\$Installer \
    # Start Install SQL Express
    Write-Host "Installing SQL Server Express..." \
    Start-Process -FilePath $Path\$Installer -Args "/ACTION=INSTALL /IACCEPTSQLSERVERLICENSETERMS /QUIET" -Verb RunAs -Wait \
    Remove-Item $Path\$Installer \
} \
# Execute function for install 
Install-SQLServerExpress2019
ADD publish src
WORKDIR src
# Publish applciation in server 
EXPOSE 44360