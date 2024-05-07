# Acerto - Desafio Técnico Back-end

 ## Projeto

 ### Objetivos

- [x] Criar api de produtos e permitir CRUD de produtos
- [x] Criar api de pedidos, registrar e processar pedidos assincronamente usando fila de mensagens no RabbitMQ
- [x] Criar api de autenticação e proteger os endpoints das apis de produto e pedidos usando JWT
- [x] Usar Entity Framework para persistência de dados
- [x] Validar dados de entrada das apis
- [x] Criação de SDKs cliente para apis usando biblioteca Refit
- [x] Escrever testes unitários para validação dos endpoints principais usando xUnit
- [x] Usar swagger para documentação da api - swagger UI disponível para visualização
- [x] Utilização da ultima versão C#, NET 8.0
- [x] Uso frequente de abstrações, polimorfismo e generics pra um código mais flexivel e reutilizavel
- [x] Implementação de caching usando Redis
- [x] Uso de containers docker - docker-compose disponível.
- [x] Readme contendo descrição, instrução de configuração e uso, com exemplos de chamadas de api.
- [x] Código disponível no github em repositório publico

 ### Observações

 Esse se trata de um projeto de demonstração, e portanto nao pretende atacar problemas grandes que soluções complexas e modernas exigem. Portanto certas implementações nesse projeto aparecem de forma simplificada, para que seja possível demonstrar o uso de ferramentas e soluções de software dentro do prazo proposto. Toda problema e sua solução precisa ser pensada de forma criterioza, analisando todas as circustâncias, ferramentas, casos extremosa etc. Por exemplo, ao utilizar proxyies no EF core e por consequencia listar pedidos com vários items, caimos no problema n+1 o que em produção seria bastante crítico. Também posso citar o fato do do consumidor estar propositalmente configurado para processar 1 mensagem por vêz do RabbitMQ, o que na vida real, a solução poderia ser mto mais complexa, indepotente, e possivelmente utilizando transações distribuídas, escalável. etc.. 

 ### Escopo - Serviços
 A solução contém 3 serviços principais conforme proposto no desafio. Todos os serviços utilizam banco de dados postgreSQL + EntityFrameworkCore para persistência de dados. Foram definidas entidades simples, somente para demonstração do desafio proposto, sem regras de negócio elaboradas, contando com o essencial em termos de validação de dados.

 #### Acerto.Auth.Service
 Serviço responsável pela gestão de usuários e geração de JWT para authenticação de usuario, e também dos outros serviços clientes (Produtos e Pedidos). Neste serviço foi utilizado o Asp.NET Identity Model e criado um controller simples somente para registro e autenticação de usuários, portanto sem recursos de autorização para ações específicas dos serviços clientes.

 #### Acerto.Products.Service
 Serviço para CRUD de produtos, que utiliza Redis para manter as entidades produto disponível em cache. Uma solução simples foi criada com base nos requisitos do desafio, sempre que um produto é criado, atualizado ou excluído, essa ação é imediatamente refletida no estado dessa entidade no cache.

 #### Acerto.Orders.Service
 Serviço para gestão de Pedidos. Ao receber e validar os dados de requisição, este serviço nao publica o pedido em um tópico do RabbitMQ que possui um binding para a fila que possui o nome do próprio serviço. Foi utilizado o framework Rebus que deixa mais elegante e simples a implementação. Ao receber um pedido válido, uma resposta imediata é dada a requisição, e esse pedido é processado posteriormente. Criei uma máquina de estados, que simula o processamento assincrono do pedido e mantem seu status atualizado. Para simplificar, sempre que um erro acontecer, o pedido será cancelado imediatamente e o erro constará no status. Um erro neste momento pode acontecer por diversos motivos, inclusive podendo ser erros transientes. Um tratamento para erro transiente foi adicioado, porém somente uma nova tentativa será feita e caso o erro persista, este constará no status do pedido.

 Ao processar o pedido e obter dados dos produtos que constam neste pedido, este serviço também tentará primeiro obter os dados do produro do cache Redis e só entao, caso nao disponível, este fará uma chamada à API de produtos.

 ### SDK's
 3 outros projetos (Acerto.Auth.SDK; Acerto.Orders.SDK e Acerto.Products.SDK), que poderiam ser distribuidos como pacotes Nuget por exemplo, foram criados para implementar o acesso as APIs (autenticação, produtos e pedidos). A SDK facilita o acesso as APIs, uma vez que também cuida da parte de incluir o JWT em cada requisição etc. Foram implementações simples, que viram ilustrar e facilitar testes etc.

 ### Testes
 #### Testes unitários
 O serviço de produtos conta com testes unitários, que visam testar o controller de Produtos, validar os retornos de dados em diferentes situações (dados válidos, inválidos). Utilizei banco de dados em memoria (Sqlite) para mockar dependências do controller e portanto validar retornos de forma assertiva.
 #### Console - Acerto.Testing.APp
 Uma aplicação de teste/debug foi criada para faciliar o uso e compreensão do que foi feito. Essa aplicação utiliza as SDK's criadas, e durante a execução cuida de autenticar e ou/criar um novo usuário e criar diversos produtos, pedidos e acompanhar o status dos pedidos até que estes sejam concluídos (máquina de estados)

 ## Pré requisitos

