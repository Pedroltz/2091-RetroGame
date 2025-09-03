# 🎮 Historia2092 - Corporate Cyberpunk Adventure

**Uma aventura cyberpunk corporativa ambientada no ano 2092**

Entre no mundo cyberpunk de Neo-Tokyo 2092, onde mega-corporações controlam cada aspecto da sociedade. Como um novo agente corporativo, suas habilidades e decisões moldam completamente sua carreira e a narrativa em tempo real.

---

## ✨ **Características Principais**

### 🎯 **Sistema de Contratação Corporativa**
- **Interface de contrato elegante** com tema corporativo cyberpunk
- **5 atributos profissionais**: Resistência Física, Estabilidade Mental, Força Bruta, Aptidão Técnica e Comunicação
- **Sistema de avaliação**: 50 pontos para distribuir (base: 50, máximo: 100 cada)
- **Classificação dinâmica**: INADEQUADO → BÁSICO → COMPETENTE → AVANÇADO → EXCEPCIONAL
- **Sistema de confirmação**: Revise e ajuste seu perfil antes de assinar o contrato

### 🎨 **Interface Corporativa Avançada**
- **Design cyberpunk profissional** com bordas ASCII e elementos visuais sofisticados
- **Navegação intuitiva**: ↑↓ para navegar, ←→ para ajustar, Enter para confirmar
- **Feedback em tempo real**: Barras de progresso coloridas `[▓▓▓▓▓▓▓░░░░░░░░]` 
- **Sistema de validação visual**: Indicadores de status e mensagens contextuais
- **Tema consistente**: Interface unificada seguindo padrões corporativos

### 🤖 **Sistema de Variáveis Inteligente**
- **14+ variáveis disponíveis**: `{name}`, `{health}`, `{intelligence}`, `{conversation}`, `{date}`, etc.
- **Textos personalizados**: Cada história usa o nome e atributos do jogador
- **Dados temporais**: Data de criação, tempo jogado, última sessão

### ⚔️ **Sistema de Combate JSON**
- **Inimigos em JSON**: Sistema modular de inimigos configuráveis
- **3 Tipos de combate**: Lutar (Força), Hackear (Inteligência), Observar (Psicologia)
- **Fluxo narrativo**: Texto → Combate → Continuação da história
- **Mensagens dinâmicas**: Cada inimigo tem diálogos únicos por JSON

### 🎵 **Sistema de Áudio Imersivo**
- **Trilha sonora cyberpunk** com música de fundo
- **Suporte multiplataforma**: Windows, Linux, macOS
- **FFmpeg integrado**: Detecção e instalação automática
- **Controle inteligente**: Inicia/para música automaticamente

### 💾 **Save System Avançado**
- **Save automático** após cada mudança
- **Dados persistentes**: Personagem, atributos, progresso
- **Formato JSON** legível e editável

### 🔧 **Sistema de Instalação Inteligente**
- **Detecção automática de SO**: Windows, Debian, Ubuntu, Fedora, Arch Linux
- **Instalação automática do FFmpeg**: Winget, apt, dnf, pacman
- **Verificação de dependências**: Checa FFmpeg antes de iniciar
- **Compatibilidade total**: .NET 8.0 e 9.0

---

## 🏗️ **Arquitetura do Projeto (SOLID)**

