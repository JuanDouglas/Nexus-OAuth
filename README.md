# C# OAuth [Asp.Net Core](https://asp.net) API
This project is the implementation of an OAuth service using .NET Core 6 based on [RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749)
that governs the authentication protocol is authorization using the OAuth mechanism.

## Project structure
The project solution is divided into two folders
- API

	Contains the implementations of the OAuth system for the authorization server.

- Client

	Contains clients for platforms such as:
    - Android
    - Linux
    - Windows
    - Test Client

# Starting API
To start the API for the first time you need:
- Add the connection key corresponding to your database in the 'appsettings.json' file within the 'Nexus.OAuth.Api' project.
- Run the migration inside your server with the same connection, for that run the following command in the root of the project:
    ```
    dotnet ef database update --connection "{connectionString}" -p Nexus.OAuth.Dal
- If you do not have EntityFramework installed on your machine, run this command before the previous one:
    ```
      dotnet tool install dotnet-ef -g
- In case you need the project, it has a collection ready to test the project in **Postman**.


- Running the **API** project, run the following command in root:
    ```
    dotnet run -p Nexus.OAuth.Api
> The project is configured to not launch the browser when debugging, use the **[44360](https://localhost:44360/swagger)** port of your localhost to connect to the server.

## Test with Client
If you want you can debug with the WEB client that is in the solution for this, just:

- Start the **API** following the steps [previous](#Starting-API).
- Right after in another terminal start the web project command.
	```
      dotnet run -p Nexus.OAuth.Web
> The Web Project does not have a default Index and redirects to the [Nexus](https://nexus-company.tech/) page. So if you want to manage OAuth applications, connect to port **44337** using the route [/Applications](https://localhost:44337/Applications).