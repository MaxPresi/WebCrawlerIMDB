# WebCrawlerIMDB

Aplicação WebCrawler criada para extrair dados específicos dos 20 melhores filmes avaliados pelo usuário ou da lista pública do IMDB e salva em um CSV
Compativel com Windows e Linux.

## Funcionalidade
### O programa pode funcionar de duas formas:
1. Lista Pública do IMDB  
	Uma lista dos 20 melhores filmes da lista geral do IMDB.  
	Esse resultado pode ser alcançado de duas formas:  
	- Ao abrir o programa, deixar o usuário em branco e apertar enter (como explicado na tela);  
	- Usando `-a` com parametro ao executar o software;  

2. Lita do Usuário  
	Essa lista pega até os 20 filmes mais bem avaliados do usuário.  
	Para chegar nesse resultado também existe duas formas:  
	- Preenchendo o Email do usuário e a Senha corretamente conform requisitado no programa.  
	- Usando os parametros `-u <email> -s <senha>`  


### Os dados específicos são:
1. Titulo do Filme
2. Ano de Lançamento do Filme
3. Diretor Principal do FIlme
4. Avaliaçã Média do Público
5. Número Estimado de Avaliaçõs que o Filme Possui
 

## Executando
1. Baixa a última versão disponível.  

2. Compile você mesmo
   - Clone o repositório ou baixe o zip.
   - Instale o [.Net8](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
   - Compile usando a IDE compatível de sua preferencia ou pelo terminal usando `dotnet build WebCrawlerIMDB.csproj`

### É necessário ter o Chrome instalado no sistema

 
## Argumentos
O programa aceita argumentos para agilizar o recebimento dos resultados

### Logs
- Mudar o nível mínimo de registro
  Usando `-l` mais um numero entre `0` e `3` você pode definir o nível mínimo do log. O nível minimo padrão é Error  
  0 = Debug  
  1 = Informational  
  2 = Warning  
  3 = Error  

- Salvar log em arquivo
Usando `-f` será criado um arquivo log.txt no mesmo diretório onde o programa estiver sendo executado

ex.: 
> WebCrawlerIMDB.exe -l 0  
> WebCrawlerIMDB.exe -f   
> WebCrawlerIMDB.exe -l 1 -f  


### Seleção de Lista
- Especificar como Lista Pública.  
Usando `-a` você especifica a lista como Lista Pública, pulando a interação inicial.  

- Especificar como Lista de Usuário
Aqui é necessário usar  `-u` e `-p` para especificar, respectivamente, Email e senha do usuário.  
Os dois devem ser preenchidos corretamente e obrigatóriamente

ex.: 
> WebCrawlerIMDB.exe -a  
> WebCrawlerIMDB.exe -u teste@exemplo.com -p Sup&rSenh@123   
> WebCrawlerIMDB.exe -a -l 1 -f  
> WebCrawlerIMDB.exe -u teste@exemplo.com -p Sup&rSenh@123 -l 0
 
## Sobre  
O software foi feito usando alguns pacotes disponíveis na internet. Sendo eles:
- Serilog: Biblioteca para Log de Informações
  - Console: Mostrar o Log no no Console
  - File: Salvar o Log para Arquivo
- Selenium WebDriver: Biblioteca principal para navegação e extração de dados de páginas da web
- ServiceStack Text: Serialização de objetos, usado para criar o CSV
