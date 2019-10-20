# WebProjectTemplate

This is base complete project for SPA developing with popular supporting instruments. 
* Back-end: ASP.NET Core 2.2
* Front-end: Angular 8
* Supporting instruments: SwaggerUI, Prettier.

## Back-end
The back-end has base repository class with CRUD operations, that use connection with DB context. You can use any DB, just change connection string in appsetting.json and string in Startup.cs file.

User service has methods that can be provided with User entity: authenticate, register, delete and so on.
User controller provides http methods: GET, POST, PUT, DELETE and so on.

## Front-end
The front-end has base components: login, register and home. 

Authentication service, JWT interceptor and error interceptor manage authentication process. 
In each component you can subscribe to currentUser "behaviour subject" and check is authorised someone at this moment or not. Information about authorised user is stored in "local storage". 

## Unit-tests
All components have unit-tests. You can run them with "ng test". 

## Linting
Before each committing run commands: "ng lint --fix" and "npm run format:all"
First is linting your code with Angular rules (more here). Second is modify your code style for the best developer experience.
