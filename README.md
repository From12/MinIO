cd MinIO

dotnet restore

dotnet build

docker-compose up -d postgres minio

# Проверка, что контейнеры запущены (должны быть видны files-postgres и files-minio)
docker ps

cd FilesWebAPI

# Создаём миграцию (если её нет)
dotnet ef migrations add InitialCreate --project ../Files.Persistence

# Применяем миграцию к БД
dotnet ef database update --project ../Files.Persistence

#Если команда dotnet ef не найдена
dotnet tool install --global dotnet-ef

dotnet run


#Тестирование API через Swagger (http://localhost:5000)

#Загрузка файла (POST /api/files/upload)
В Swagger найдите endpoint POST /api/files/upload
Выберите файл
Введите userId (любой GUID, например: 550e8400-e29b-41d4-a716-446655440000)
Введите expiresAt
Нажмите Execute

#Получение файла (GET /api/files/{id})
Скопируйте fileId из ответа загрузки и используйте для получения информации

#Удаление
введите FileId, UserID, выберите soft или permanent (с удалением из БД) delete

#Получение списка файлова /api/Files/user/{userId}
Введите UserID
#Получение ссылки для скачивания /api/Files/{id}/download-url
Введите FileID
