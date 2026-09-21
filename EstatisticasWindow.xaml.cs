using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Jogo
{
    public partial class EstatisticasWindow : Window
    {
        public EstatisticasWindow(List<Jogador> jogadores)
        {
            InitializeComponent();
            CarregarEstatisticas(jogadores);
        }

        private void CarregarEstatisticas(List<Jogador> jogadores)
        {
            if (jogadores == null || jogadores.Count == 0)
            {
                TxtSemDados.Visibility      = Visibility.Visible;
                ListaEstatisticas.Visibility = Visibility.Collapsed;
                return;
            }

            TxtSemDados.Visibility      = Visibility.Collapsed;
            ListaEstatisticas.Visibility = Visibility.Visible;

            var dados = jogadores.Select(j => new JogadorEstatisticaVM
            {
                Nome               = j.Nome,
                N_Partidas_Jogadas = j.N_Partidas_Jogadas,
                N_Partidas_Ganhos  = j.N_Partidas_Ganhos,
                N_Jogos_Jogados    = j.N_Jogos_Jogados,
                N_Jogos_Ganhos     = j.N_Jogos_Ganhos,
                TaxaVitoriaStr     = j.N_Partidas_Jogadas > 0
                    ? $"{(j.N_Partidas_Ganhos * 100.0 / j.N_Partidas_Jogadas):F1}%"
                    : "—"
            }).ToList();

            ListaEstatisticas.ItemsSource = dados;
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Tens a certeza que queres apagar todas as estatísticas?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                // Apaga o ficheiro XML
                GestorXml.LimparEstatisticas();

                ListaEstatisticas.ItemsSource    = null;
                TxtSemDados.Visibility           = Visibility.Visible;
                ListaEstatisticas.Visibility     = Visibility.Collapsed;
            }
        }
    }

    // ViewModel auxiliar para a tabela
    public class JogadorEstatisticaVM
    {
        public string Nome               { get; set; } = string.Empty;
        public int    N_Partidas_Jogadas { get; set; }
        public int    N_Partidas_Ganhos  { get; set; }
        public int    N_Jogos_Jogados    { get; set; }
        public int    N_Jogos_Ganhos     { get; set; }
        public string TaxaVitoriaStr     { get; set; } = "—";
    }
}
