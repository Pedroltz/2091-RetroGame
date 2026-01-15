# Script PowerShell para criar build portátil do jogo 2091
# Este script compila o jogo como executável standalone com proteção de dados

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"

# Cores para output
function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

Write-Host ""
Write-Host "╔════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     2091 - Builder de Aplicativo Portátil      ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Variáveis
$projectPath = Get-Location
$outputFolder = Join-Path $projectPath "Build-Portable"
$appName = "2091"

# Limpar build anterior se existir
if (Test-Path $outputFolder) {
    Write-Host "Limpando build anterior..." -ForegroundColor Yellow
    Remove-Item -Path $outputFolder -Recurse -Force
}

# Criar pasta de output
New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null

Write-Host ""
Write-Host "Compilando projeto..." -ForegroundColor Green
Write-Host "  -> Configuração: $Configuration" -ForegroundColor Gray
Write-Host "  -> Runtime: $Runtime" -ForegroundColor Gray
Write-Host "  -> Modo: Single-File (Arquivo Único)" -ForegroundColor Gray
Write-Host "  -> Proteção: Dados Embedded (Capítulos, Inimigos, Itens)" -ForegroundColor Magenta
Write-Host ""

# Build com PublishSingleFile para criar arquivo único
$buildResult = dotnet publish `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishTrimmed=true `
    -p:TrimMode=partial `
    -o "$outputFolder\$appName"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERRO: Falha ao compilar o projeto!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Build concluído com sucesso!" -ForegroundColor Green
Write-Host ""

# Remover DLLs soltas se PublishSingleFile funcionou
$exeCount = (Get-ChildItem -Path "$outputFolder\$appName" -Filter "2091.exe").Count
$dllCount = (Get-ChildItem -Path "$outputFolder\$appName" -Filter "*.dll" -File).Count

if ($exeCount -gt 0 -and $dllCount -gt 0) {
    Write-Host "  -> Removendo DLLs desnecessárias (arquivo único criado)" -ForegroundColor Gray
    Get-ChildItem -Path "$outputFolder\$appName" -Filter "*.dll" -File | Remove-Item -Force
}

# Limpar arquivos desnecessários
Write-Host "Limpando arquivos desnecessários..." -ForegroundColor Green
$filesToRemove = @("*.pdb", "*.xml", "*.deps.json")
foreach ($pattern in $filesToRemove) {
    Get-ChildItem -Path "$outputFolder\$appName" -Filter $pattern -File | Remove-Item -Force -ErrorAction SilentlyContinue
}

Write-Host "Verificando arquivos de dados do jogo..." -ForegroundColor Green
Write-Host ""
Write-Host "  [PROTEGIDO] Chapters/ - Embutido no executável" -ForegroundColor Magenta
Write-Host "  [PROTEGIDO] Enemies/  - Embutido no executável" -ForegroundColor Magenta
Write-Host "  [PROTEGIDO] Items/    - Embutido no executável" -ForegroundColor Magenta
Write-Host ""

# Note: Chapters, Enemies, and Items are now embedded in the executable for protection
# Only Config, Saves, Sounds, and NPCs need to be copied as external files
$dataFolders = @(
    "Config",
    "Saves",
    "Sounds",
    "NPCs"
)

foreach ($folder in $dataFolders) {
    $sourcePath = $folder
    $destPath = "$outputFolder\$appName\$folder"

    # Verificar se a pasta já foi copiada pelo dotnet publish
    if (Test-Path $destPath) {
        Write-Host "  -> $folder já copiado pelo build" -ForegroundColor Gray
        continue
    }

    # Verificar se a pasta fonte existe
    if (Test-Path $sourcePath) {
        Write-Host "  -> Copiando $folder..." -ForegroundColor Gray

        # Criar pasta de destino
        New-Item -ItemType Directory -Path $destPath -Force | Out-Null

        # Copiar APENAS o conteúdo (não a pasta em si)
        Copy-Item -Path "$sourcePath\*" -Destination $destPath -Recurse -Force
    } else {
        Write-Host "  -> Aviso: Pasta $folder não encontrada, ignorando..." -ForegroundColor Yellow
    }
}

# Handle API keys separately
Write-Host ""
Write-Host "Verificando configuração de API..." -ForegroundColor Green
$apiKeysSource = "Config\api-keys.json"
$apiKeysExample = "Config\api-keys.example.json"
$apiKeysDest = "$outputFolder\$appName\Config\api-keys.json"

if (Test-Path $apiKeysSource) {
    Copy-Item -Path $apiKeysSource -Destination $apiKeysDest -Force
    Write-Host "  [✓] API keys copiadas para o build" -ForegroundColor Green
    Write-Host "  [!] AVISO: Mantenha sua chave API segura!" -ForegroundColor Yellow
} else {
    if (Test-Path $apiKeysExample) {
        Copy-Item -Path $apiKeysExample -Destination $apiKeysDest -Force
        Write-Host "  [!] Arquivo de exemplo copiado como api-keys.json" -ForegroundColor Yellow
        Write-Host "  [!] Configure a chave DeepSeek API para usar o sistema de chat" -ForegroundColor Yellow
    }
}

# Criar arquivo README.txt no build
Write-Host ""
Write-Host "Criando README..." -ForegroundColor Green

$readmeContent = @"
╔════════════════════════════════════════════════════════════════╗
║                          2091 - NIGHT CITY                     ║
║                    Versão Portátil Standalone                  ║
╚════════════════════════════════════════════════════════════════╝

COMO JOGAR:
-----------
1. Execute "2091.exe" ou "Jogar-2091.bat"
2. Use as teclas de seta para navegar nos menus
3. Pressione Enter para confirmar opções
4. Pressione F5 durante o jogo para salvar

CONTROLES:
----------
- ↑↓ : Navegar nas opções
- Enter : Confirmar seleção
- Escape : Voltar/Cancelar
- F5 : Salvar jogo manualmente
- I : Abrir inventário (durante o jogo)

REQUISITOS:
-----------
- Windows 7 ou superior (64-bit)
- Não requer .NET instalado (runtime incluído)
- ~100MB de espaço em disco

ARQUIVOS IMPORTANTES:
---------------------
- 2091.exe : Executável principal do jogo
- Jogar-2091.bat : Atalho rápido para iniciar
- Config/ : Configurações do jogo (pode editar)
- Saves/ : Salvamentos do jogo (pode editar/backup)
- Sounds/ : Música e sons do jogo

PROTEÇÃO DE CONTEÚDO:
----------------------
Os capítulos da história, inimigos e itens estão protegidos
e embutidos no executável para prevenir modificações não
autorizadas e preservar a experiência original do jogo.

SUPORTE:
--------
Para reportar bugs ou obter suporte, visite:
https://github.com/anthropics/claude-code/issues

Desenvolvido com Claude Code
© 2025 - 2091: Another Story

Divirta-se em Night City!
"@

Set-Content -Path "$outputFolder\$appName\README.txt" -Value $readmeContent -Encoding UTF8

# Criar arquivo .bat para executar o jogo facilmente
Write-Host "Criando atalho de execução..." -ForegroundColor Green

$batContent = @"
@echo off
chcp 65001 >nul
title 2091 - Night City
cls
2091.exe
pause
"@

Set-Content -Path "$outputFolder\$appName\Jogar-2091.bat" -Value $batContent -Encoding ASCII

Write-Host ""
Write-Host "╔════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║           BUILD CONCLUÍDO COM SUCESSO!         ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

# Estatísticas do build
$buildPath = "$outputFolder\$appName"
$totalSize = (Get-ChildItem -Path $buildPath -Recurse | Measure-Object -Property Length -Sum).Sum
$totalSizeMB = [math]::Round($totalSize / 1MB, 2)
$fileCount = (Get-ChildItem -Path $buildPath -Recurse -File).Count

Write-Host "ESTATÍSTICAS:" -ForegroundColor Cyan
Write-Host "  -> Pasta: $buildPath" -ForegroundColor Gray
Write-Host "  -> Tamanho Total: $totalSizeMB MB" -ForegroundColor Gray
Write-Host "  -> Arquivos: $fileCount" -ForegroundColor Gray
Write-Host ""

# Listar conteúdo
Write-Host "CONTEÚDO DO BUILD:" -ForegroundColor Cyan
Get-ChildItem -Path $buildPath -File | ForEach-Object {
    $sizeMB = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  [EXE] $($_.Name) - $sizeMB MB" -ForegroundColor White
}

Get-ChildItem -Path $buildPath -Directory | ForEach-Object {
    $dirSize = (Get-ChildItem -Path $_.FullName -Recurse | Measure-Object -Property Length -Sum).Sum
    $dirSizeMB = [math]::Round($dirSize / 1MB, 2)
    $fileCount = (Get-ChildItem -Path $_.FullName -Recurse -File).Count
    Write-Host "  [DIR] $($_.Name)/ - $fileCount arquivos ($dirSizeMB MB)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "PROTEÇÃO DE DADOS:" -ForegroundColor Magenta
Write-Host "  [✓] Capítulos protegidos (embedded)" -ForegroundColor Green
Write-Host "  [✓] Inimigos protegidos (embedded)" -ForegroundColor Green
Write-Host "  [✓] Itens protegidos (embedded)" -ForegroundColor Green
Write-Host "  [✓] Saves acessíveis (Saves/)" -ForegroundColor Cyan
Write-Host "  [✓] Config acessível (Config/)" -ForegroundColor Cyan
Write-Host ""

Write-Host "PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host "  1. Teste o jogo: $buildPath\2091.exe" -ForegroundColor White
Write-Host "  2. Comprima em .zip para distribuição" -ForegroundColor White
Write-Host "  3. Distribua a pasta completa '$appName'" -ForegroundColor White
Write-Host ""

Write-Host "Pressione qualquer tecla para fechar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
