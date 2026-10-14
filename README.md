# blazor-dbz-api-personagem
homework

## O que foi solicitado

| Item do PDF | Solicitação | Implementação / situação |
| --- | --- | --- |
| a | Pedir o ID de um personagem e consumir o endpoint Dragon Ball | Formulário na página inicial, com consulta dinâmica pelo ID informado |
| b | Mostrar Nome, Planeta e Transformações | Nome, planeta de origem e lista de todas as transformações recebidas |
| c | Publicar no GitHub em um repositório chamado blazor-dbz-api-personagem | Será feito manualmente pelo aluno; nenhum repositório foi criado ou publicado |
| d | Entregar no Google Classroom | Será feito manualmente pelo aluno, conforme informado |

O projeto usa o nome sugerido **BlazorDbzPersonagem** e mantém o layout Blazor
semelhante à imagem da segunda página. Counter e Weather são exemplos do
template preservados no menu; não fazem parte da consulta Dragon Ball.
Weather utiliza dados locais de demonstração.

## Como executar

Pré-requisitos: SDK **.NET 10**, navegador com WebAssembly e internet para
acessar a API. O SDK utilizado foi `10.0.400`.

No PowerShell:

```powershell
Set-Location 'C:\Users\lucas\OneDrive\Documentos\vs code uni\homework-blazordbz'
dotnet restore BlazorDbzPersonagem.csproj
dotnet run --project BlazorDbzPersonagem.csproj
```

Abra **http://localhost:5202**. Informe um ID, como **1** ou **2**, e clique em
**Consultar** ou pressione **Enter**. A aplicação não faz consulta automática
nem escolhe um personagem fixo: a requisição depende do ID digitado.

O primeiro carregamento pode demorar alguns segundos enquanto o navegador
carrega o runtime WebAssembly. Não abra `wwwroot/index.html` diretamente;
utilize o servidor iniciado por `dotnet run`. Encerre com `Ctrl+C`.

Para compilar:

```powershell
dotnet build BlazorDbzPersonagem.csproj
```

Para trabalhar com atualização durante a edição:

```powershell
dotnet watch --project BlazorDbzPersonagem.csproj run
```

O fluxo verificado nesta entrega foi `dotnet run`. No VS Code, abra a raiz
com `code .`. Em **Terminal > Run Task**, há tarefas para compilar e executar
o projeto, além de recomendações das extensões C# e C# Dev Kit.

## Estrutura

```text
homework-blazordbz/
├── BlazorDbzPersonagem.csproj
├── Program.cs
├── App.razor
├── _Imports.razor
├── Models/
│   ├── Personagem.cs
│   ├── Planeta.cs
│   └── Transformacao.cs
├── Pages/
│   ├── Home.razor
│   ├── Home.razor.css
│   ├── Counter.razor
│   └── Weather.razor
├── Layout/                        # Menu e estrutura visual
├── Properties/launchSettings.json # Perfil local, porta 5202
├── wwwroot/                       # HTML, CSS e Bootstrap local
├── tests/BlazorDbzPersonagem.Tests/
│   ├── BlazorDbzPersonagem.Tests.csproj
│   └── HomeTests.cs
├── .vscode/                       # Tarefas e extensões
├── .gitignore
└── README.md
```

## Como os códigos funcionam

### Inicialização e interatividade

`Program.cs` inicia o host WebAssembly e registra o `HttpClient` com timeout
de **15 segundos**. `App.razor` resolve as rotas e `MainLayout.razor` organiza
o menu e o conteúdo. O Bootstrap está incluído localmente.

Esta é uma aplicação **WebAssembly standalone**: o C# roda no navegador e a
consulta HTTPS é enviada diretamente à API. Não é necessário usar
`@rendermode InteractiveServer`, pois não se trata de Blazor Server.
Não há backend próprio, autenticação, banco de dados ou chave de API.

### Modelos e JSON

Os atributos `JsonPropertyName` associam os campos JSON às propriedades C#:

| JSON | Modelo / propriedade |
| --- | --- |
| id | Personagem.Id |
| name | Personagem.Nome |
| originPlanet | Personagem.PlanetaOrigem, do tipo Planeta |
| originPlanet.name | Planeta.Nome |
| transformations | Personagem.Transformacoes, lista de Transformacao |
| transformations[].name | Transformacao.Nome |