### Para compilar o projeto localmente
 - .NET SDK 8.0 (dotnet CLI) e/ou Visual Studio 17.8 ou outra IDE compatível.

### Para rodar o projeto localmente
 - Docker (docker-compose.yml disponível no reposítório)

 #### docker-compose
 O arquivo docker-compose.yml possui os seguintes serviços, que rodão nas seguintes portas informadas abaixo, todas as portas dos serviços serão expostas no computador host, portanto certifique-se que nao terá nenhum bloqueio em relação a isso.
 - PostgresSQL - Porta **5432**
 - PgAdmin - Porta **54321**
 - Redis - Porta **6379**
 - RabbitMQ - Porta **5672** e UI na porta **15672**
 - API Autenticação - Portas **HTTP:50010** e **HTTP:50011**
 - API Produtos - Portas **HTTP:50020** e **HTTP:50021**
 - API Pedidos - Portas **HTTP:50030** e **HTTP:50031**

 #### Dados de acesso

 - PostgresSQL > Usuário: **acerto** Senha: **acerto**
 - PgAdmin > Usuário: **acerto@acerto.com.br** Senha: **acerto**
 - Redis > Usuário: **default**; Senha: **redis2024!**
 - RabbitMQ > Usuário: **acerto** Senha: **acerto**
 - API Autenticação - Usuário precisa ser registrado para dar acesso aos outros serviços.

 ## Rodando o projeto

 - ``git clone https://github.com/wallacepca/acerto.git``
 - Abra o terminal no diretório que acabamos de clonar (acerto), vamos rodar os testes unitário do serviço de produtos usando o seguinte comando: ``dotnet test .\test\Acerto.Products.Service.Tests\Acerto.Products.Service.Tests.csproj``. Confirme que o projeto irá restaurar todas as dependências, e após a compilação, todos os 17 testes deverám passar.
 - agora é hora de rodar o projeto, ainda com o terminal aberto no diretório raiz do git, vamos rodar o projeto usando o ``docker-compose``. Rode o comando ``docker-compose u  p``, certifique-se que você está no mesmo diretório onde ser encontra o arquivo docker-compose.yaml e que todas as portas mencionadas acima estejam disponíveis no computador host. Após essa etapa vamos aguardar que todos o serviços estejam rodando, para isso é possivel verificar o status usando docker CLI (comando: ``docker ps``), docker desktop, ou qualquer outro cliente docker. Durante esse período as imagens serão baixadas, geradas e após algum tempo todos os serviços deverão estar rodando. Obs: As imagens das APIs foram otimizadas para utilizarem um menor tamanho em comparação a configuração padrão de ``Dockerfile`` disponibilizada no template do Visual Studio.
 - Não é preciso se preocupar com migração e ou configuração de banco de dados, pois todos os arquivo de configuração estão configurados perfeitamente para rodar no ambiente docker e tbm no ambiente de desenvolvimento, com configurações individualizadas. A migração de banco de dados será executada automaticamente quando o projeto rodar. É normal que aconteca alguns erros durante a inicialização tendo em vista que nenhuma ferramenta externa de espera pela disponibilidade de projetos dependentes está sendo usado, e a simples configuração de dependência no arquivo docker-compose.yml nem sempre é suficiente.

 ### Health checks endpoints
 Quando todos os serviços estiverem rodando, podemos mais uma vez confirmar que todas as dependências se encontram com status saudável utilizando os seguintes endpoints the health check para cada API. 
 - [API Autenticação - Health check](http://localhost:50010/healthz)
 - [API Produtos - Health check](http://localhost:50020/healthz)
 - [API Pedidos - Health check](http://localhost:50030/healthz)

 ### Documentação Swagger
 - [API Autenticação - Swagger Docs](http://localhost:50010/swagger)
 - [API Produtos - Swagger Docs](http://localhost:50020/swagger)
 - [API Pedidos - Swagger Docs](http://localhost:50030/swagger)

 ## Passo a passo

 Vamos agora iniciar os testes funcionais. Para tal pode-se usar o cliente de preferência, tal como Postman ou rodar os comandos curl que serão disponibilidades abaixo juntamente com os json.

 ### 1 - Criação de usuário - POST - http://localhost:50010/auth/register
 #### Usuário com dados inválidos - Bad Request

 request body:
 ``{
  "email": "acerto@acerto.com.br",
  "password": "acertoacerto",
  "confirmPassword": "erroerro"
}``

curl:
``curl -X 'POST' \
  'http://localhost:50010/Auth/register' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "acerto@acerto.com.br",
  "password": "acertoacerto",
  "confirmPassword": "erroerro"
}'``


 #### Usuário com dados válidos

 request body:

 ``{
  "email": "acerto@acerto.com.br",
  "password": "Acerto@2024",
  "confirmPassword": "Acerto@2024"
}``

curl:

``curl -X 'POST' \
  'http://localhost:50010/Auth/register' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "acerto@acerto.com.br",
  "password": "Acerto@2024",
  "confirmPassword": "Acerto@2024"
}'``

 ### 2 - Login de usuário - POST - http://localhost:50010/auth/login
 #### Tentativa de acesso com dados inválidos - Bad Request

 request body:

 ``{
  "email": "acerto@acerto.com.br",
  "password": "erroerro"
}``

curl:

``curl -X 'POST' \
  'http://localhost:50010/Auth/login' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "acerto@acerto.com.br",
  "password": "erroerro"
}'``

 #### Tentativa de acesso com dados válidos - usuario criando com sucesso anteriormente - JWT gerado.

 request body:

 ``{
  "email": "acerto@acerto.com.br",
  "password": "Acerto@2024"
}``

curl:

``curl -X 'POST' \
  'http://localhost:50010/Auth/login' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "acerto@acerto.com.br",
  "password": "Acerto@2024"
}'``

 ### ATENÇÃO - Incluir accessToken no cabeçalho da requisição - Tipo: Authorization Bearer - caso contrário API's retornarão erro 401 - Não autorizado.

 ### 3 - Criação de produto - POST - http://localhost:50020/products

 #### Dados Inválidos - Preço deve ser maior que 0
 #### API retornará um objeto de erro completo do framework de validação - Não é o ideal, e nao o faria em produção, porém não tratei como prioridade neste projeto.

 payload:

 ``{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Samsung Galaxy S24 Ultra",
  "brand": "Samsung",
  "price": 0,
  "description": null,
  "color": null
}``

curl: (*substituir accessToken*)

``curl -X 'POST' \
  'http://localhost:50020/Products' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Samsung Galaxy S24 Ultra",
  "brand": "Samsung",
  "price": 0,
  "description": null,
  "color": null
}'``

 #### Dados Válidos

 payload 1:

 ``{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Samsung Galaxy S24 Ultra",
  "brand": "Samsung",
  "price": 5000,
  "description": "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur",
  "color": "Preto"
}``

payload 2:

``{
  "id": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
  "name": "Iphone 15 Pro Max",
  "brand": "Apple",
  "price": 10000,
  "description": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
  "color": "Branco"
}``

 ### 4 - Atualizar produto - PUT - http://localhost:50020/products

 #### Dados Inválidos

 #### Dados Válidos - Atualizando Preço

  payload 1:

 ``{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Samsung Galaxy S24 Ultra",
  "brand": "Samsung",
  "price": 4599,
  "description": "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur",
  "color": "Preto"
}``

curl 1:

``url -X 'POST' \
  'http://localhost:50020/Products' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Samsung Galaxy S24 Ultra",
  "brand": "Samsung",
  "price": 4599,
  "description": "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur",
  "color": "Preto"
}'``


