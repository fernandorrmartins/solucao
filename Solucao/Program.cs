using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;

namespace Solucao
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu menuPrincipal = new Menu();
        }
    }

    #region Classes de Objetos para o bom Funcionamento do Sistema
    /// <summary>
    /// Classe Responsável por Gerenciar o Menu e suas Opções
    /// </summary>
    class Menu
    {
        #region Atributos da Classe
        /// <summary>
        /// Vairável que controla o fluxo do menu. Recebe o comando do usuário
        /// </summary>
        String cmd;
        /// <summary>
        /// Armazena a lista de opções
        /// </summary>
        StringBuilder opcoesMenu = new StringBuilder();
        /// <summary>
        /// Atributo responsável por armazenar a lista de reunióes registradas
        /// e gerenciar os arquivos .json
        /// </summary>
        GerenciadorJson gerenciadorJson = new GerenciadorJson();
        #endregion
        
        /// <summary>
        /// Método Construtor da Classe Menu
        /// </summary>
        public Menu()
        {
            opcoesMenu.Append("############################################# Menu Principal #################################################\n");
            opcoesMenu.Append("#                                                                                                            #\n");
            opcoesMenu.Append("# (1) Agendar reunião                                                                                        #\n");
            opcoesMenu.Append("# (2) Ver reuniões agendadas                                                                                 #\n");
            opcoesMenu.Append("# (0) Sair                                                                                                   #\n");
            opcoesMenu.Append("#                                                                                                            #\n");
            opcoesMenu.Append("##############################################################################################################\nDigite sua opção: ");
            apresentarMenu();
        }

        /// <summary>
        /// Função que apresenta o menu para o usuário e gerencia a escolha das opções
        /// </summary>
        public void apresentarMenu()
        {
            Console.Write(opcoesMenu);
            cmd = Console.ReadLine();

            // Condição para registrar uma nova reunião
            if (cmd.Equals("1"))
            {
                Console.Clear();

                // Recebe a entrada do usuário
                string agendar = Console.ReadLine();
                Console.Clear();

                // Verifica se é uma entrada valida
                int count = agendar.Split(';').Length - 1;
                if (count == 6)
                {
                    // Separa as entradas para criar o objeto de Reunião
                    string[] arrayReuniao = agendar.Split(';');
                    DateTime dataInicio = Convert.ToDateTime(arrayReuniao[0] + " " + arrayReuniao[1]);
                    DateTime dataFim = Convert.ToDateTime(arrayReuniao[2] + " " + arrayReuniao[3]);
                    int qtdPessoas = Convert.ToInt32(arrayReuniao[4]);
                    bool internet = (arrayReuniao[5].ToLower().Equals("sim") ? true : false);
                    bool tv_webcam = (arrayReuniao[6].ToLower().Equals("sim") ? true : false);

                    // Cria o objeto de reunião
                    Reuniao reuniao = new Reuniao(dataInicio, dataFim, qtdPessoas, internet, tv_webcam);

                    // Recebe nulo se não for uma reunião válida, ou uma sala se for válida
                    reuniao.salaDeReuniao = Reuniao.avaliarSePodeAgendarReuniao(reuniao, gerenciadorJson);
                    // Se a sala for diferente de nulo, ele salva a reunião no arquivo .json com as listas de reuniões
                    if (reuniao.salaDeReuniao != null)
                    {
                        gerenciadorJson.reunioes.Add(reuniao);
                        gerenciadorJson.salvarReunioes();
                    }
                }
                // Se não for entrada válida, informa ao usuário
                else
                {
                    StringBuilder mensagem = new StringBuilder("Formato incorreto para agendamento!\n\n");
                    mensagem.Append("Pressione qualquer tecla para continuar...");
                    Console.Write(mensagem);
                }

                Console.ReadKey();
                Console.Clear();
                apresentarMenu();
            }
            // Condições para listar as reuniões registradas
            else if (cmd.Equals("2"))
            {
                Console.Clear();
                // Conidção para Listar as Reuniões
                if (gerenciadorJson.reunioes.Count > 0)
                {
                    StringBuilder listarReunioes = new StringBuilder();
                    foreach (Reuniao reuniao in gerenciadorJson.reunioes)
                    {
                        listarReunioes.Append("| Número da Sala: " + (reuniao.salaDeReuniao.numero < 10 ? "0" + reuniao.salaDeReuniao.numero : reuniao.salaDeReuniao.numero.ToString())
                            + " | Hora/Data de Inicio: " + reuniao.dataInicio.ToString()
                            + " | Hora/Data de Fim: " + reuniao.dataFim.ToString()
                            + " | Internet: " + (reuniao.internet ? "Sim" : "Não")
                            + " | TV e Webcam: " + (reuniao.tv_webcam ? "Sim" : "Não")
                            + " |\n");
                    }
                    Console.Write(listarReunioes);
                }
                /*
                 * Condição se não houverem reuniões registradas, apresenta uma mensagem
                 * informando ao usuário e retorna ao menu principal, após o usuário
                 * pressionar uma tecla
                 */
                else
                {
                    StringBuilder mensagem = new StringBuilder();
                    mensagem.Append("Não há reuniões registradas!\n\n\n");
                    mensagem.Append("Pressione qualquer tecla para voltar ao menu principal...");
                    Console.WriteLine(mensagem);
                }
                Console.ReadKey();
                Console.Clear();
                apresentarMenu();
            }
            // Condição para encerrar a solução
            else if (cmd.Equals("0"))
            {
                Console.Clear();
                Console.WriteLine("Sistema finalizado. Pressione qualquer tecla para finalizar...");
                Console.ReadKey();
            }
            // Condição padrão para qualquer opção digitada inválida
            else
            {
                Console.Clear();
                Console.WriteLine("############################################# Opção Invalida #################################################");
                apresentarMenu();
            }
        }
    }

    /// <summary>
    /// Classe Responsável por Representar Registros de Salas
    /// </summary>
    class Sala
    {
        #region Atributos
        /// <summary>
        /// Número da Sala
        /// </summary>
        public int numero;
        /// <summary>
        /// Quantidade Máxima de Pessoas para a sala
        /// </summary>
        public int maxPessoas;
        /// <summary>
        /// Variável que indica se a sala possui internet ou não
        /// </summary>
        public bool internet;
        /// <summary>
        /// Variável que indica se a sala possui tv e webcam para vídeo conferencias ou não
        /// </summary>
        public bool tv_webcam;
        #endregion

        /// <summary>
        /// Método Construtor, responsável por inicializar o objeto
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="maxPessoas"></param>
        /// <param name="internet"></param>
        /// <param name="tv_webcam"></param>
        public Sala(int numero, int maxPessoas, bool internet, bool tv_webcam)
        {
            this.numero = numero;
            this.maxPessoas = maxPessoas;
            this.internet = internet;
            this.tv_webcam = tv_webcam;
        }
    }

    /// <summary>
    /// Classe Responsável por Gerenciar os Arquivos .json gerados pelo programa
    /// </summary>
    class GerenciadorJson
    {
        #region Atributos da Classe
        /// <summary>
        /// Variável Responsável por Armazenar Caminho + Nome do Arquivo .json
        /// que armazena as reuniões
        /// </summary>
        private string arquivoDeReunioes = "ListaReuniao.json";
        /// <summary>
        /// Variável Responsável por Armazenar Caminho + Nome do Arquivo .json
        /// que armazena as salas
        /// </summary>
        private string arquivoDeSalas = "ListaSalas.json";
        /// <summary>
        /// Atributo Responsável por Guardar Lista das Reuniões Salvas ao Carregar ou Registrar uma nova Reunião
        /// </summary>
        public List<Reuniao> reunioes = null;
        /// <summary>
        /// Atributo Responsável por Guardar Lista de Salas ao Carregar o Sistema
        /// </summary>
        public List<Sala> salas = null;
        #endregion

        /// <summary>
        /// Método Contrutor da Classe
        /// Responsável por Carregar As Reuniões já Registradas Assim que o Programa Inicializa
        /// </summary>
        public GerenciadorJson()
        {
            // Carrega as Reuniões Salvas para Apresentar quando requisitado
            carregarReunioesSalvas();
            carregarSalas();
        }

        /// <summary>
        /// Função que carrega todas as Reuniões salvas no arquivo .json da pasta do programa
        /// </summary>
        public void carregarReunioesSalvas()
        {
            /* 
             * Parte que verificar se o arquivo .json responsável por guardar
             * os registro existe, e se não existe cria
             */
            try
            {
                if (!File.Exists(arquivoDeReunioes))
                {
                    File.Create(arquivoDeReunioes).Dispose();
                    using (StreamWriter w = File.AppendText(arquivoDeReunioes))
                    {
                        w.WriteLine("[]");
                        w.Close();
                    }
                    carregarReunioesSalvas();
                }
                else
                {
                    using (StreamReader reader = new StreamReader(arquivoDeReunioes))
                    {
                        /*
                         *Parte que carrega o arquivo .json responsável por carregar
                         * os registros das reuniões
                         */
                        try
                        {
                            var json = reader.ReadToEnd();
                            reunioes = JsonConvert.DeserializeObject<List<Reuniao>>(json);
                            reader.Close();
                        }
                        catch (Exception e)
                        {
                            /*
                             * Tratamento de erro caso não seja possível ler o arquivo 
                             * .json por estar corrompido ou outro problema
                             */
                            Console.Clear();
                            StringBuilder erro = new StringBuilder();
                            erro.Append("Ocorreu um erro ao ler o arquivo json!\n");
                            erro.Append(e.Message);
                            Console.WriteLine(erro);
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                /*
                 *Tratamento de erro caso não seja possível ler ou criar o arquivo
                 * .json por estar com falta de permissão ou outro problema
                 */
                Console.Clear();
                StringBuilder erro = new StringBuilder();
                erro.Append("Ocorreu um erro ao criar o arquivo de reuniões!\n");
                erro.Append(e.Message);
                Console.WriteLine(erro);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Função que carrega todas as Salas no arquivo .json da pasta do programa
        /// </summary>
        public void carregarSalas()
        {
            /* 
             * Parte que verificar se o arquivo .json responsável por guardar
             * os registro existe, e se não existe cria
             */
            try
            {
                if (!File.Exists(arquivoDeSalas))
                {
                    File.Create(arquivoDeSalas).Dispose();
                    using (StreamWriter w = File.AppendText(arquivoDeSalas))
                    {
                        w.WriteLine(
                                    @"[
                                        {
                                            'numero' : 1,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 2,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 3,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 4,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 5,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 6,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : false,
	                                    },
	                                    {
                                        'numero' : 7,
	                                    'maxPessoas' : 10,
	                                    'internet' : true,
	                                    'tv_webcam' : false,
	                                    },
	                                    {
                                        'numero' : 8,
	                                    'maxPessoas' : 3,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 9,
	                                    'maxPessoas' : 3,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 10,
	                                    'maxPessoas' : 3,
	                                    'internet' : true,
	                                    'tv_webcam' : true,
	                                    },
	                                    {
                                        'numero' : 11,
	                                    'maxPessoas' : 20,
	                                    'internet' : false,
	                                    'tv_webcam' : false,
	                                    },
	                                    {
                                        'numero' : 12,
	                                    'maxPessoas' : 20,
	                                    'internet' : false,
	                                    'tv_webcam' : false,
	                                    }
                                    ]");
                        w.Close();
                    }
                    carregarSalas();
                }
                else
                {
                    using (StreamReader reader = new StreamReader(arquivoDeSalas))
                    {
                        /*
                         *Parte que carrega o arquivo .json responsável por carregar
                         * os registros das salas
                         */
                        try
                        {
                            var json = reader.ReadToEnd();
                            salas = JsonConvert.DeserializeObject<List<Sala>>(json);
                            reader.Close();
                        }
                        catch (Exception e)
                        {
                            /*
                             * Tratamento de erro caso não seja possível ler o arquivo 
                             * .json por estar corrompido ou outro problema
                             */
                            Console.Clear();
                            StringBuilder erro = new StringBuilder();
                            erro.Append("Ocorreu um erro ao ler o arquivo json!\n");
                            erro.Append(e.Message);
                            Console.WriteLine(erro);
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                /*
                 *Tratamento de erro caso não seja possível ler ou criar o arquivo
                 * .json por estar com falta de permissão ou outro problema
                 */
                Console.Clear();
                StringBuilder erro = new StringBuilder();
                erro.Append("Ocorreu um erro ao criar o arquivo de reuniões!\n");
                erro.Append(e.Message);
                Console.WriteLine(erro);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Salva as reuniões no arquivo .json responsável
        /// para acessos futuros
        /// </summary>
        /// <param name="reunioes"></param>
        public void salvarReunioes()
        {
            try
            {
                // Salva a lista de reuniões no arquivo pertinente
                string json = JsonConvert.SerializeObject(reunioes);
                FileStream stream = new FileStream(arquivoDeReunioes, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                writer.Close();
                stream.Close();
                carregarReunioesSalvas();
            }
            catch (Exception e)
            {
                /*
                 * Tratamento de erro durante o processo de registro de reuniões
                 * no arquivo pertinente
                 */
                Console.Clear();
                StringBuilder erro = new StringBuilder();
                erro.Append("Ocorreu um erro!\n");
                erro.Append(e.Message);
                Console.WriteLine(erro);
                Console.ReadKey();
            }
        }
    }

    /// <summary>
    /// Classe Responsável por Representar, Guardar, Salvar e Apresentar Registros de Reuniões
    /// </summary>
    class Reuniao
    {
        #region Attributos da Classe
        /// <summary>
        /// Data de Inicio da Reunião
        /// </summary>
        public DateTime dataInicio;
        /// <summary>
        /// Data de Fim da Reunião
        /// </summary>
        public DateTime dataFim;
        /// <summary>
        /// Quantidade de Pessoas que irão comparecer a reunião
        /// </summary>
        public int qtdPessoas;
        /// <summary>
        /// Variável Booleana que informa se há ou não internet na sala de reunião
        /// </summary>
        public bool internet;
        /// <summary>
        /// Variável Booleana que informa se há ou não tv e webcam na sala de reunião
        /// </summary>
        public bool tv_webcam;
        /// <summary>
        /// Variável que armazena a sala de reunião para uma reunião já registrada
        /// </summary>
        public Sala salaDeReuniao;
        #endregion

        /// <summary>
        /// Gera uma data aleátoria a partir de hoje com no minimo 1 dia e no máximo 40 dias
        /// </summary>
        /// <returns></returns>
        private static DateTime[] gerarDatasAleatoria()
        {
            Random gen = new Random();
            DateTime start = DateTime.Today;
            int diasAdicionais = gen.Next(1, 40);
            DateTime[] datas = { start.AddDays(gen.Next(1, 40)).AddHours(gen.Next(0, 23)).AddMinutes(gen.Next(0, 59)), start.AddDays(gen.Next(1, 40)).AddHours(gen.Next(0, 23)).AddMinutes(gen.Next(0, 59)), start.AddDays(gen.Next(1, 40)).AddHours(gen.Next(0, 23)).AddMinutes(gen.Next(0, 59)) };
            if(!datas[0].DayOfWeek.ToString().ToLower().Equals("sunday")
               && !datas[0].DayOfWeek.ToString().ToLower().Equals("saturday")
               && !datas[1].DayOfWeek.ToString().ToLower().Equals("sunday")
               && !datas[1].DayOfWeek.ToString().ToLower().Equals("saturday")
               && !datas[2].DayOfWeek.ToString().ToLower().Equals("sunday")
               && !datas[2].DayOfWeek.ToString().ToLower().Equals("saturday"))
            {
                return datas;
            } else
            {
                return gerarDatasAleatoria();
            }
        }

        /// <summary>
        /// Gera o json da reunião no formato de entrada do usuário
        /// </summary>
        /// <param name="reuniao"></param>
        /// <returns></returns>
        private static string gerarJsonDeReuniao(Reuniao reuniao)
        {
            String json = reuniao.dataInicio.Day + "/" + reuniao.dataInicio.Month + "/" + reuniao.dataInicio.Year + ";"
                        + reuniao.dataInicio.Hour + ":" + reuniao.dataInicio.Minute + ";"
                        + reuniao.dataFim.Day + "/" + reuniao.dataFim.Month + "/" + reuniao.dataFim.Year + ";"
                        + reuniao.dataFim.Hour + ":" + reuniao.dataFim.Minute + ";"
                        + reuniao.qtdPessoas + ";"
                        + (reuniao.internet ? "Sim" : "Não") + ";"
                        + (reuniao.tv_webcam ? "Sim" : "Não");

            return json;
        }

        /// <summary>
        /// Gera 3 exemplos para apresentar ao usuário
        /// </summary>
        /// <param name="agendar"></param>
        /// <param name="gerenciadorJson"></param>
        /// <returns></returns>
        private static void gerarExemplos(Reuniao reuniao)
        {
            DateTime[] datasNovas = gerarDatasAleatoria();

            StringBuilder mensagem = new StringBuilder();
            mensagem.Append("Segue algumas opções de agendamento para reuniões:\n");
            Reuniao r1 = new Reuniao(datasNovas[0], reuniao.dataFim, reuniao.qtdPessoas, reuniao.internet, reuniao.tv_webcam);
            Reuniao r2 = new Reuniao(datasNovas[1], reuniao.dataFim, reuniao.qtdPessoas, reuniao.internet, reuniao.tv_webcam);
            Reuniao r3 = new Reuniao(datasNovas[2], reuniao.dataFim, reuniao.qtdPessoas, reuniao.internet, reuniao.tv_webcam);
            mensagem.Append(gerarJsonDeReuniao(r1) + "\n");
            mensagem.Append(gerarJsonDeReuniao(r2) + "\n");
            mensagem.Append(gerarJsonDeReuniao(r3) + "\n");
            Console.Write(mensagem);
        }
        
        /// <summary>
        /// Avalia se a reunião informada pelo usuário é válida, e se for retorna uma sala para a reunião, se não, retorna nulo
        /// </summary>
        /// <param name="reuniao"></param>
        /// <param name="gerenciadorJson"></param>
        /// <returns></returns>
        public static Sala avaliarSePodeAgendarReuniao(Reuniao reuniao, GerenciadorJson gerenciadorJson)
        {
            // Verifica se a data informa é maior que o prazo minimo para agendamento(24 horas)
            if (DateTime.Today.AddHours(24) < reuniao.dataInicio)
            {
                // Verifica se a data é diferente de um dia útil
                if (!reuniao.dataInicio.DayOfWeek.ToString().ToLower().Equals("sunday")
                    && !reuniao.dataInicio.DayOfWeek.ToString().ToLower().Equals("saturday")
                    && !reuniao.dataFim.DayOfWeek.ToString().ToLower().Equals("sunday")
                    && !reuniao.dataFim.DayOfWeek.ToString().ToLower().Equals("saturday"))
                {
                    bool podeRegistrarEssaSala;
                    // Percorre todas as salas registradas na lista de salas
                    foreach (Sala s in gerenciadorJson.salas)
                    {
                        // Verifica se as informações da reunião batem com a da sala
                        if (s.maxPessoas >= reuniao.qtdPessoas 
                            && (s.internet == reuniao.internet || !reuniao.internet) 
                            && (s.tv_webcam == reuniao.tv_webcam || !reuniao.tv_webcam))
                        {
                            if (gerenciadorJson.reunioes.Count > 0)
                            {
                                // Percorre a lista de reuniões agendadas para ver se nenhum horário conflita
                                foreach (Reuniao r in gerenciadorJson.reunioes)
                                {
                                    if (s.numero != r.salaDeReuniao.numero
                                        || (s.numero == r.salaDeReuniao.numero
                                        && ((reuniao.dataInicio > r.dataFim && reuniao.dataInicio > r.dataInicio)
                                            || (reuniao.dataInicio < r.dataInicio && reuniao.dataFim < r.dataInicio))))
                                    {
                                        bool naoPode = false;
                                        foreach (Reuniao re in gerenciadorJson.reunioes)
                                        {
                                            if (s.numero == re.salaDeReuniao.numero 
                                                && !((reuniao.dataInicio > r.dataFim && reuniao.dataInicio > r.dataInicio)
                                            || (reuniao.dataInicio < r.dataInicio && reuniao.dataFim < r.dataInicio))) naoPode = true;
                                        }
                                        if (naoPode) break;
                                        // Informa ao usuário a sala registrada
                                        Console.WriteLine("Sala " + s.numero);
                                        //Retorna a sala para a reunião
                                        return s;
                                    }
                                }
                            } else
                            {
                                // Informa ao usuário a sala registrada
                                Console.WriteLine("Sala " + s.numero);
                                //Retorna a sala para a reunião
                                return s;
                            }
                            
                        }
                    }
                    Console.WriteLine("Não há sala para esses dados!");
                }
                else
                {
                    // Informa que a data passada não é dia útil
                    Console.WriteLine("As reuniões devem ser agendadas em dias úteis!");
                }
            } else
            {
                // Informa que o agendamento foi feito com menos de 24 horas
                Console.WriteLine("A data para agendamento deve ser com no minimo 24 horas de antecedencia.");
            }
            // Passa 3 exemplos de agendamento para o usuário
            gerarExemplos(reuniao);

            return null;
        }

        /// <summary>
        /// Método Construtor da Classe
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="qtdPessoas"></param>
        /// <param name="internet"></param>
        /// <param name="tv_webcam"></param>
        public Reuniao(DateTime dataInicio, DateTime dataFim, int qtdPessoas, bool internet, bool tv_webcam)
        {
            this.dataInicio = dataInicio;
            this.dataFim = dataFim;
            this.qtdPessoas = qtdPessoas;
            this.internet = internet;
            this.tv_webcam = tv_webcam;
        }
    }
    #endregion
}
