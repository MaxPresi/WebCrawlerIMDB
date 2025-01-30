using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebCrawlerIMDB.Model;
using Serilog;
using System.Text;
using ServiceStack.Text;
using System.IO;

namespace WebCrawlerIMDB.Controller
{
    public class Crawler
    {
        BrowserDriver wb = new BrowserDriver();

        /// <summary>
        /// Método que extrai os dados dos 20 filmes mais bem avaliados da lista especificada no link.
        /// </summary>
        /// <param name="link"></param>
        public void WebCrawler(string link)  
        {
            // Lista com os itens que serão extraídos de cada filme. Especificado na classe MovieItems.cs em Models.
            var movieItems = new List<MovieItems>();

            WebDriver driver = wb.GetDriver();

            var header = new MovieItems
            {
                Title = "Título",
                Year = "Ano",
                Director = "Diretor",
                ReviewsStars = "Nota",
                ReviewsCount = "Avaliações"
            };
            movieItems.Add(header);

            // Verifica se o arquivo csv que será criado está aberto
            Log.Debug("Verificando se o arquivo está aberto CSV.");
            var fi1 = new FileInfo(@"IMDB.csv");
            while (Aberto(fi1) == true)
            {
                Console.WriteLine("\nArquivo IMDB.csv está em uso!");
                Console.WriteLine("Favor fechar o programa que esteja usando IMDB.csv e depois aperte Enter");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            }

            // Especfifica a página que será usada. O método retorna o link dependendo se um usuário foi especificado ou não.
            Log.Debug("Carregando página {0}", link);
            driver.Navigate().GoToUrl(link);
            Log.Information("Página carregada!");

            if (link.Contains("user") || !wb.ElementAre(By.ClassName("ipc-title__text")))
            {
                Log.Error("Usuário sem avaliaçoes. Favor selecionar outro usuário.");
                return;
            }

            // Procura e clica no botão que altera a lista para uma visão detalhada, mostrando mais informações sem precisar abrir o link de cada filme
            driver.FindElement(By.Id("list-view-option-detailed")).Click();
            Log.Debug("Exibição alterada para lista detalhada.");

            // Filtro feito pela classe dos itens presentes na lista requerida
            var movieElements = driver.FindElements(By.ClassName("ipc-metadata-list-summary-item"));
            Log.Information("Seleção por Classe efetuada. Total de Itens: {0}", movieElements.Count);

            // Aqui limitei a lista em 20, conforme exigido
            

            foreach (var movieElement in movieElements.Take(20))
            {
                // Filtros feitos pelas classes de cada item
                var TitleElement = movieElement.FindElement(By.ClassName("ipc-title__text"));
                var YearElement = movieElement.FindElement(By.ClassName("dli-title-metadata-item"));
                var DirectorElement = movieElement.FindElement(By.ClassName("dli-director-item"));
                var ReviewStarElement = movieElement.FindElement(By.ClassName("ipc-rating-star--rating"));
                var ReviewCountElement = movieElement.FindElement(By.ClassName("ipc-rating-star--voteCount"));

                // Extraindo os textos de cada item. 
                var title = TitleElement.Text.Substring(TitleElement.Text.IndexOf(' ') + 1); // Substring para remover a numeração
                var year = YearElement.Text;
                var director = DirectorElement.Text;
                var reviewstar = ReviewStarElement.Text;
                var reviewcount = ReviewCountElement.Text.Replace(" (", "").Replace(")",""); // Replace para remover caracteres desnecessários

                // Criando objeto para manipulação.
                var movieItem = new MovieItems 
                { 
                    Title = title,
                    Year = year,
                    Director = director,
                    ReviewsStars = reviewstar,
                    ReviewsCount = reviewcount
                };
                Log.Debug($"Filme: {movieItem.Title}, Ano: {movieItem.Year}, Diretor: {movieItem.Director}, Nota: {movieItem.ReviewsStars}/10, Avaliações: {movieItem.ReviewsCount}");
                movieItems.Add(movieItem);
            }

            wb.StopDriver();           

            // Criação do CSV
            Log.Information("Criando CSV");
            CsvConfig.ItemSeperatorString = ";"; // troca o seperador para evitar confusão com as reviews
            CsvConfig<MovieItems>.OmitHeaders = true;  // omite os cabeçalhos automáticos
            string csv = CsvSerializer.SerializeToCsv(movieItems); //converte a lista de objetos em csv
            Log.Debug("CSV Criado e pronto para ser salvo");
            
            File.WriteAllText("IMDB.csv", csv.ToString(), Encoding.UTF8); //cria o arquivo csv
            Log.Information("CSV salvo com sucesso");

            Log.Information("Tarefa completada com sucesso!");
            Log.CloseAndFlush();
            Environment.Exit(0);
        }

        /// <summary>
        /// Verifica se o arquivo existe e se está aberto.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Aberto(FileInfo file)
        {
            FileStream? stream = null;

            if (file.Exists)
            {
                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    return true;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

    }
}
