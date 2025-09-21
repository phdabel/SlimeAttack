# SlimeAttack 🎮

Um jogo de ação e aventura desenvolvido em Unity, inspirado no estilo Zelda, onde o jogador enfrenta slimes em um mundo dinâmico com sistemas climáticos e IA.

## 📋 Índice

- [Sobre o Jogo](#sobre-o-jogo)
- [Características Principais](#características-principais)
- [Personagens](#personagens)
- [Sistemas do Jogo](#sistemas-do-jogo)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Aspectos do Desenvolvimento](#aspectos-do-desenvolvimento)
- [Como Jogar](#como-jogar)
- [Instalação](#instalação)
- [Desenvolvimento](#desenvolvimento)

## 🎮 Sobre o Jogo

SlimeAttack é um jogo de ação em terceira pessoa onde o jogador controla um aventureiro que deve enfrentar slimes inteligentes em diferentes ambientes. O jogo combina combate dinâmico, exploração e sistemas ambientais interativos.

### 🎯 Objetivo

Sobreviva aos ataques dos slimes hostis, explore o mundo e domine diferentes tipos de combate em um ambiente que reage às suas ações.

## ✨ Características Principais

### 🎮 Controles Modernos
- **Suporte completo ao New Input System da Unity**
- **Controle via teclado (WASD/Setas)** para movimento
- **Suporte nativo ao controle Xbox** com analógico e D-pad
- **Sistema de câmera** alternando entre câmera em terceira pessoa e câmera aérea

### ⚔️ Sistema de Combate
- **Ataque normal** (Ctrl/Botão X) - Dano alto, velocidade normal
- **Ataque rápido** (Shift/Botão A) - Dano baixo, velocidade alta
- **Sistema de defesa** (Shift esquerdo/Botão B)
- **Troca de câmera** (Botão C/D-pad Cima)
- **Feedback visual** com partículas e animações

### 🧠 IA Avançada dos Slimes
- **Sistema de estados** (Idle, Patrol, Alert, Fury)
- **Campo de visão realista** com campo de visão e detecção de obstáculos
- **Memória comportamental** - slimes atacados ficam mais agressivos
- **Patrulhamento inteligente** entre waypoints

### 🌧️ Sistema Climático
- **Zonas de chuva** ativadas por triggers
- **Efeitos visuais** de partículas atmosféricas
- **Impacto ambiental** no gameplay

## 👥 Personagens

### 🏃‍♂️ Player (Protagonista)
- **HP configurável** pelo GameManager
- **Múltiplos tipos de ataque** com danos diferentes
- **Sistema de gravidade** realista
- **Animações fluidas** para movimento, ataque e defesa

### 👾 Slimes (Inimigos)
- **HP variável** (padrão: 3 pontos)
- **Comportamento adaptativo** baseado em experiências anteriores
- **Estados emocionais** representados por animações

## 🔧 Sistemas do Jogo

### 🤖 Sistema de IA
```
Estados dos Slimes:
├── IDLE - Parado, observando
├── PATROL - Patrulhando entre waypoints
├── ALERT - Detectou o player, mas ainda observando
└── FURY - Perseguindo e atacando ativamente
```

### 👁️ Sistema de Detecção
- **Field of View (FoV)** com ângulo e distância configuráveis
- **Raycast para obstáculos** - inimigos não veem através de paredes
- **Detecção contínua** otimizada (verificação a cada 0.2s)
- **Trigger zones** para detecção de proximidade

### 🎯 Sistema de Combate
- **Hit detection** via OverlapSphere
- **Layer masks** para separar alvos
- **Particle effects** para feedback visual
- **Animation events** para timing preciso

### 🌍 Sistema Ambiental
- **Gerenciador de chuva** com zonas específicas
- **Múltiplas câmeras** (terceira pessoa e aérea)
- **Waypoint system** para navegação de NPCs

## 🛠️ Tecnologias Utilizadas

### 🎮 Unity Technologies
- **Unity 6.2**
- **New Input System** para controles modernos
- **NavMesh Agent** para navegação de IA
- **Particle System** para efeitos visuais
- **Animator Controller** para animações
- **Character Controller** para movimento do player
- **Cinemachine** para controle avançado de câmeras

### 📝 Linguagens e Frameworks
- **C#** como linguagem principal
- **Unity's Job System** para otimização de performance
- **Coroutines** para comportamentos assíncronos

### 🎨 Assets e Recursos
- **Simple Nature Pack** para ambientação
- **BTM Assets** para elementos visuais
- **DogKnight** e **Nicrom** para personagens
- **Sistema de sprites** personalizado

## 🔍 Aspectos do Desenvolvimento

### 🏗️ Arquitetura do Código

#### 📂 Organização de Scripts
```
Scripts/
├── PlayerController.cs - Controle do jogador
├── SlimeAI.cs - Inteligência artificial dos inimigos
├── GameManager.cs - Gerenciamento global do jogo
├── FoVDetection.cs - Sistema de campo de visão
├── RainManager.cs - Controle do sistema climático
└── Collectible.cs - Itens colecionáveis, como gemas
```

### 🤝 Contribuições
Este é um projeto em desenvolvimento ativo. Sugestões e melhorias são bem-vindas!

### 📈 Roadmap
- [ ] Múltiplos tipos de inimigos
- [ ] Drop de HP para recuperar a vida
- [ ] Criar inimigos de tamanhos diferentes baseado no HP
- [ ] Diminuir o tamanho dos inimigos baseado no HP
- [ ] Levels procedurais

---

**Desenvolvido com ❤️ usando Unity Engine**

*Projeto criado como estudo de sistemas de IA, controles modernos e gameplay dinâmico.*
