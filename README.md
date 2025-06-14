# Todo List com .NET 8 e Minimal API

Este repositório tem como objetivo demonstrar a criação de uma aplicação de Todo List utilizando **.NET 8** com o padrão **Minimal API**. O projeto também explora boas práticas como **versionamento de API** e **autenticação**.

## Objetivos

- Aprender a estruturar uma API RESTful com Minimal API no .NET 8
- Implementar versionamento de endpoints
- Adicionar autenticação utilizando AspNetCore.Identity
- Aplicar boas práticas de organização e manutenção de código
## Passos para Executar

1. Clone o repositório:

    ```bash
    git clone <URL-do-repositório>  
     ```

2. Acesse a pasta do projeto via terminal:

     ``` bash
     cd MinimalApi.TodoList
     ```
3. Gere uma imagem a partir do build do dockerfile:
     ``` bash
     docker build -t dotnet8-todo-api .
     ```
4. Execute a imagem:
     ``` bash
     docker run -p 5000:5000 dotnet8-todo-api
     ```

5. Para acessar  utilize as URLs:
-   API: http://localhost:5000
    
-   Swagger : http://localhost:5000/swagger

6. Para gerar requisições nos endpoints de TodoList

- Crie um usuário no endpoint /auth/register, passando um e-mail e uma senha que contenha:

     - Pelo menos 6 caracteres

     - Uma letra maiúscula

     - Uma letra minúscula

     - Um número

     - Um caractere especial

- Autentique-se no endpoint /auth/login.
A aplicação está configurada para utilizar cookies como mecanismo de autenticação, portanto, após o login, o cookie de autenticação será enviado automaticamente nas requisições subsequentes.
