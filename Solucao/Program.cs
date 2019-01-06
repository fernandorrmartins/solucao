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
        /// Atributo por Guardar Lista das Reuniões Salvas ao Carregar ou Registrar uma nova Reunião
        /// </summary>
        public List<Reuniao> reunioes = null;
        #endregion

        /// <summary>
        /// Método Contrutor da Classe
        /// Responsável por Carregar As Reuniões já Registradas Assim que o Programa Inicializa
        /// </summary>
        public GerenciadorJson ()
        {
            // Carrega as Reuniões Salvas para Apresentar quando requisitado
            carregarReunioesSalvas();
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
                    File.Create(arquivoDeReunioes);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(arquivoDeReunioes))
                    {
                        /*
                         *Parte que carrega o arquivo .json responsável por guardar
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
            } catch (Exception e)
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
    /// Classe Responsável por Guardar, Salvar e Apresentar Registros de Reuniões
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
        #endregion

        /// <summary>
        /// Método Construtor da Classe
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="qtdPessoas"></param>
        /// <param name="internet"></param>
        /// <param name="tv_webcam"></param>
        public Reuniao (DateTime dataInicio, DateTime dataFim, int qtdPessoas, bool internet, bool tv_webcam)
        {
            this.dataInicio = dataInicio;
            this.dataFim = dataFim;
            this.qtdPessoas = qtdPessoas;
            this.internet = internet;
            this.tv_webcam = tv_webcam;
        }
    }

    /// <summary>
    /// Classe Responsável por Gerenciar o Menu e suas Opções
    /// </summary>
    class Menu
    {
        GerenciadorJson gerenciadorJson = new GerenciadorJson();

        public Menu()
        {
            carregarReuniaoASerRegistrada();
        }
        
        public void carregarReuniaoASerRegistrada()
        {
            
        }
        
        /*
         * Código com menu que estava sendo desenvolvido, mas não era necessário então apenas comentei
         * 
        String cmd;
        StringBuilder opcoesMenu = new StringBuilder();
        public Menu()
        {
            opcoesMenu.Append("############################################# Menu Principal #################################################\n");
            opcoesMenu.Append("#                                                                                                            #\n");
            opcoesMenu.Append("# (1) Agendar reunião(O arquivo .json com a reunião deve estar na mesma pasta do executavel)                 #\n");
            opcoesMenu.Append("# (2) Ver reuniões agendadas                                                                                 #\n");
            opcoesMenu.Append("# (0) Sair                                                                                                   #\n");
            opcoesMenu.Append("#                                                                                                            #\n");
            opcoesMenu.Append("##############################################################################################################\nDigite sua opção: ");
            apresentarMenu();
        }

        public void apresentarMenu()
        {
            Console.Write(opcoesMenu);
            cmd = Console.ReadLine();

            // Condição para registrar uma nova reunião
            if (cmd.Equals("1"))
            {

            }
            // Condições para listar as reuniões registradas
            else if (cmd.Equals("2"))
            {
                // Conidção para Listar as Reuniões
                if (gerenciadorJson.reunioes.Count > 0)
                {
                    Console.Clear();
                    StringBuilder listarReunioes = new StringBuilder();
                    foreach(Reuniao reuniao in gerenciadorJson.reunioes)
                    {
                        listarReunioes.Append(reuniao.);
                    }
                }
                /*
                 * Condição se não houverem reuniões registradas, apresenta uma mensagem
                 * informando ao usuário e retorna ao menu principal, após o usuário
                 * pressionar uma tecla
                else
                {
                    StringBuilder mensagem = new StringBuilder();
                    Console.Clear();
                    mensagem.Append("Não há reuniões registradas!\n");
                    mensagem.Append("Pressione qualquer tecla para voltar ao menu principal...");
                    Console.WriteLine(mensagem);
                    Console.ReadKey();
                    apresentarMenu();
                }
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
        */
    }
}
