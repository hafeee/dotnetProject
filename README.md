# Flowerspot app

For connecting to potgresql you need to run:
```
create database flowerspot;
CREATE ROLE hamza WITH LOGIN PASSWORD 'pass';
GRANT CONNECT ON DATABASE flowerspot TO hamza;
```

After that you can run:
```
dotnet ef migrations add "initial_migrations"
dotnet ef database update
```

