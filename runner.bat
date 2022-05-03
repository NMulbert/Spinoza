@echo off

taskkill /F /IM daprd.exe 

taskkill /F /IM dapr.exe

Start /B dapr run --app-id testaccessor --app-port 5001 --dapr-http-port 50001 -- dotnet run --project %spinoza_root%\backend\Accessors\TestCatalogAccessor --urls http://localhost:5001

Start /B dapr run --app-id catalogmanager --app-port 5000 --dapr-http-port 50000 -- dotnet run --project %spinoza_root%\backend\Managers\CatalogManager\

cd %spinoza_root%\frontend

npm start
