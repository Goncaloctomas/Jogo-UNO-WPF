using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Jogo
{
    /// <summary>
    /// Responsável por guardar e carregar o estado do jogo e as estatísticas em XML.
    /// Os ficheiros ficam na pasta pessoal do utilizador Windows (%USERPROFILE%\UNO\).
    /// </summary>
    public static class GestorXml
    {
        // ── Caminhos ─────────────────────────────────────────────────────────────

        private static string PastaBase =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "UNO");

        private static string FicheiroEstatisticas =>
            Path.Combine(PastaBase, "estatisticas.xml");

        private static string FicheiroJogoGuardado =>
            Path.Combine(PastaBase, "jogo_guardado.xml");

        private static void GarantirPasta()
        {
            if (!Directory.Exists(PastaBase))
                Directory.CreateDirectory(PastaBase);
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  ESTATÍSTICAS
        // ═════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Guarda as estatísticas de todos os jogadores em XML.
        /// </summary>
        public static void GuardarEstatisticas(List<Jogador> jogadores)
        {
            GarantirPasta();

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Estatisticas",
                    new XAttribute("DataGuardado", DateTime.Now.ToString("o")),
                    jogadores.Select(j =>
                        new XElement("Jogador",
                            new XElement("Nome",              j.Nome),
                            new XElement("Fotografia",        j.Fotografia),
                            new XElement("N_Partidas_Jogadas",j.N_Partidas_Jogadas),
                            new XElement("N_Partidas_Ganhos", j.N_Partidas_Ganhos),
                            new XElement("N_Jogos_Jogados",   j.N_Jogos_Jogados),
                            new XElement("N_Jogos_Ganhos",    j.N_Jogos_Ganhos)
                        )
                    )
                )
            );

            doc.Save(FicheiroEstatisticas);
        }

        /// <summary>
        /// Carrega estatísticas do XML e aplica-as à lista de jogadores fornecida.
        /// Jogadores não encontrados no XML mantêm os valores atuais.
        /// </summary>
        public static void CarregarEstatisticas(List<Jogador> jogadores)
        {
            if (!File.Exists(FicheiroEstatisticas)) return;

            try
            {
                var doc = XDocument.Load(FicheiroEstatisticas);

                foreach (var j in jogadores)
                {
                    var elem = doc.Root?
                        .Elements("Jogador")
                        .FirstOrDefault(e => (string?)e.Element("Nome") == j.Nome);

                    if (elem == null) continue;

                    j.N_Partidas_Jogadas = (int?)elem.Element("N_Partidas_Jogadas") ?? 0;
                    j.N_Partidas_Ganhos  = (int?)elem.Element("N_Partidas_Ganhos")  ?? 0;
                    j.N_Jogos_Jogados    = (int?)elem.Element("N_Jogos_Jogados")    ?? 0;
                    j.N_Jogos_Ganhos     = (int?)elem.Element("N_Jogos_Ganhos")     ?? 0;
                }
            }
            catch { /* ficheiro corrompido — ignora */ }
        }

        /// <summary>
        /// Carrega todos os jogadores guardados no XML de estatísticas.
        /// Usado para mostrar estatísticas históricas mesmo sem jogo ativo.
        /// </summary>
        public static List<Jogador> CarregarTodosJogadores()
        {
            var lista = new List<Jogador>();
            if (!File.Exists(FicheiroEstatisticas)) return lista;

            try
            {
                var doc = XDocument.Load(FicheiroEstatisticas);
                foreach (var elem in doc.Root?.Elements("Jogador") ?? [])
                {
                    lista.Add(new Jogador
                    {
                        Nome               = (string?)elem.Element("Nome")               ?? "",
                        Fotografia         = (string?)elem.Element("Fotografia")         ?? "",
                        N_Partidas_Jogadas = (int?)elem.Element("N_Partidas_Jogadas")    ?? 0,
                        N_Partidas_Ganhos  = (int?)elem.Element("N_Partidas_Ganhos")     ?? 0,
                        N_Jogos_Jogados    = (int?)elem.Element("N_Jogos_Jogados")       ?? 0,
                        N_Jogos_Ganhos     = (int?)elem.Element("N_Jogos_Ganhos")        ?? 0
                    });
                }
            }
            catch { }

            return lista;
        }

        public static void LimparEstatisticas()
        {
            if (File.Exists(FicheiroEstatisticas))
                File.Delete(FicheiroEstatisticas);
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  GUARDAR / CARREGAR ESTADO DO JOGO
        // ═════════════════════════════════════════════════════════════════════════

        public static bool ExisteJogoGuardado =>
            File.Exists(FicheiroJogoGuardado);

        /// <summary>
        /// Serializa o estado completo do jogo para XML.
        /// </summary>
        public static void GuardarJogo(Game jogo)
        {
            GarantirPasta();

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("JogoGuardado",
                    new XAttribute("DataGuardado", DateTime.Now.ToString("o")),

                    // Estado da partida
                    new XElement("Estado",
                        new XElement("IndiceAtivo",           jogo.IndiceAtivo),
                        new XElement("SentidoNormal",         jogo.SentidoNormal),
                        new XElement("CartasPendentesCompra", jogo.CartasPendentesCompra),
                        new XElement("CorAtiva",              (int)jogo.Mesa.CorAtiva)
                    ),

                    // Pontuações acumuladas
                    new XElement("Pontuacoes",
                        jogo.Pontuacoes.Select((p, i) =>
                            new XElement("Pontuacao",
                                new XAttribute("Indice", i), p))
                    ),

                    // Jogadores e as suas mãos
                    new XElement("Jogadores",
                        jogo.Jogadores.Select(j =>
                            new XElement("Jogador",
                                new XElement("Nome",              j.Nome),
                                new XElement("Fotografia",        j.Fotografia),
                                new XElement("N_Partidas_Jogadas",j.N_Partidas_Jogadas),
                                new XElement("N_Partidas_Ganhos", j.N_Partidas_Ganhos),
                                new XElement("N_Jogos_Jogados",   j.N_Jogos_Jogados),
                                new XElement("N_Jogos_Ganhos",    j.N_Jogos_Ganhos),
                                new XElement("Mao",
                                    j.Cartas.Select(c =>
                                        new XElement("Carta",
                                            new XAttribute("Id",      c.Id),
                                            new XAttribute("Cor",     (int)c.Cor),
                                            new XAttribute("Simbolo", (int)c.Simbolo)
                                        )
                                    )
                                )
                            )
                        )
                    ),

                    // Baralho restante
                    new XElement("Baralho",
                        jogo.Mesa.Baralho.Cartas.Select(c =>
                            new XElement("Carta",
                                new XAttribute("Id",      c.Id),
                                new XAttribute("Cor",     (int)c.Cor),
                                new XAttribute("Simbolo", (int)c.Simbolo)
                            )
                        )
                    ),

                    // Pilha de descarte
                    new XElement("Descarte",
                        jogo.Mesa.CartasJogadas.Select(c =>
                            new XElement("Carta",
                                new XAttribute("Id",      c.Id),
                                new XAttribute("Cor",     (int)c.Cor),
                                new XAttribute("Simbolo", (int)c.Simbolo)
                            )
                        )
                    )
                )
            );

            doc.Save(FicheiroJogoGuardado);
        }

        /// <summary>
        /// Carrega o estado do jogo a partir do XML.
        /// Devolve um novo Game pronto a jogar, ou null se falhar.
        /// </summary>
        public static Game? CarregarJogo()
        {
            if (!File.Exists(FicheiroJogoGuardado)) return null;

            try
            {
                var doc  = XDocument.Load(FicheiroJogoGuardado);
                var root = doc.Root!;

                var jogo = new Game();

                // Jogadores
                foreach (var elem in root.Element("Jogadores")?.Elements("Jogador") ?? [])
                {
                    var j = new Jogador
                    {
                        Nome               = (string?)elem.Element("Nome")               ?? "",
                        Fotografia         = (string?)elem.Element("Fotografia")         ?? "",
                        N_Partidas_Jogadas = (int?)elem.Element("N_Partidas_Jogadas")    ?? 0,
                        N_Partidas_Ganhos  = (int?)elem.Element("N_Partidas_Ganhos")     ?? 0,
                        N_Jogos_Jogados    = (int?)elem.Element("N_Jogos_Jogados")       ?? 0,
                        N_Jogos_Ganhos     = (int?)elem.Element("N_Jogos_Ganhos")        ?? 0
                    };

                    foreach (var ce in elem.Element("Mao")?.Elements("Carta") ?? [])
                        j.Cartas.Add(CartaDeXml(ce));

                    jogo.Jogadores.Add(j);
                }

                // Pontuações
                foreach (var pe in root.Element("Pontuacoes")?.Elements("Pontuacao") ?? [])
                    jogo.Pontuacoes.Add((int)pe);

                // Baralho
                foreach (var ce in root.Element("Baralho")?.Elements("Carta") ?? [])
                    jogo.Mesa.Baralho.Cartas.Add(CartaDeXml(ce));

                // Descarte
                foreach (var ce in root.Element("Descarte")?.Elements("Carta") ?? [])
                    jogo.Mesa.CartasJogadas.Add(CartaDeXml(ce));

                // Estado
                var estado = root.Element("Estado")!;
                jogo.RestaurarEstado(
                    indiceAtivo:           (int)estado.Element("IndiceAtivo")!,
                    sentidoNormal:         (bool)estado.Element("SentidoNormal")!,
                    cartasPendentes:       (int)estado.Element("CartasPendentesCompra")!,
                    corAtiva:              (CorCarta)(int)estado.Element("CorAtiva")!
                );

                return jogo;
            }
            catch
            {
                return null;
            }
        }

        public static void ApagarJogoGuardado()
        {
            if (File.Exists(FicheiroJogoGuardado))
                File.Delete(FicheiroJogoGuardado);
        }

        // ── Helper ────────────────────────────────────────────────────────────────

        private static Carta CartaDeXml(XElement e) => new Carta
        {
            Id      = (int)e.Attribute("Id")!,
            Cor     = (CorCarta)(int)e.Attribute("Cor")!,
            Simbolo = (SimboloCarta)(int)e.Attribute("Simbolo")!
        };
    }
}