```
2091/
├── Core/                           # 🔒 Núcleo do sistema
│   ├── Interfaces/                 # Contratos (Dependency Inversion)
│   │   ├── IChapterService.cs      # Gerenciamento de capítulos
│   │   ├── ICombatService.cs       # Sistema de combate
│   │   ├── IEnemyService.cs        # Gerenciamento de inimigos
│   │   ├── IGameConfigService.cs   # Configurações do jogo  
│   │   ├── IPlayerSaveService.cs   # Sistema de save
│   │   └── IUIService.cs           # Interface do usuário
│   └── Models/                     # Modelos de dados
│       ├── Attributes.cs           # Sistema de atributos
│       ├── Chapter.cs              # Estrutura dos capítulos
│       ├── CombatResult.cs         # Resultados de combate
│       ├── Enemy.cs                # Estrutura dos inimigos
│       ├── GameConfig.cs           # Configurações gerais
│       ├── PlayerSave.cs           # Dados do save
│       └── Protagonist.cs          # Dados do protagonista
├── Services/                       # 🔧 Lógica de negócio
│   ├── ChapterService.cs           # Carregamento de capítulos
│   ├── CombatService.cs            # Sistema de combate
│   ├── EnemyService.cs             # Carregamento de inimigos JSON
│   ├── GameConfigService.cs        # Gerenciamento de config
│   ├── GameController.cs           # Controlador principal
│   ├── MusicService.cs             # Sistema de música multiplataforma
│   └── PlayerSaveService.cs        # Gerenciamento de saves
├── Controllers/                    # 🎯 Controladores especializados
│   └── CombatController.cs         # Fluxo de combate narrativo
├── UI/                             # 🎨 Interface do usuário
│   ├── Components/
│   │   └── UIService.cs            # Menus, texto, animações
│   └── Menus/
│       ├── CharacterCreationMenu.cs # Criação de personagem
│       └── SettingsMenu.cs          # Configurações
├── Utils/                          # 🛠️ Utilitários
│   ├── ServiceContainer.cs         # Dependency Injection
│   └── FFmpegInstaller.cs          # Sistema de instalação automática
├── Enemies/                        # ⚔️ Inimigos JSON
│   ├── security_drone.json         # Drone de segurança
│   ├── corpo_guard.json            # Guarda corporativo
│   └── cyber_thug.json             # Bandido cibernético
├── Chapters/Capitulo1/             # 📚 História (JSON)
│   ├── init_inicio.json            # ⭐ Capítulo inicial (único)
│   ├── combate_drone.json          # Combate com drone
│   ├── confronto_tuneis.json       # Combate nos túneis
│   ├── victory_combat.json         # Vitória no combate
│   ├── defeat_combat.json          # Derrota no combate
│   ├── hack_victory.json           # Vitória por hacking
│   ├── flee_success.json           # Fuga bem-sucedida
│   └── [outros capítulos...]       # Continuação da história
├── sounds/                         # 🎵 Sistema de áudio
│   └── Menus/                      # Música dos menus
│       └── [música de fundo].mp3   # Música cyberpunk automática
├── Config/config.json              # ⚙️ Configurações do jogo
├── Saves/player.json               # 💾 Save do jogador
└── Program.cs                      # 🚀 Ponto de entrada (20 linhas!)
```

**Redução dramática:** De ~1000 linhas no Program.cs para apenas **20 linhas**!

---

## 📋 **Requisitos do Sistema**

### **🖥️ Sistema Operacional**
- **Windows**: Windows 10/11 (x64/ARM64)
- **Linux**: Ubuntu 18.04+, Debian 9+, CentOS 7+, RHEL 7+, Fedora 33+, SUSE 12+
- **macOS**: macOS 10.15+ (Catalina ou superior)

### **⚙️ Runtime Necessário**
- **[.NET 9.0 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)** ou superior

#### **.NET - Instalação por Gerenciadores:**
```bash
# Verificar se já está instalado
dotnet --version

# Windows (Chocolatey)
choco install dotnet-9.0-runtime

# Ubuntu/Debian
sudo apt update && sudo apt install -y dotnet-runtime-9.0

# Fedora
sudo dnf install dotnet-runtime-9.0

# macOS (Homebrew)
brew install --cask dotnet
```

#### **.NET - Download Direto (oficial Microsoft):**

##### **🪟 Windows:**
```powershell
# 1. Baixar instalador
Invoke-WebRequest -Uri "https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-win-x64.exe" -OutFile "dotnet-runtime.exe"

# 2. Instalar
.\dotnet-runtime.exe

# 3. Verificar
dotnet --version
```

