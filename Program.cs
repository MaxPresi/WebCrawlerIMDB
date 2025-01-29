using Serilog;
using Serilog.Core;
using Serilog.Events;
using WebCrawlerIMDB.Controller;

namespace WebCrawlerIMDB
{
    class Program
    {        
        static void Main(string[] args)
        {
            Crawler wc = new Crawler();

            // Cria um switch para alterar o nível do log de acordo com argumentos passados na execução. Por padrão o nível é Error
            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Debug;

            // Verifica se o argumento "-l" foi passado e define o próximo arumento como nível de log.
            // Se qualquer outra coisa for passada além dos 4 números espefificados, será definido o nível Error
            int lPos = Array.IndexOf(args, "-l");
            if (lPos > -1)
            {
                switch (args[lPos++])
                {
                    case "0": levelSwitch.MinimumLevel = LogEventLevel.Debug; break;
                    case "1": levelSwitch.MinimumLevel = LogEventLevel.Information; break;
                    case "2": levelSwitch.MinimumLevel = LogEventLevel.Warning; break;
                    default: levelSwitch.MinimumLevel = LogEventLevel.Error; break;
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

            Log.Information("Iniciando WebCrawler");


            

            //Executa o WebCrawler
            try { wc.WebCrawler(); }
            catch (Exception ex) 
            {
                Log.Fatal("Erro ao executar o WebCrawler: " + ex.Message +
                "\nPara mais detalhes, verifique as entradas anteriores. " +
                "\n(Se necessário mudar o nível do log, favor olhar a documentação)"); 
            }

            Log.Information("Tarefa completada com sucesso!");
            Log.CloseAndFlush();
        }        
    }
}