payload 2:

``{
  "id": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
  "name": "Iphone 15 Pro Max",
  "brand": "Apple",
  "price": 7999,
  "description": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
  "color": "Branco"
}``

curl 2:

``curl -X 'POST' \
  'http://localhost:50020/Products' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
  "name": "Iphone 15 Pro Max",
  "brand": "Apple",
  "price": 7999,
  "description": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
  "color": "Branco"
}'``


### 5 - Consultar produto - GET

url:

http://localhost:50020/Products/3fa85f64-5717-4562-b3fc-2c963f66afa6

curl:

``curl -X 'GET' \
  'http://localhost:50020/Products/3fa85f64-5717-4562-b3fc-2c963f66afa6' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``

### 6 - Listar produtos - GET

url:
http://localhost:50020/Products

curl:

``curl -X 'GET' \
  'http://localhost:50020/Products' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``

### 7 - Exclusão de produto - DELETE - http://localhost:50020/products

#### Dados Inválidos - 404 Not Found

url:

``http://localhost:50020/Products/a78a8ef6-6721-41ce-a668-2409e5fb28a4``

curl:

``curl -X 'DELETE' \
  'http://localhost:50020/Products/a78a8ef6-6721-41ce-a668-2409e5fb28a4' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``

#### Dados Válidos