##### **🐧 Linux (script universal):**
```bash
# 1. Baixar script de instalação
wget https://dot.net/v1/dotnet-install.sh

# 2. Dar permissão e executar
chmod +x dotnet-install.sh
./dotnet-install.sh --runtime dotnet --version 9.0.0

# 3. Adicionar ao PATH
export PATH="$HOME/.dotnet:$PATH"
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc

# 4. Verificar
dotnet --version
```

##### **🍎 macOS:**
```bash
# 1. Baixar script de instalação
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --runtime dotnet --version 9.0.0

# 2. Adicionar ao PATH
export PATH="$HOME/.dotnet:$PATH"
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.zshrc

# 3. Verificar
dotnet --version
```

### **🎵 Sistema de Áudio (Opcional - para música de fundo)**
O jogo inclui música de fundo automática que toca durante o menu principal.

> **ℹ️ Importante**: O jogo **funciona perfeitamente sem FFmpeg**. Se FFmpeg não estiver disponível, a música simplesmente não toca, mas todas as outras funcionalidades continuam normais.

Para a melhor experiência imersiva, instale:

#### **FFmpeg/FFplay - Instalação por Gerenciadores:**
```bash
# Windows (Chocolatey)
choco install ffmpeg

# Windows (Scoop)  
scoop install ffmpeg

# Ubuntu/Debian
sudo apt update && sudo apt install ffmpeg

# Fedora/RHEL/CentOS
sudo dnf install ffmpeg

# Arch Linux
sudo pacman -S ffmpeg

# macOS (Homebrew)
brew install ffmpeg

# Snap (Universal Linux)
sudo snap install ffmpeg

# Flatpak (Universal Linux)
flatpak install flathub org.ffmpeg.ffplay
```

#### **FFmpeg/FFplay - Download Direto (sem gerenciadores):**

##### **🪟 Windows:**
```bash
# 1. Baixar FFmpeg
# Acesse: https://www.gyan.dev/ffmpeg/builds/
# Ou use PowerShell:
Invoke-WebRequest -Uri "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip" -OutFile "ffmpeg.zip"

# 2. Extrair
Expand-Archive -Path "ffmpeg.zip" -DestinationPath "C:\ffmpeg"

# 3. Adicionar ao PATH (PowerShell como Admin)
$env:PATH += ";C:\ffmpeg\bin"
[Environment]::SetEnvironmentVariable("PATH", $env:PATH, [EnvironmentVariableTarget]::Machine)

# 4. Verificar instalação
ffplay -version
```

##### **🐧 Linux (se não tem gerenciador de pacotes):**
```bash
# 1. Baixar FFmpeg binário
wget https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz

# 2. Extrair
tar -xf ffmpeg-release-amd64-static.tar.xz

# 3. Mover para diretório do sistema
sudo cp ffmpeg-*-amd64-static/ffmpeg /usr/local/bin/
sudo cp ffmpeg-*-amd64-static/ffplay /usr/local/bin/
sudo chmod +x /usr/local/bin/ffmpeg /usr/local/bin/ffplay

# 4. Verificar instalação
ffplay -version
```

##### **🍎 macOS (sem Homebrew):**
```bash
# 1. Baixar FFmpeg
curl -O https://evermeet.cx/ffmpeg/ffmpeg-5.1.2.zip
curl -O https://evermeet.cx/ffmpeg/ffplay-5.1.2.zip

# 2. Extrair e instalar
unzip ffmpeg-5.1.2.zip && unzip ffplay-5.1.2.zip
sudo mv ffmpeg ffplay /usr/local/bin/

# 3. Verificar instalação
ffplay -version
```

#### **Sistemas de Áudio Linux:**
- **PulseAudio** (padrão na maioria das distros)
- **ALSA** (fallback automático)
- **JACK** (compatível via ffplay)
- **Pipewire** (compatível com modo PulseAudio)

