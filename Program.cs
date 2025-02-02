﻿using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.ComponentModel.DataAnnotations;
using WebCrawlerIMDB.Controller;

namespace WebCrawlerIMDB
{
    class Program
    {        
        static void Main(string[] args)
        {
            Crawler wc = new Crawler();
            RPA rpa = new RPA();

            bool pub = false;
            string? user = null;
            string? pwd = null;

            // Cria um switch para alterar o nível do log de acordo com argumentos passados na execução. Por padrão o nível é Error
            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Error;

            #region Argumentos
            // Verifica se o argumento "-l" foi passado e define o próximo arumento como nível de log.
            // Se qualquer outra coisa for passada além dos 4 números espefificados, será definido o nível Error
            //int lPos = Array.IndexOf(args, "-l");
            if (Array.IndexOf(args, "-l") is int lPos && lPos > -1)
            {
                try
                {
                    string lvl = args[lPos + 1].Replace("\"", "");
                    switch (lvl)
                    {
                        case "0": levelSwitch.MinimumLevel = LogEventLevel.Debug; break;
                        case "1": levelSwitch.MinimumLevel = LogEventLevel.Information; break;
                        case "2": levelSwitch.MinimumLevel = LogEventLevel.Warning; break;
                        case "3": levelSwitch.MinimumLevel = LogEventLevel.Error; break;
                        default: Console.WriteLine("ERRO: \"{0}\" não é um valor válido para nível de log.\n(0 = Debug, 1= Info, 2 = Warning, 3 = Error)", args[lPos++]); Environment.Exit(0); break;
                    }
                }
                catch (IndexOutOfRangeException)
                { 
                    Console.WriteLine("ERRO: \"-l\" foi especificado mas nenhum parametro foi passado.\n(0 = Debug, 1= Info, 2 = Warning, 3 = Error)"); 
                    Environment.Exit(0); 
                }
            }

            //Inicia o log
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.ControlledBy(levelSwitch)
              .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message}{NewLine}{Exception}")
              .CreateLogger();

            // Se o argumento "-f" for especificado, será criado um arquivo txt com o log de execução
            if (args.Contains("-f"))
            {
                Log.Logger = new LoggerConfiguration()
               .MinimumLevel.ControlledBy(levelSwitch)
               .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message}{NewLine}{Exception}")
               .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day,
                   outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}")
               .CreateLogger();
            }
            
            // Se o argumento "-a" for especificado, o programa irá usar a lista pública do IMDB automaticamente
            // Sem pedir usuário e senha
            if (args.Contains("-a")) pub = true;

            // Se o parametro "-u" e "-p" for especificado, ele irá logar automaticamente no usuário inserido.
            // Uma verificação é feita se um email correto foi inserido e se uma senha foi especificada
            if (Array.IndexOf(args, "-u") is int uPos && uPos > -1)
            {
                try
                {
                    user = args[uPos + 1].Replace("\"", "");
                    if (!new EmailAddressAttribute().IsValid(user))
                    {
                        Log.Error("ERRO:\"{0}\" não é um email válido. Favor inserir um e-mail válido" +
                        "\nFavor especificar um usuário e senha válidos ou remova o parametro" +
                        "\nPara mais informações, busque a documentação", user);
                        Environment.Exit(0);
                    }
                    int pPos = Array.IndexOf(args, "-p");
                    if (pPos < 0) throw new Exception("Não foi especificado \"-p\" e uma senha. " +
                        "\nFavor inserir um usuário e senha válidos ou remova o parametro" +
                        "\nPara mais informações, busque a documentação");

                    pwd = args[pPos + 1].Substring(1, args[pPos + 1].Length - 2);

                }
                catch (IndexOutOfRangeException)
                {
                    Log.Error("Usuário e/ou senha não foram especificados." +
                        "\nFavor inserir um usuário e senha válidos ou remova o parametro" +
                        "\nPara mais informações, busque a documentação");
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    Environment.Exit(0);
                }
            }
            #endregion

            Log.Information("Iniciando WebCrawler");
            //Executa o WebCrawler
            try 
            {
                if (pub) wc.WebCrawler("https://www.imdb.com/chart/top/?ref_=nv_mv_250");
                else rpa.AutoUser(user, pwd);
            }
            catch (Exception ex) 
            {
                Log.Fatal("Erro ao executar o WebCrawler: " + ex.Message +
                "\nPara mais detalhes, verifique as entradas anteriores. " +
                "\n(Se necessário mudar o nível do log, favor olhar a documentação)"); 
            }
                       
            Log.CloseAndFlush();
        }        
    }
}
