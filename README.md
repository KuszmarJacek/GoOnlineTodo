# GoOnlineTodo
This is an example of a Todo REST API built with minimal apis.

## Requirements
This is a list of requirements that were supposed to be met for this project:
### Minimal ToDo structure
- [x] Date and Time of expiry
- [x] Title
- [x] Description
- [x] % Complete
### Required Operations
- [x] Get All Todos
- [x] Get Specific Todo
- [x] Get Incoming ToDo (for today/next day/current week)
- [x] Create Todo
- [x] Update Todo
- [x] Set Todo percent complete
- [x] Delete Todo
- [x] Mark Todo as Done
### Other requirements
- [x] .net >= 8.0
- [x] Minimal API / Web API
- [x] data persisted in db using ORM (postgresql/mysql/mariadb)
- [x] xunit for unit/integration tests
- [x] commented code
- [x] code must be ready to compile and run
- [x] solution shouldn’t require running any external scripts
- [x] data should be properly validated
### Nice to have
- [x] Code running in docker
- [ ] performance tests

## How to run
Provide your DB connection credentials in the ```appsettings.json``` file, then apply migrations (see considerations below) and after that you should be good to go.
From within the GoOnlineTodo.Api directory run ```dotnet run``` and the app should start running in development mode.
## Considerations
Being a multiproject solution, whenever migrations are to be applied remember to specify dependencies, for example: <br/>
```dotnet ef database update --project .\GoOnlineTodo.DataService\ --startup-project .\GoOnlineTodo.Api\```
