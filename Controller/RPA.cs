using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebCrawlerIMDB.Controller
{
    public class RPA
    {
        BrowserDriver wb = new BrowserDriver();        
        Crawler wc = new Crawler();
        private IWebDriver driver;
        /// <summary>
        /// Método que automatiza o login com os parametros fornecidos.
        /// Depois passa o link para o Crawler para extração dos dados
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        public void AutoUser(string? user, string? pwd)
        {
            bool sucess = false;
            bool usuario = false;
            bool senha = false;

            // Verifica se algum usuário foi passado via parametros ao executar
            // Caso não, pede para o usuário especificar o email e senha, ou se deseja usar a Lista Geral Públic do IMDB
            Console.WriteLine("\n" + @"==//== IMDB WebCrawler ==\\==" + "\n");
            if (user == null)
            {
                do
                {
                    do
                    {
                        Console.WriteLine("\nDigite o Email de Usuário e aperte Enter\n(Ou deixe vazio para usar a Lista Geral):");
                        user = Console.ReadLine();

                        if (user == string.Empty)
                        {
                            wc.WebCrawler("https://www.imdb.com/chart/top/?ref_=nv_mv_250");
                            return;
                        }
                        if (user != string.Empty && new EmailAddressAttribute().IsValid(user))
                        {
                            usuario = true;

                            do
                            {
                                Console.WriteLine("\n\nDigite a Senha do Usuário:");
                                pwd = Password();

                                if (pwd.Length < 7)
                                {
                                    Log.Error("A senha não pode estar em branco ou ser menor que 8 caracteres. Favor tentar novamente...");
                                }
                                else senha = true;

                            } while (!senha);
                        }
                        else
                        {
                            Log.Error("\"{0}\" não é um email válido. Favor tentar novamente...", user);
                        }
                    } while (!usuario);

                    sucess = AutoLogin(user, pwd);

                } while (!sucess);
            }

            else AutoLogin(user, pwd);

            wc.WebCrawler("https://www.imdb.com/list/ratings/?ref_=nv_usr_rt_4");
        }

        /// <summary>
        /// Método que automatiza o login.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private bool AutoLogin(string user, string pwd)
        {
            driver = BrowserDriver.GetDriver();
            try
            {
                Log.Debug("Carregando página de Login...");
                // Especfifica a página que será usada.
                // Não vi necessidade de implementar waiters já que os dados não vêm de elementos dinamicos.
                driver.Navigate().GoToUrl("https://www.imdb.com/ap/signin?openid.pape.max_auth_age=0&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.assoc_handle=imdb_us&openid.mode=checkid_setup&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0");
                Log.Debug("Página carregada!");

                // Insere os dados fornecidos pelo usuário na tela de login
                Log.Debug("Inserindo dados de login");
                driver.FindElement(By.Name("email")).Clear();
                driver.FindElement(By.Name("email")).SendKeys(user);
                driver.FindElement(By.Name("password")).Clear();
                driver.FindElement(By.Name("password")).SendKeys(pwd);
                driver.FindElement(By.Id("signInSubmit")).Click();
                Log.Debug("Tentando Logar...");

                // Detecta se houve erro no login e passa a mensagem de erro da página diretamente para o usuário                
                if (wb.ElementAre(By.Id("auth-error-message-box")))
                {
                    Log.Error("Falha ao Tentar Logar! \n" + driver.FindElement(By.ClassName("a-list-item")).Text);

                    ConsoleKey response;
                    do
                    {
                        Console.WriteLine("Tentar novamente? [s/n]");
                        response = Console.ReadKey(false).Key;
                        if (response != ConsoleKey.Enter)
                            Console.WriteLine();

                    } while (response != ConsoleKey.S && response != ConsoleKey.Y && response != ConsoleKey.N);

                    if (response == ConsoleKey.S || response == ConsoleKey.Y) return false;
                    else Environment.Exit(0);
                }

                // Verifica se o o menu de usuário existe, existindo, é um ótimo sinal que o login foi feito :)
                if (wb.ElementAre(By.ClassName("nav__userMenu")))
                {
                    Log.Information("Login efetuado com sucesso!");
                    return true;
                }

                // Após repetidas tentativas incorretas de login com o mesmo usuário,
                // a página irá pedir captcha para seguir com o login. 
                if (wb.ElementAre(By.ClassName("cvf-captcha-img")))
                {
                    Log.Error("Login incorreto por repetidas vezes." +
                        "\nFavor verifique se o usuário está correto ou tente outro usuário");
                    return false;
                }

                Log.Error("Login não efetuado por motivo desconhecido." +
                        "\nFavor verifique se o usuário está correto ou tente outro usuário");
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }




        /// <summary>
        /// Máscara simples de senha
        /// </summary>
        /// <returns></returns>
        private static string Password()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }


    }
}
