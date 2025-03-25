docker-compose up -d

dotnet-ef database drop -f -c WriteDbContext -p .\src\Volunteers\PetFamily.Volunteers.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef database drop -f -c WriteDbContext -p .\src\Species\PetFamily.Species.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef database drop -f -c AccountsDbContext -p .\src\Accounts\PetFamily.Accounts.Infrastructure\ -s .\src\PetFamily.Web\


dotnet-ef migrations remove -c WriteDbContext -p .\src\Volunteers\PetFamily.Volunteers.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef migrations remove -c WriteDbContext -p .\src\Species\PetFamily.Species.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef migrations remove -c AccountsDbContext -p .\src\Accounts\PetFamily.Accounts.Infrastructure\ -s .\src\PetFamily.Web\

dotnet-ef migrations add Volunteers_Init -c WriteDbContext -p .\src\Volunteers\PetFamily.Volunteers.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef migrations add Species_Init -c WriteDbContext -p .\src\Species\PetFamily.Species.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef migrations add Accounts_Init -c AccountsDbContext -p .\src\Accounts\PetFamily.Accounts.Infrastructure\ -s .\src\PetFamily.Web\

dotnet-ef database update -c WriteDbContext -p .\src\Volunteers\PetFamily.Volunteers.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef database update -c WriteDbContext -p .\src\Species\PetFamily.Species.Infrastructure\ -s .\src\PetFamily.Web\
dotnet-ef database update -c AccountsDbContext -p .\src\Accounts\PetFamily.Accounts.Infrastructure\ -s .\src\PetFamily.Web\

pause
