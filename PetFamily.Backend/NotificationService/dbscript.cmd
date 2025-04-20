docker exec postgres psql -U postgres -c "DROP DATABASE IF EXISTS notification_service;"
docker exec postgres psql -U postgres -c "CREATE DATABASE notification_service;"

dotnet-ef database drop -f -c WriteDbContext -p .\src\NotificationService\ -s .\src\NotificationService\

dotnet-ef migrations remove -c WriteDbContext -p .\src\NotificationService\ -s .\src\NotificationService\

dotnet-ef migrations add NotificationService_Init -c WriteDbContext -p .\src\NotificationService\ -s .\src\NotificationService\

dotnet-ef database update -c WriteDbContext -p .\src\NotificationService\ -s .\src\NotificationService\

pause