### **💻 Hardware Mínimo**
- **CPU**: Qualquer processador moderno (x64/ARM64)
- **RAM**: 512 MB disponível
- **Armazenamento**: 50 MB de espaço livre
- **Terminal**: Console/Terminal com suporte a UTF-8

### **🎮 Terminais Compatíveis**

#### **Windows:**
- ✅ Command Prompt (CMD)
- ✅ PowerShell 5.x
- ✅ PowerShell 7.x  
- ✅ Windows Terminal
- ✅ ConEmu
- ✅ Git Bash

#### **Linux:**
- ✅ Bash
- ✅ Zsh
- ✅ Fish
- ✅ GNOME Terminal
- ✅ Konsole (KDE)
- ✅ Xterm
- ✅ Terminator

#### **macOS:**
- ✅ Terminal
- ✅ iTerm2
- ✅ Zsh (padrão)

---

## 🚀 **Como Executar**

### **Pré-requisitos**
- **.NET 8.0** ou superior
- **Windows/Linux/macOS** (multiplataforma)
- **FFmpeg** (instalação automática incluída)

### **🔍 Verificação de Dependências**
```bash
# Verificar .NET
dotnet --version

# Verificar FFplay (opcional)
ffplay -version
```

### **🛠️ Solução de Problemas Comuns**

#### **❌ "dotnet: command not found"**
```bash
# Windows: Reiniciar terminal após instalar .NET
# Linux/macOS: Adicionar ao PATH
export PATH="$HOME/.dotnet:$PATH"
```

#### **❌ "ffplay: command not found"**
```bash
# Normal - o jogo funciona sem FFplay
# Para instalar, use os comandos acima da seção de Áudio
```

#### **🔇 Música não toca no Linux**
```bash
# Verificar sistema de áudio
pulseaudio --check -v

# Testar áudio manualmente
aplay /usr/share/sounds/alsa/Noise.wav

# Verificar permissões de áudio
sudo usermod -a -G audio $USER
# Fazer logout e login novamente
```

#### **⚠️ Problemas de Terminal no Windows**
```bash
# Use Windows Terminal ou PowerShell 7 para melhor suporte
# Evite PowerShell ISE (não suportado)
```

### **🔗 Links Diretos para Download**

#### **.NET 9.0 Runtime:**
- **Windows x64**: https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-win-x64.exe
- **Windows ARM64**: https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-win-arm64.exe  
- **Linux x64**: https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-linux-x64.tar.gz
- **macOS x64**: https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-osx-x64.tar.gz
- **macOS ARM64**: https://download.microsoft.com/download/6/0/f/60fc12b9-8a3e-4b5d-8b2b-1e8c7b0d5a51/dotnet-runtime-9.0.0-osx-arm64.tar.gz

#### **FFmpeg (Opcional):**
- **Windows**: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip
- **Linux**: https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz
- **macOS**: https://evermeet.cx/ffmpeg/

### **⚡ Instalação Rápida por Sistema**

#### **🪟 Windows (com Chocolatey):**
```powershell
choco install dotnet-9.0-runtime ffmpeg && dotnet run
```

#### **🐧 Ubuntu/Debian:**
```bash
sudo apt update && sudo apt install -y dotnet-runtime-9.0 ffmpeg && dotnet run
```

#### **🐧 Fedora:**
```bash
sudo dnf install dotnet-runtime-9.0 ffmpeg && dotnet run
```

#### **🍎 macOS (com Homebrew):**
```bash
brew install --cask dotnet && brew install ffmpeg && dotnet run
```

### **📦 Instalação e Execução Manual**
```bash
# 1. Clone ou baixe o projeto
cd 2091-Game

# 2. Restaurar dependências (apenas primeira vez)
dotnet restore

# 3. Compilar e executar
dotnet run

# O FFmpeg será instalado automaticamente se não estiver presente
# Suporte para: Windows (winget), Debian/Ubuntu (apt), Fedora (dnf), Arch (pacman)
```

---

## 🎮 **Como Jogar**