url:

``http://localhost:50020/Products/3fa85f64-5717-4562-b3fc-2c963f66afa6
``

curl:

``curl -X 'DELETE' \
  'http://localhost:50020/Products/3fa85f64-5717-4562-b3fc-2c963f66afa6' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``


### 8 - Criação de pedido - POST - http://localhost:50030/orders

 #### Dados Inválidos - Quantidade tem que ser maior ou igual a 1

 payload:

 ``{
  "id": "7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e",
  "orderItems": [
    {
      "productId": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
      "quantity": 0
    }
  ]
}``

curl:

``curl -X 'POST' \
  'http://localhost:50030/Orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e",
  "orderItems": [
    {
      "productId": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
      "quantity": 0
    }
  ]
}'``

 #### Dados Válidos

 payload:

 ``{
  "id": "7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e",
  "orderItems": [
    {
      "productId": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
      "quantity": 3
    }
  ]
}``

curl:

``curl -X 'POST' \
  'http://localhost:50030/Orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": "7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e",
  "orderItems": [
    {
      "productId": "893474d1-b7c7-4fb5-bcf1-94627dd5b62a",
      "quantity": 3
    }
  ]
}'``

 ### 8 - Consultar Pedido - GET

 #### Dados Inválidos - 404 Not found

 url: http://localhost:50030/Orders/dd5d8ade-78d7-44d3-8c65-fd362ceb48d4

curl:

``curl -X 'GET' \
  'http://localhost:50030/Orders/dd5d8ade-78d7-44d3-8c65-fd362ceb48d4' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``

 #### Dados Válidos

 url: http://localhost:50030/Orders/7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e

 curl:

 ``curl -X 'GET' \
  'http://localhost:50030/Orders/7c47d1fa-7cd8-4ea0-9fee-077ddb63d67e' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``


 ### 9 - Listar pedidos - GET

 url: http://localhost:50030/Orders

 curl:

 ``curl -X 'GET' \
  'http://localhost:50030/Orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer eyJhbGciOiJQUzI1NiIsImtpZCI6IjRDc01FN2RIT2t4U1paOTJMeExESVEiLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI3N2ZjODllZi1mMzBmLTRiMjEtYjFjNS1kZjU4ZjE0NWI4NzQiLCJlbWFpbCI6ImFjZXJ0b0BhY2VydG8uY29tLmJyIiwianRpIjoiNmFhYzM0OTAtZDcyMi00NWQwLTgyYjYtNjIzZDE5ZjAzNzlhIiwibmJmIjoxNzE1MDk1NDQ2LCJpYXQiOjE3MTUwOTU0NDYsImV4cCI6MTcxNTE4MTg0NiwiaXNzIjoiZGVzYWZpby5iYWNrZW5kLmFjZXJ0by5jb20uYnIifQ.bkkuFogU8DedtZJ3JtfmT5lujK9IXM76WQpJ2twEAFE-pkpWZWdwzqUTI1TdjFmvmLiU0qZ22m8sgDUSeVC8Qucue-PMJC6FKFiD2byprTKoYbyXCSQT_fFW6eMVKFDdKOufYMMPQII1C0RA4uCr_MuZBMFvc9AofwoGPBwsdrGQnUMLdPry1ktCqKM3iYCaLvwO5j1gaGGzAM3UCJ03bFNTmpDpjzbWe5Mc_uQnP7PfEzK8cneaAYQEn1B1WYsu8mFUgZgcce4SYnCJr8d4zIvMSoiyK8vF1WdF9HndOcVPuDJstOcVlMuq1oMK1kS_goY5Sx-HH19aRuD14U4INRB7nd39myohOFWxaBcbfTa5aRZ0SJfi-uvB6C78G6GigT6tg4tKbfMyKGDxMyO3-_Ph0qeLihrMdyafPps4gMg4alUIS99PlAjwj4EX6nX3G9blv1gJd6W5t7Ct1PfEPA3Q-l6PwaVdDlipDmwIcXZEoiFd5l2DA5svPdHot93i'``

  Observe as trocas de status que acontecem automaticamente após alguns segundos/minutos

