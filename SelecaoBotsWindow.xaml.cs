using System.Windows;

namespace Jogo
{
    /// <summary>
    /// Janela de seleção do número de bots adversários.
    /// Abre quando o utilizador clica em "Começar Jogo" no menu principal.
    /// </summary>
    public partial class SelecaoBotsWindow : Window
    {
        /// <summary>
        /// Número de bots selecionado pelo utilizador (1, 2 ou 3).
        /// </summary>
        public int NumeroDeBots { get; private set; } = 1;

        public SelecaoBotsWindow()
        {
            InitializeComponent();
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Fecha esta janela e volta ao menu principal
            this.Close();
        }

        private void BtnJogar_Click(object sender, RoutedEventArgs e)
        {
            // Determina a opção selecionada
            if (Rb1Adversario.IsChecked == true)
                NumeroDeBots = 1;
            else if (Rb2Adversarios.IsChecked == true)
                NumeroDeBots = 2;
            else
                NumeroDeBots = 3;

            // Define DialogResult como true para que o chamador saiba que o utilizador confirmou
            this.DialogResult = true;
            this.Close();
        }
    }
}