### **1. 🎨 Tela Inicial**
- ASCII art animado da cidade cyberpunk
- Animação linha por linha (100ms cada)
- Pressione qualquer tecla para continuar

### **2. 🎯 Menu Principal**
- **↑↓** para navegar entre opções
- **Enter** para selecionar
- **Escape** para voltar/sair
- Opções: Play / Settings / Exit

### **3. 👤 Sistema de Contratação Corporativa**

#### **Configuração de Identidade:**
- **Interface de arquivo pessoal** com tema corporativo
- **Sistema de identificação neural** com validação em tempo real
- **20 caracteres máximo** para nome do agente
- **Validação inteligente** com feedback visual instantâneo

#### **Avaliação Psicométrica:**
- **50 pontos corporativos** para distribuir entre competências
- **Base mínima de 50 pontos** por atributo (padrão corporativo)
- **Navegação profissional**: ↑↓ navegar, ←→ ajustar pontos
- **Sistema de classificação**: INADEQUADO a EXCEPCIONAL
- **Barras de status corporativas**: `[▓▓▓▓▓▓▓░░░░░░░░]`

#### **Confirmação de Contrato:**
- **Revisão final** do perfil completo com ID de contrato
- **Opções de ação**: [ENTER] Assinar contrato / [ESC] Reajustar perfil
- **Sistema inteligente** permite voltar e modificar atributos

### **4. 📖 Gameplay**
- **Animação de texto** caracter por caracter
- **Enter corta toda animação** do bloco atual
- **Escolhas numeradas** (1, 2, 3...)
- **0** volta ao menu principal
- **Save automático** a cada ação

---

## 📝 **Sistema de Variáveis**

### **Variáveis Disponíveis nos JSONs:**

| Categoria | Variável | Descrição | Exemplo |
|-----------|----------|-----------|---------|
| **Personagem** | `{name}`, `{protagonist}`, `{player}` | Nome do jogador | "João" |
| **Atributos** | `{health}`, `{psychology}`, `{strength}`, `{intelligence}`, `{conversation}` | Valores 0-100 | 85 |
| **Progresso** | `{playtime}`, `{created}`, `{lastplayed}` | Dados da sessão | "15/01/2025" |
| **Temporal** | `{date}`, `{time}`, `{year}` | Data/hora atual | "14:30" |

### **Exemplo de Uso:**
```json
{
  "Text": [
    "Olá, {name}! Sua saúde está em {health}/100.",
    "Com {intelligence} de inteligência, você pode hackear sistemas avançados.",
    "Sua habilidade de conversação é {conversation}/100.",
    "Hoje é {date} e você joga há {playtime} minutos."
  ]
}
```

---

## 📚 **Criando Novos Capítulos**

### **Estrutura do Arquivo JSON:**
```json
{
  "Id": "nome_unico_capitulo",
  "Title": "Título Exibido no Jogo", 
  "Text": [
    "Linha 1 da narrativa",
    "Use {name} para personalizar",
    "Atributos: {health} HP, {intelligence} IQ, {conversation} Social"
  ],
  "Options": [
    {
      "Text": "Escolha 1 - Descrição",
      "NextChapter": "proximo_capitulo",
      "Conditions": null
    }
  ],
  "GameEnd": false,
  "NextChapter": null
}
```

### **⚠️ Regras Importantes:**
- ✅ **APENAS 1 arquivo** deve começar com `init_` 
- ✅ **`GameEnd: true`** termina a história
- ✅ **`NextChapter`** para sequência automática (sem escolhas)
- ✅ **`Options`** para ramificações com escolhas
- ✅ Use **variáveis** para personalização dinâmica

---

## 🎯 **Exemplo de Fluxo**

```
🎬 Tela Inicial → 🎮 Menu → 👤 Personagem → 📖 História
     ↓              ↓          ↓             ↓
ASCII Art    [Play/Settings]  Nome +     init_inicio.json
Animado      Navegação Setas  Atributos  Variáveis Ativas
```