A resposta é um objeto de personagem com objetos aninhados, não uma lista
simples. Os modelos representam essa estrutura. Campos adicionais, que não
são necessários para os itens pedidos, são ignorados na desserialização.

### Consulta na Home

`Home.razor` contém o formulário, os estados da interface e o método
`ConsultarPersonagem`:

1. Limpa mensagens e resultados anteriores.
2. Valida o ID com `int.TryParse`: deve ser um inteiro positivo, sem letras,
   sinal negativo ou parte decimal. Espaços nas extremidades são removidos.
3. Monta `https://dragonball-api.com/api/characters/{id}`.
4. Aguarda `Http.GetFromJsonAsync<Personagem>`, que consulta e converte o JSON.
5. Verifica se existe uma resposta com nome e exibe os dados retornados.
6. Libera o formulário em `finally`, inclusive quando ocorre um erro.

O campo usa `@bind:event="oninput"`, e `LimparResultado` remove os dados
antigos ao editar o ID. O envio do formulário usa `@onsubmit` com prevenção
do envio HTML tradicional, permitindo consultar também pelo Enter.

Durante a consulta, o campo e o botão ficam desabilitados para evitar
requisições simultâneas. Sair da página cancela a requisição pendente por
meio de `CancellationTokenSource` e `IDisposable`.

### Resultados e falhas

Nome e planeta são apresentados em parágrafos, e todas as transformações
recebidas são exibidas em uma lista. Os resultados vêm da API, sem nomes,
planetas ou transformações fixados no código da aplicação.

- Planeta ausente: **Não informado pela API**.
- Lista de transformações vazia ou ausente: mensagem indicando que não há
  transformações cadastradas.
- ID vazio ou inválido: mensagem de validação, sem fazer requisição.
- HTTP **400 ou 404**: personagem não encontrado. Na verificação real, o
  endpoint respondeu 400 com `Character ID not found` para um ID inexistente.
- Falhas HTTP ou de conexão: mensagem solicitando uma nova tentativa.
- Timeout: aviso de que a consulta demorou demais.
- JSON inválido ou resposta sem nome: mensagem de resposta inválida.

**Sobre o exemplo do PDF:** na verificação realizada, o ID 1 retornou
**Goku**, planeta **Tierra** e seis transformações. A imagem do material
mostra outro planeta. O programa preserva o valor recebido do serviço;
não o substitui por conhecimento sobre o anime nem pela captura do PDF.
Os dados da API podem mudar.

## Verificações

Foram consultados no navegador os IDs **1 (Goku)** e **2 (Vegeta)**, mostrando
nome, planeta e transformações. Também foram conferidos envio por Enter,
campo vazio, ID decimal e resposta para um ID inexistente.

O projeto inclui **19 testes automatizados** com xUnit e bUnit:

- validação de entradas sem chamada HTTP;
- uso do ID informado na URL;
- exibição de nome, planeta e lista de transformações;
- limpeza do resultado após editar;
- ausência de dados opcionais;
- resposta nula, incompleta ou JSON inválido;
- HTTP 400, 404 e 503, falha de rede e timeout;
- bloqueio e liberação dos controles durante a consulta.

Execute:

```powershell
dotnet test .\tests\BlazorDbzPersonagem.Tests\BlazorDbzPersonagem.Tests.csproj
```

Os testes usam respostas simuladas e não dependem da API pública. Os dados
fixos usados neles são exclusivos dos testes. A pasta `tests/**` está excluída
dos itens do projeto principal, portanto xUnit e bUnit não são enviados ao
navegador com a aplicação.

## Problemas comuns

- **Porta ocupada:** encerre a execução anterior ou utilize
  `dotnet run --urls http://localhost:5203`.
- **Falha na restauração:** confira o acesso ao NuGet e repita `dotnet restore`.
- **Falha na consulta:** confira a internet e a disponibilidade da API.
- **Tela desatualizada após alterar código:** compile novamente e recarregue
  o navegador, ou utilize `dotnet watch`.

