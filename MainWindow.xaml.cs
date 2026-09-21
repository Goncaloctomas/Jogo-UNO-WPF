using System.Collections.Generic;
using System.Windows;

namespace Jogo
{
    public partial class MainWindow : Window
    {
        private Game jogo;

        public MainWindow()
        {
            InitializeComponent();
            jogo = new Game();

            // Botão "Carregar Jogo" só ativo se existir jogo guardado
            BtnCarregarJogo.IsEnabled = GestorXml.ExisteJogoGuardado;
        }

        private void BtnComecarJogo_Click(object sender, RoutedEventArgs e)
        {
            var selecaoBots = new SelecaoBotsWindow();
            if (selecaoBots.ShowDialog() != true) return;

            int numeroDeBots = selecaoBots.NumeroDeBots;

            jogo = new Game();

            // Nome do perfil Windows
            string nomeWindows = System.Environment.UserName;
            jogo.Jogadores.Add(new Jogador { Nome = nomeWindows });

            for (int i = 1; i <= numeroDeBots; i++)
                jogo.Jogadores.Add(new Jogador { Nome = $"Bot {i}" });

            // Carrega estatísticas históricas do XML
            GestorXml.CarregarEstatisticas(jogo.Jogadores);

            jogo.IniciarNovoJogo();
            AbrirJanelaDeJogo();
        }

        private void BtnCarregarJogo_Click(object sender, RoutedEventArgs e)
        {
            var jogoCarregado = GestorXml.CarregarJogo();
            if (jogoCarregado == null)
            {
                MessageBox.Show("Não foi possível carregar o jogo guardado.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            jogo = jogoCarregado;
            AbrirJanelaDeJogo();
        }

        private void AbrirJanelaDeJogo()
        {
            var jogoWindow = new JogoWindow(jogo);
            jogoWindow.ShowDialog();

            // Atualiza botão após fechar a janela
            BtnCarregarJogo.IsEnabled = GestorXml.ExisteJogoGuardado;
        }

        private void BtnEstatisticas_Click(object sender, RoutedEventArgs e)
        {
            var jogadores = jogo.Jogadores.Count > 0
                ? jogo.Jogadores
                : GestorXml.CarregarTodosJogadores();

            var janela = new EstatisticasWindow(jogadores);
            janela.ShowDialog();
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
