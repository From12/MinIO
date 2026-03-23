**API для управления файлами с хранением в MinIO**, построенный на принципах **Clean Architecture** с использованием паттерна **CQRS**.

---

## ✨ Возможности

- 📤 **Загрузка файлов** в S3-совместимое хранилище MinIO
- 📥 **Скачивание файлов** через presigned URLs
- 📋 **Управление файлами** — просмотр, поиск, удаление
- 🔐 **Мягкое удаление** с возможностью восстановления
- ⏰ **Временные ссылки** с настраиваемым сроком жизни
- ✅ **Валидация файлов** — размер, тип, имя
- 📊 **Пагинация** при просмотре списка файлов
- 📖 **Swagger UI** для документации и тестирования API

---

## 🏗️ Архитектура



### Принципы проектирования

| Принцип | Описание |
|---------|----------|
| **SOLID** | Каждый класс имеет одну ответственность, открыт для расширения |
| **CQRS** | Разделение команд (запись) и запросов (чтение) |
| **DIP** | Зависимость от абстракций, а не от реализаций |
| **Repository** | Абстракция доступа к данным |
| **Options Pattern** | Типизированная конфигурация |

---

## 🛠️ Технологии

### Backend
- **.NET 10.0** — платформа разработки
- **ASP.NET Core Web API** — REST API
- **MediatR** — реализация CQRS
- **AutoMapper** — маппинг DTOs
- **FluentValidation** — валидация запросов
- **Serilog** — структурированное логирование

### База данных
- **PostgreSQL 16** — основная БД
- **Entity Framework Core 10** — ORM

### Хранилище файлов
- **MinIO** — S3-совместимое объектное хранилище

### Документация
- **Swagger/OpenAPI** — интерактивная документация API

---

## 📋 Требования

- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started/) (для PostgreSQL и MinIO)
- IDE: [Visual Studio 2026](https://visualstudio.microsoft.com/) или [JetBrains Rider](https://www.jetbrains.com/rider/)

---

## 🚀 Быстрый старт

### 1. Клонирование репозитория

```bash
git clone https://github.com/your-username/files-storage-api.git
cd files-storage-api
```

### 2. Запуск инфраструктуры

```bash
docker-compose up -d postgres minio
```

Проверьте статус контейнеров:
```bash
docker ps
```

### 3. Восстановление пакетов

```bash
dotnet restore
```

### 4. Применение миграций

```bash
cd Presentation/WebAPI
dotnet ef migrations add InitialCreate --project ../../Infrastructure/Persistence
dotnet ef database update --project ../../Infrastructure/Persistence
```

### 5. Запуск приложения

```bash
dotnet run
```

### 6. Открытие Swagger UI

```
http://localhost:5000
```

---

## 📖 API Endpoints

| Метод | Endpoint | Описание |
|-------|----------|----------|
| `POST` | `/api/files/upload` | Загрузить файл |
| `GET` | `/api/files/{id}` | Получить информацию о файле |
| `GET` | `/api/files/user/{userId}` | Список файлов пользователя |
| `GET` | `/api/files/{id}/download-url` | Получить ссылку для скачивания |
| `DELETE` | `/api/files/{id}` | Удалить файл |

### Примеры запросов

#### Загрузка файла

```bash
curl -X POST "http://localhost:5000/api/files/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@document.pdf" \
  -F "userId=550e8400-e29b-41d4-a716-446655440000"
```

**Ответ:**
```json
{
  "fileId": "abc123-def456",
  "fileName": "document.pdf",
  "size": 102400,
  "contentType": "application/pdf",
  "downloadUrl": "http://localhost:9000/files/abc123...",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

#### Получение списка файлов

```bash
curl "http://localhost:5000/api/files/user/550e8400-e29b-41d4-a716-446655440000?page=1&pageSize=10"
```

---

## 📁 Структура проекта

```
MinIO/
│
├── Core/                              # Ядро приложения
│   │
│   ├── Domain/                        # Доменный слой
│   │   ├── Entities/                  # Сущности (TFile)
│   │   ├── Interfaces/                # Интерфейсы репозиториев и сервисов
│   │   └── Exceptions/                # Доменные исключения
│   │
│   └── Application/                   # Слой приложения
│       ├── Files/
│       │   ├── Commands/              # CQRS команды (Upload, Delete)
│       │   ├── Queries/               # CQRS запросы (Get, List)
│       │   └── DTOs/                  # Объекты передачи данных
│       ├── Common/Mappings/           # Конфигурация AutoMapper
│       ├── Interfaces/                # Интерфейсы (IFilesDbContext)
│       └── DependencyInjection.cs     # Регистрация сервисов
│
├── Infrastructure/                    # Инфраструктура
│   │
│   ├── Persistence/                   # Работа с БД
│   │   ├── Contexts/                  # DbContext
│   │   ├── Configurations/            # Конфигурация сущностей
│   │   ├── Repositories/              # Реализация репозиториев
│   │   ├── Migrations/                # Миграции EF Core
│   │   └── DependencyInjection.cs
│   │
│   └── MinIOStorage/                  # Работа с файлами
│       ├── MinioOptions.cs            # Настройки подключения
│       ├── MinioFileStorage.cs        # Реализация IFileStorageService
│       └── DependencyInjection.cs
│
├── Presentation/                      # Презентационный слой
│   └── WebAPI/
│       ├── Controllers/               # API контроллеры
│       ├── Middleware/                # Middleware компоненты
│       ├── Program.cs                 # Точка входа
│       └── appsettings.json           # Конфигурация
│
├── Tests/                             # Тесты
│   ├── Domain/                        # Unit тесты сущностей
│   └── Application/                   # Unit тесты обработчиков
│
├── docker-compose.yml                 # Docker Compose конфигурация
└── MinIO.sln                          # Solution файл
```

---

## ⚙️ Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=files_db;Username=postgres;Password=postgres"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "UseSSL": false,
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "DefaultBucket": "files",
    "PresignedUrlExpiryMinutes": 60
  }
}
```

### Переменные окружения

| Переменная | Описание |
|------------|----------|
| `ConnectionStrings__DefaultConnection` | Строка подключения к PostgreSQL |
| `MinIO__Endpoint` | Адрес MinIO сервера |
| `MinIO__AccessKey` | Access Key для MinIO |
| `MinIO__SecretKey` | Secret Key для MinIO |

---

## 🧪 Тестирование

### Unit тесты

```bash
# Запуск всех тестов
dotnet test

# Запуск с покрытием кода
dotnet test --collect:"XPlat Code Coverage"

# Запуск конкретного тестового класса
dotnet test --filter "FullyQualifiedName~TFileTests"
```


---

## 🐳 Docker

### Запуск в Docker

```bash
# Сборка и запуск всех сервисов
docker-compose up -d

# Просмотр логов
docker-compose logs -f files-api

# Остановка
docker-compose down
```

### MinIO Console

После запуска контейнеров MinIO Console доступна по адресу:

```
http://localhost:9001
```

**Учётные данные по умолчанию:**
- Login: `minioadmin`
- Password: `minioadmin`

---

## 📊 Диаграмма потока данных

```
┌──────────┐     HTTP POST      ┌──────────────┐
│  Client  │ ──────────────────▶│  Controller  │
│ (Swagger)│                    │   (WebAPI)   │
└──────────┘                    └──────┬───────┘
                                       │
                                       ▼ Command
                                ┌──────────────┐
                                │   MediatR    │
                                │  Pipeline    │
                                └──────┬───────┘
                                       │
                    ┌──────────────────┼──────────────────┐
                    ▼                  ▼                  ▼
             ┌──────────┐       ┌──────────┐       ┌──────────┐
             │ Validator│       │  Handler │       │   ...    │
             └──────────┘       └────┬─────┘       └──────────┘
                                     │
                    ┌────────────────┼────────────────┐
                    ▼                ▼                ▼
             ┌──────────┐     ┌──────────┐     ┌──────────┐
             │   TFile  │     │  MinIO   │     │ PostgreSQL│
             │ (Domain) │     │ Storage  │     │   (DB)    │
             └──────────┘     └──────────┘     └──────────┘
```

