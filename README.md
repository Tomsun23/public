# public
Для работы ПО необходимо выполнить следующие действия:
1) Установить Docker Desktop
2) выполнить
    docker pull postgres
    docker run --name postgres -p 5416:5432 -e POSTGRES_PASSWORD=admin -d postgres
3) Открыть solution
4) Открыть appsettings.json
5) Добавить
    "DbConnection": "Host=172.26.16.1; Port=5416; Database=postgres; User Id=postgres; Password=admin;"
    172.26.16.1 - IP докера, если отличается, заменить на корректный
6) Package Manager Console -> установить Default -> Clinic.Infrastructure
7) Выполнить
    Add-Migration Initial
    Update-Database
8) Консольное приложение ожидает параметр командной строки URL сервиса, например
    Clinic.Client.exe https://localhost:32781
9) Файл Postman коллекции 
    {solution_path}\Clinic.postman_collection.json 
