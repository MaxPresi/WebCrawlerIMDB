# WebCrawlerIMDB

Aplicação WebCrawler criada para extrair dados específicos dos 20 melhores filmes seguindo a lista pública do IMDB e salva em um CSV

### Os dados específicos são:
1. Titulo do Filme
2. Ano de Lançamento do Filme
3. Diretor Principal do FIlme
4. Avaliaçã Média do Público
5. Número Estimado de Avaliaçõs que o Filme Possui

Para executar o programa, basta baixar e compilar a solução, ou baixar a última versão.

## Argumentos
O programa aceita argumentos para especificar configurações de log.

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


## Sobre  
O software foi feito usando alguns pacotes disponíveis na internet. Sendo eles:
- Serilog: Biblioteca para Log de Informações
  - Console: Mostrar o Log no no Console
  - File: Salvar o Log para Arquivo
- Selenium WebDriver: Biblioteca principal para navegação e extração de dados de páginas da web
- ServiceStack Text: Serialização de objetos, usado para criar o CSV