---

## 🔧 **Configurações Avançadas**

### **`Config/config.json`:**
- **Cores**: 16 opções (Red, Blue, Cyan, etc.)
- **Velocidade**: 0-500ms por caracter (0 = instantâneo)
- **Configurações visuais**: ClearScreen, etc.

### **`Saves/player.json`:**
- **Personagem completo**: Nome, atributos, datas
- **Progresso**: Tempo jogado, última sessão
- **Extensível**: CustomData para futuras funcionalidades

---

## 🛠️ **Tecnologias**

- **C# 8.0+** com princípios **SOLID**
- **Newtonsoft.Json** para serialização
- **FFmpeg** para processamento de áudio
- **System.Diagnostics.Process** para execução de comandos
- **RuntimeInformation** para detecção de SO
- **Dependency Injection** manual
- **Arquitetura em camadas** (Core/Services/UI)
- **Console Application** multi-plataforma

---

## 🚧 **Roadmap Futuro**

### **✅ Funcionalidades Implementadas:**
- [x] **Sistema de Combate** baseado em atributos ✅
- [x] **Inimigos JSON configuráveis** ✅  
- [x] **Sistema de Música** multiplataforma ✅
- [x] **Sistema de Áudio** com trilha cyberpunk ✅
- [x] **Instalação automática do FFmpeg** ✅
- [x] **Detecção de sistema operacional** ✅
- [x] **Suporte multiplataforma completo** ✅

### **🎯 Funcionalidades Planejadas:**
- [ ] **Múltiplas Corporações** (diferentes contratos e benefícios)
- [ ] **Sistema de Promoções** (progressão na hierarquia corporativa)
- [ ] **Departamentos Especializados** (Security, R&D, Marketing, etc.)
- [ ] **Avaliações Periódicas** (revisões de performance)
- [ ] **Benefícios Corporativos** (equipamentos, acesso, privilégios)
- [ ] **Múltiplos Contratos** (histórias diferentes por corporação)
- [ ] **Sistema de Reputação** corporativa
- [ ] **Rivalidade Intercorporativa** (missões competitivas)

### **🎨 Melhorias de Interface:**
- [ ] **Templates de Contrato** (diferentes corporações com layouts únicos)
- [ ] **Animações ASCII** para assinatura de contratos
- [ ] **Sistema de Notificações** corporativas em tempo real
- [ ] **Dashboard Executivo** (interface pós-contratação)
- [ ] **Múltiplos Idiomas** (PT-BR, EN-US, ES, JA)

### **🔧 Melhorias Técnicas:**
- [ ] **Docker containerização** para deployment
- [ ] **CI/CD pipeline** com GitHub Actions
- [ ] **Testes unitários** automatizados
- [ ] **Benchmark de performance**
- [ ] **Localização** (PT, EN, ES)
- [ ] **Sistema de logs** avançado

---

## 📖 **Documentação para Desenvolvedores**

Para modificações e extensões do sistema, consulte:
- 📄 **`DEVELOPMENT_GUIDE.md`** - Guia completo de padrões e modificações
- 📋 **Código comentado** - CharacterCreationMenu.cs com padrões estabelecidos
- 🎨 **Sistema de cores** configurável e extensível
- 🏗️ **Arquitetura SOLID** preparada para expansões

---

## 🚀 **Iniciar Operações Corporativas**

```bash
dotnet run
```

**Bem-vindo ao futuro corporativo de Neo-Tokyo 2092. Sua carreira começa agora.** 🏢⚡

---

## 🏆 **Status do Projeto**

**Historia2092** é um projeto **completo e funcional** que oferece:
- ✅ **Sistema de criação de personagem** totalmente implementado
- ✅ **Interface profissional** com tema corporativo cyberpunk
- ✅ **Arquitetura extensível** preparada para futuras expansões
- ✅ **Código limpo** seguindo princípios SOLID
- ✅ **Documentação completa** para desenvolvedores