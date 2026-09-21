using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Jogo
{
    // ─── ViewModel de uma carta na mão do jogador ──────────────────────────────

    public class CartaVM : INotifyPropertyChanged
    {
        private bool _jogavel;

        public Carta  Carta      { get; init; } = null!;
        public string ImagemPath { get; init; } = string.Empty;

        public bool Jogavel
        {
            get => _jogavel;
            set { _jogavel = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // ─── Code-behind ───────────────────────────────────────────────────────────

    public partial class JogoWindow : Window
    {
        private readonly Game _jogo;

        private readonly DispatcherTimer _timerBot = new()
        {
            Interval = TimeSpan.FromMilliseconds(2000)  // protocolo: bots demoram 2 segundos
        };

        private readonly ObservableCollection<CartaVM> _maoHumano = new();

        // ── Construtor ────────────────────────────────────────────────────────

        public JogoWindow(Game jogo)
        {
            InitializeComponent();
            _jogo = jogo;

            _jogo.OnCartaJogada          += (_, _) => Dispatcher.Invoke(AtualizarUI);
            _jogo.OnCartaComprada        += (_, _) => Dispatcher.Invoke(AtualizarUI);
            _jogo.OnTurnoMudou           += Jogo_TurnoMudou;
            _jogo.OnJogoTerminou         += Jogo_JogoTerminou;
            _jogo.OnUnoGritado           += Jogo_UnoGritado;
            _jogo.OnPenalidadeUno        += Jogo_PenalidadeUno;
            _jogo.OnBaralhoReabastecido  += (_, _) => Dispatcher.Invoke(() =>
                TxtNumBaralho.Text = $"{_jogo.Mesa.Baralho.Cartas.Count}");

            _timerBot.Tick += TimerBot_Tick;

            CartasJogador.ItemsSource = _maoHumano;

            AtualizarUI();

            // Arranca o timer se o primeiro turno é de um bot
            if (_jogo.JogoEmCurso && _jogo.JogadorAtivo != _jogo.Jogadores[0])
                _timerBot.Start();
        }

        // ═════════════════════════════════════════════════════════════════════
        //  ATUALIZAÇÃO COMPLETA DA UI
        // ═════════════════════════════════════════════════════════════════════

        private void AtualizarUI()
        {
            AtualizarMaoHumano();
            AtualizarBots();
            AtualizarMesa();
            AtualizarControlos();
        }

        // ── Mão do jogador ───────────────────────────────────────────────────

        private void AtualizarMaoHumano()
        {
            if (_jogo.Jogadores.Count == 0) return;

            var humano = _jogo.Jogadores[0];
            var topo   = _jogo.Mesa.CartaNoTopo;
            bool vezHumano = _jogo.JogadorHumanoTemVez;

            _maoHumano.Clear();
            foreach (var carta in humano.Cartas)
            {
                bool jogavel = vezHumano && topo != null &&
                               carta.PodeJogarSobre(topo, _jogo.Mesa.CorAtiva);
                _maoHumano.Add(new CartaVM
                {
                    Carta      = carta,
                    ImagemPath = ObterCaminhoImagem(carta),
                    Jogavel    = jogavel
                });
            }

            NomeJogadorHumano.Text =
                $"{humano.Nome}  •  {humano.Cartas.Count} carta{(humano.Cartas.Count == 1 ? "" : "s")}";
        }

        // ── Bots ─────────────────────────────────────────────────────────────

        private void AtualizarBots()
        {
            int nBots = _jogo.Jogadores.Count - 1;

            // Bot 1 — sempre no topo centro
            AtualizarPainelBot(
                BorderBot1, NomeBot1, ContadorBot1, CartasBot1Canvas,
                nBots >= 1 ? _jogo.Jogadores[1] : null);

            // Bot 2 — esquerda, visível com ≥2 bots
            AtualizarPainelBot(
                BorderBot2, NomeBot2, ContadorBot2, CartasBot2Canvas,
                nBots >= 2 ? _jogo.Jogadores[2] : null);

            // Bot 3 — direita, só com 3 bots
            AtualizarPainelBot(
                BorderBot3, NomeBot3, ContadorBot3, CartasBot3Canvas,
                nBots >= 3 ? _jogo.Jogadores[3] : null);
        }

        private void AtualizarPainelBot(
            Border border, TextBlock nome, TextBlock contador,
            Canvas canvas, Jogador? bot)
        {
            if (bot == null)
            {
                border.Visibility = Visibility.Collapsed;
                return;
            }

            border.Visibility = Visibility.Visible;

            bool ativo = _jogo.JogadorAtivo == bot;

            // Destaca a borda quando é a vez do bot (igual à mockup)
            border.BorderBrush = ativo
                ? new SolidColorBrush(Color.FromRgb(0xE6, 0x39, 0x46))   // vermelho
                : Brushes.Transparent;

            nome.Text     = bot.Nome + (ativo ? " ◀" : "");
            contador.Text = $"{bot.Cartas.Count} Carta{(bot.Cartas.Count == 1 ? "" : "s")}";

            DesenharCostasCartas(canvas, bot.Cartas.Count);
        }

        /// <summary>
        /// Desenha as costas das cartas do bot no Canvas com sobreposição estilo "leque".
        /// </summary>
        private static void DesenharCostasCartas(Canvas canvas, int quantidade)
        {
            canvas.Children.Clear();
            if (quantidade == 0) return;

            // Máximo de cartas visíveis no leque
            int visiveis  = Math.Min(quantidade, 7);
            double largura = 38;
            double altura  = 55;
            double passo   = Math.Min(18.0, (canvas.Width - largura) / Math.Max(visiveis - 1, 1));

            for (int i = 0; i < visiveis; i++)
            {
                var img = new Image
                {
                    Source = CarregarImagem("/Assets/card_back.png"),
                    Width  = largura,
                    Height = altura
                };
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
                Canvas.SetLeft(img, i * passo);
                Canvas.SetTop(img, (canvas.Height - altura) / 2);
                canvas.Children.Add(img);
            }
        }

        // ── Mesa ─────────────────────────────────────────────────────────────

        private void AtualizarMesa()
        {
            var topo = _jogo.Mesa.CartaNoTopo;
            if (topo != null)
                ImgCartaTopo.Source = CarregarImagem(ObterCaminhoImagem(topo));

            // Bolinha colorida + texto da cor ativa
            BorderCorAtiva.Background = CorParaPincel(_jogo.Mesa.CorAtiva);
            TxtCorAtiva.Text = CorParaNome(_jogo.Mesa.CorAtiva);

            // Seta de sentido
            TxtSentido.Text = _jogo.SentidoNormal ? "↻" : "↺";

            // Contador do baralho
            TxtNumBaralho.Text = $"{_jogo.Mesa.Baralho.Cartas.Count}";
        }

        // ── Controlos ────────────────────────────────────────────────────────

        private void AtualizarControlos()
        {
            bool vezHumano = _jogo.JogadorHumanoTemVez;

            BtnComprar.IsEnabled     = vezHumano;
            BtnComprarAcao.IsEnabled = vezHumano;

            // Botão UNO — aparece quando o humano tem 2 cartas e ainda não gritou
            BtnUno.Visibility = _jogo.HumanoDeveGritarUno
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Texto do botão de compra consoante compras pendentes
            BtnComprarAcao.Content = _jogo.CartasPendentesCompra > 0
                ? $"Comprar +{_jogo.CartasPendentesCompra}"
                : "Comprar Carta";

            // Escolha de cor
            PainelEscolhaCor.Visibility =
                (_jogo.AguardarEscolhaCor && _jogo.JogadorAtivo == _jogo.Jogadores[0])
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Mensagem de estado (barra de topo)
            if (!_jogo.JogoEmCurso)
            {
                TxtEstado.Text = "";
                return;
            }

            if (_jogo.AguardarEscolhaCor && _jogo.JogadorAtivo == _jogo.Jogadores[0])
                TxtEstado.Text = "Escolhe uma cor!";
            else if (vezHumano)
                TxtEstado.Text = _jogo.JogadorHumanoTemCartaJogavel
                    ? "A tua vez — joga uma carta!"
                    : "Sem cartas jogáveis — compra uma carta.";
            else
                TxtEstado.Text = $"Vez de {_jogo.JogadorAtivo?.Nome}...";
        }

        // ═════════════════════════════════════════════════════════════════════
        //  EVENTOS DO MOTOR DE JOGO
        // ═════════════════════════════════════════════════════════════════════

        private void Jogo_TurnoMudou(object? sender, TurnoMudouEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                AtualizarUI();
                if (_jogo.JogoEmCurso && _jogo.JogadorAtivo != _jogo.Jogadores[0])
                    _timerBot.Start();
                else
                    _timerBot.Stop();
            });
        }

        private void Jogo_JogoTerminou(object? sender, JogoTerminouEventArgs e)
        {
            _timerBot.Stop();
            Dispatcher.Invoke(() =>
            {
                AtualizarUI();

                // Constrói mensagem de pontuações
                string pontuacoes = string.Join("\n", _jogo.Jogadores.Select((j, i) =>
                    $"  {j.Nome}: {_jogo.Pontuacoes[i]} pontos"));

                bool humanoVenceu = e.Vencedor == _jogo.Jogadores[0];
                string cabecalho  = humanoVenceu
                    ? $"🎉 Parabéns! Ganhaste esta partida!\n(+{e.PontuacaoRond} pontos)"
                    : $"😔 {e.Vencedor.Nome} ganhou a partida!\n(+{e.PontuacaoRond} pontos)";

                if (e.FimDeJogo)
                {
                    // Jogo completo terminou (alguém atingiu 500 pontos)
                    string vencedorJogo = humanoVenceu ? "Tu ganhaste o jogo!" : $"{e.Vencedor.Nome} ganhou o jogo!";
                    MessageBox.Show(
                        $"{cabecalho}\n\n🏆 {vencedorJogo}\n\nPlacar final:\n{pontuacoes}",
                        "Fim do Jogo!", MessageBoxButton.OK, MessageBoxImage.Information);
                    GestorXml.ApagarJogoGuardado();
                    Close();
                }
                else
                {
                    var r = MessageBox.Show(
                        $"{cabecalho}\n\nPlacar:\n{pontuacoes}\n\nJogar nova partida?",
                        "Fim da Partida", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (r == MessageBoxResult.Yes)
                    {
                        _jogo.IniciarNovoJogo();
                        AtualizarUI();
                        if (_jogo.JogadorAtivo != _jogo.Jogadores[0])
                            _timerBot.Start();
                    }
                    else
                    {
                        _jogo.SuspenderJogo();
                        Close();
                    }
                }
            });
        }

        private void Jogo_UnoGritado(object? sender, UnoGritadoEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TxtEstado.Text = $"🃏 UNO! {e.Jogador.Nome} tem só 1 carta!";
                if (e.Jogador == _jogo.Jogadores[0])
                    BtnUno.Visibility = Visibility.Collapsed;
            });
        }

        private void Jogo_PenalidadeUno(object? sender, PenalidadeUnoEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TxtEstado.Text = $"⚠️ {e.Jogador.Nome} não gritou UNO! +{e.NumCartas} cartas de penalidade!";
                AtualizarUI();
            });
        }

        // ═════════════════════════════════════════════════════════════════════
        //  TIMER DOS BOTS
        // ═════════════════════════════════════════════════════════════════════

        private void TimerBot_Tick(object? sender, EventArgs e)
        {
            _timerBot.Stop();
            if (!_jogo.JogoEmCurso) return;
            if (_jogo.JogadorAtivo == _jogo.Jogadores[0]) return;
            _jogo.ExecutarTurnoBot();
        }

        // ═════════════════════════════════════════════════════════════════════
        //  HANDLERS DE BOTÕES
        // ═════════════════════════════════════════════════════════════════════

        private void CartaJogador_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not Carta carta) return;
            if (!_jogo.JogarCarta(carta))
                TxtEstado.Text = "Não podes jogar essa carta agora.";
        }

        private void BtnUno_Click(object sender, RoutedEventArgs e)
        {
            _jogo.GritarUno();
            TxtEstado.Text = $"🃏 UNO! {_jogo.Jogadores[0].Nome} gritou UNO!";
            BtnUno.Visibility = Visibility.Collapsed;
        }

        private void BtnComprar_Click(object sender, RoutedEventArgs e)
            => _jogo.ComprarCarta();

        private void BtnCorVermelho_Click(object sender, RoutedEventArgs e)
            => _jogo.EscolherCor(CorCarta.Vermelho);

        private void BtnCorVerde_Click(object sender, RoutedEventArgs e)
            => _jogo.EscolherCor(CorCarta.Verde);

        private void BtnCorAzul_Click(object sender, RoutedEventArgs e)
            => _jogo.EscolherCor(CorCarta.Azul);

        private void BtnCorAmarelo_Click(object sender, RoutedEventArgs e)
            => _jogo.EscolherCor(CorCarta.Amarelo);

        private void BtnPausa_Click(object sender, RoutedEventArgs e)
        {
            _timerBot.Stop();

            var resultado = MessageBox.Show(
                "Jogo em pausa.\n\nQueres guardar e sair?",
                "Pausa", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                _jogo.SuspenderJogo();
                MessageBox.Show("Jogo guardado com sucesso!", "Guardado",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
                return;
            }
            else if (resultado == MessageBoxResult.No)
            {
                // Continua sem guardar
            }
            // Cancel — fica na pausa, retoma
            if (_jogo.JogoEmCurso && _jogo.JogadorAtivo != _jogo.Jogadores[0])
                _timerBot.Start();
        }

        private void BtnAjuda_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Como jogar UNO:\n\n" +
                "• Joga uma carta da mesma cor ou símbolo que a carta do topo.\n" +
                "• Joker: muda a cor ativa.\n" +
                "• Joker +4: muda a cor e o próximo jogador compra 4 cartas.\n" +
                "• +2: o próximo jogador compra 2 cartas.\n" +
                "• Inverter: inverte o sentido de jogo.\n" +
                "• Salta: o próximo jogador perde a vez.\n" +
                "• Se não tiveres cartas jogáveis, compra uma carta.\n" +
                "• O primeiro a ficar sem cartas ganha!",
                "Ajuda — Regras do UNO",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tens a certeza que queres sair?", "Sair",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
            => _timerBot.Stop();

        // ═════════════════════════════════════════════════════════════════════
        //  HELPERS — IMAGENS E CORES
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Mapeia Carta → caminho do asset.
        ///
        /// Nomes dos ficheiros confirmados:
        ///   Números  : red_0, blue_5, green_9, yellow_3, …
        ///   Compra2  : red_draw_two, blue_draw_two, …
        ///   Inverter : red_reverse, blue_reverse, …
        ///   Salta    : red_skip, blue_skip, …
        ///   Joker    : wild_card
        ///   Joker4   : wild_card   (usa o mesmo asset; ajusta se tiveres wild_draw_four)
        ///   Costas   : card_back
        /// </summary>
        private static string ObterCaminhoImagem(Carta carta)
        {
            // Wildcards não têm cor
            if (carta.Simbolo == SimboloCarta.Joker)
                return "/Assets/wild_card.png";
            if (carta.Simbolo == SimboloCarta.Joker4)
                return "/Assets/wild_card_four.png";   // ajusta se o ficheiro tiver outro nome

            string cor = carta.Cor switch
            {
                CorCarta.Vermelho => "red",
                CorCarta.Verde    => "green",
                CorCarta.Azul     => "blue",
                CorCarta.Amarelo  => "yellow",
                _                 => "red"
            };

            string simbolo = carta.Simbolo switch
            {
                SimboloCarta.Zero    => "0",
                SimboloCarta.Um      => "1",
                SimboloCarta.Dois    => "2",
                SimboloCarta.Tres    => "3",
                SimboloCarta.Quatro  => "4",
                SimboloCarta.Cinco   => "5",
                SimboloCarta.Seis    => "6",
                SimboloCarta.Sete    => "7",
                SimboloCarta.Oito    => "8",
                SimboloCarta.Nove    => "9",
                SimboloCarta.Compra2  => "draw_two",
                SimboloCarta.Inverter => "reverse",
                SimboloCarta.Salta    => "skip",
                _                     => "0"
            };

            return $"/Assets/{cor}_{simbolo}.png";
        }

        private static BitmapImage? CarregarImagem(string path)
        {
            try { return new BitmapImage(new Uri(path, UriKind.Relative)); }
            catch { return null; }
        }

        private static Brush CorParaPincel(CorCarta cor) => cor switch
        {
            CorCarta.Vermelho => new SolidColorBrush(Color.FromRgb(0xC0, 0x39, 0x2B)),
            CorCarta.Verde    => new SolidColorBrush(Color.FromRgb(0x27, 0xAE, 0x60)),
            CorCarta.Azul     => new SolidColorBrush(Color.FromRgb(0x29, 0x80, 0xB9)),
            CorCarta.Amarelo  => new SolidColorBrush(Color.FromRgb(0xF1, 0xC4, 0x0F)),
            _                 => Brushes.Gray
        };

        private static string CorParaNome(CorCarta cor) => cor switch
        {
            CorCarta.Vermelho => "Vermelho",
            CorCarta.Verde    => "Verde",
            CorCarta.Azul     => "Azul",
            CorCarta.Amarelo  => "Amarelo",
            _                 => "—"
        };
    }
}
