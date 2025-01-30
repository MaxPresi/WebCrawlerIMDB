using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Serilog;

namespace WebCrawlerIMDB.Controller
{
    
    public class BrowserDriver
    {
        public WebDriver driver;

        /// <summary>
        /// Inicia o Chrome WebDrive com os argumentos definidos
        /// </summary>
        /// <returns></returns>
        public WebDriver GetDriver()
        {
            if (driver == null)
            {
                Log.Debug("Configurando Chrome WebDriver");
                // Passa argumentos para execução do Chrome
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments(@"--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36\"); //Especifica o user agent, foi necessário pois em headless mode estava dando erro 403
                chromeOptions.AddArguments("--headless=new");
                chromeOptions.AddArguments("--ignore-certificate-errors");
                chromeOptions.AddArguments("--ignore-ssl-errors");
                chromeOptions.AddArguments("--log-level=3");
                chromeOptions.AddArguments("--incognito");
                Log.Information("Executando Chrome WebDriver com os Argumentos: \n {0}", chromeOptions.Arguments);

                driver = new ChromeDriver(chromeOptions);
                Log.Debug("WebDriver criado com sucesso");
                return driver;
            }
            return driver;
        }

        /// <summary>
        /// Verifica se o elemento existe na página.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public bool ElementAre(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Finaliza o WebDrive
        /// </summary>
        /// <returns></returns>
        public void StopDriver()
        {
            Log.Information("Parando o WebDriver.");
            if (driver != null) driver.Quit(); // Fechar o WebDriver
        }
                
    }
}
