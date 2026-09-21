# 🎴 Jogo UNO em WPF (.NET 8)

Um jogo de cartas **UNO** interativo desenvolvido em C# com **WPF (Windows Presentation Foundation)**. O projeto inclui suporte para múltiplos jogadores, cartas especiais, persistência de dados em XML para guardar o estado do jogo e estatísticas completas de vitórias/derrotas.

---

## 🚀 Funcionalidades

- **Regras Clássicas do UNO:**
  - Suporte a cartas numéricas (0 a 9) e cartas especiais (*Compra 2*, *Inverter*, *Salta*).
  - Cartas de ação especial (*Joker* e *Joker +4*) com escolha de cor ativa.
  - Mecânica de acumulação de penalizações e alternância do sentido de jogo.
- **Persistência de Dados (XML):**
  - **Guardar e Carregar Jogo:** Guarda o estado completo do jogo (mão dos jogadores, baralho, pilha de descarte, sentido e cor ativa) para continuar mais tarde.
  - **Estatísticas Históricas:** Registo automático de jogos/partidas jogadas, vitórias e percentagens de sucesso por jogador.
- **Interface Gráfica (WPF):**
  - Layout personalizado com design visual temático.
  - Animações e efeitos visuais usando *DropShadows* e estilização XAML personalizada.

---

## 🛠️ Tecnologias Utilizadas

- **Linguagem:** C#
- **Framework:** .NET 8.0 (Windows)
- **Interface Gráfica:** WPF (Windows Presentation Foundation)
- **Armazenamento de Dados:** XML (`XDocument` / `System.Xml.Linq`)

---

## 📂 Estrutura do Projeto

```text
├── Assets/                 # Imagens e recursos visuais (baralho, mesa)
├── App.xaml / App.xaml.cs  # Ponto de entrada da aplicação
├── Baralho.cs              # Lógica de criação, baralhação e compra de cartas
├── Carta.cs                # Modelo de dados e regras de validação das cartas
├── GestorXml.cs            # Leitura e escrita de ficheiros XML (Jogo e Estatísticas)
├── EstatisticasWindow.xaml # Janela de visualização do histórico de jogadores
└── MainWindow.xaml         # Janela principal do jogo
```

---

## ⚙️ Como Executar o Projeto

### Pré-requisitos
- **Visual Studio 2022** (com a carga de trabalho *.NET desktop development* instalada) ou **SDK do .NET 8.0**.
- Sistema Operativo **Windows**.

### Passos
1. Clona este repositório:
   ```bash
   git clone https://github.com/TEU_UTILIZADOR/TEU_REPOSITORIO.git
   ```
2. Abre o ficheiro do projeto (`.csproj` ou `.sln`) no Visual Studio.
3. Compila e executa o projeto premindo **F5** ou clicando em **Start / Iniciar**.

---

## 📝 Licença

Este projeto foi desenvolvido para fins educacionais e de demonstração. Sente-te à vontade para utilizar, modificar e contribuir!