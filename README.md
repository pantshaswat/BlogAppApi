## For Database Migration
### Install dotnet-ef if you havent already
```bash
dotnet tool install --global dotnet-ef
```
### Run the following commands
At beginning or every time you change the model classes run following command. "add change message in string".
```bash
dotnet ef migrations add "initialDBSetup"
```
Then to update the database:
```bash
dotnet ef database update
```