# 🐳 Docker Setup для Team-6

Полная настройка микросервисной системы управления обращениями в Docker контейнерах.

## 🏗️ Архитектура

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│    Frontend     │    │   API Gateway   │    │  Auth Service   │
│   (React+Nginx) │◄───┤   (Ocelot)      │◄───┤   (JWT Auth)    │
│   Port: 3000    │    │   Port: 5003    │    │   Port: 56468   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                ┌───────────────┼───────────────┐
                │               │               │
        ┌───────▼──────┐ ┌─────▼─────┐ ┌──────▼──────┐
        │ Conversation │ │  Channel  │ │ Conversation│
        │   Service    │ │ Settings  │ │ Distributed │
        │ Port: 55822  │ │Port: 7220 │ │ (Background)│
        └──────────────┘ └───────────┘ └─────────────┘
                │               │               │
                └───────────────┼───────────────┘
                                │
                    ┌───────────▼───────────┐
                    │     PostgreSQL        │
                    │     Port: 5432        │
                    └───────────────────────┘
                                │
                    ┌───────────▼───────────┐
                    │      RabbitMQ         │
                    │  Ports: 5672, 15672   │
                    └───────────────────────┘
                                │
                    ┌───────────▼───────────┐
                    │        Seq            │
                    │     Port: 5341        │
                    │   (Logging)           │
                    └───────────────────────┘
```

## 🚀 Быстрый старт

### Вариант 1: Полная продакшн среда (все в Docker)

```powershell
# Запустить все сервисы
.\scripts\start-prod.ps1

# Или вручную:
docker-compose up --build -d
```

**Доступ к сервисам:**
- 🌐 **Фронтенд**: http://localhost:3000
- 🔗 **API Gateway**: http://localhost:5003
- 🔐 **Auth Service**: http://localhost:56468
- 💬 **Conversation Service**: http://localhost:55822
- ⚙️ **Channel Settings**: http://localhost:7220
- 🐰 **RabbitMQ Management**: http://localhost:15672 (admin/admin)
- 📊 **Seq Logging**: http://localhost:5341

### Вариант 2: Разработка (только инфраструктура)

```powershell
# Запустить только инфраструктуру
.\scripts\start-dev.ps1

# Затем запустить .NET сервисы локально
dotnet run --project ApiGateway
dotnet run --project AuthService  
dotnet run --project ConversationService.Api
dotnet run --project ChannelSettings

# И фронтенд
cd frontend
npm run dev
```

## 🛠️ Управление

### Остановка всех сервисов

```powershell
.\scripts\stop.ps1

# Или вручную:
docker-compose down
```

### Просмотр логов

```bash
# Все сервисы
docker-compose logs -f

# Конкретный сервис
docker-compose logs -f api-gateway
docker-compose logs -f frontend
docker-compose logs -f auth-service
```

### Перезапуск сервиса

```bash
# Перезапустить конкретный сервис
docker-compose restart api-gateway

# Пересобрать и перезапустить
docker-compose up --build -d api-gateway
```

## 🔧 Конфигурация

### Переменные окружения

Основные настройки в `docker-compose.yml`:

```yaml
environment:
  # База данных
  POSTGRES_DB: atmccdb
  POSTGRES_USER: neondb_owner
  POSTGRES_PASSWORD: npg_1Axu8qSTmypF
  
  # RabbitMQ
  RABBITMQ_DEFAULT_USER: admin
  RABBITMQ_DEFAULT_PASS: admin
  
  # API Gateway для фронтенда
  VITE_API_URL: http://localhost:5003
```

### Порты

Если порты заняты, измените их в `docker-compose.yml`:

```yaml
ports:
  - "3001:80"  # Вместо 3000:80 для фронтенда
  - "5004:80"  # Вместо 5003:80 для API Gateway
```

## 📁 Структура файлов

```
Team-6/
├── docker-compose.yml          # Основная конфигурация
├── docker-compose.dev.yml      # Только инфраструктура
├── Dockerfile                  # Общий Dockerfile
├── .dockerignore              # Исключения для Docker
├── scripts/                   # Скрипты управления
│   ├── start-dev.ps1         # Запуск для разработки
│   ├── start-prod.ps1        # Запуск продакшн
│   └── stop.ps1              # Остановка
├── ApiGateway/
│   └── Dockerfile            # API Gateway
├── AuthService/
│   └── Dockerfile            # Auth Service
├── ConversationService.Api/
│   └── Dockerfile            # Conversation Service
├── ChannelSettings/
│   └── Dockerfile            # Channel Settings
├── ConversationDistributed/
│   └── Dockerfile            # Conversation Distributed
└── frontend/
    ├── Dockerfile            # Frontend
    ├── nginx.conf            # Nginx конфигурация
    └── .dockerignore         # Исключения для фронтенда
```

## 🐛 Troubleshooting

### Проблемы с портами

```bash
# Проверить занятые порты
netstat -an | findstr :3000
netstat -an | findstr :5003

# Остановить процесс на порту
taskkill /PID <PID> /F
```

### Проблемы с базой данных

```bash
# Очистить данные PostgreSQL
docker volume rm team-6_postgres_data

# Пересоздать контейнер
docker-compose up --force-recreate postgres
```

### Проблемы с RabbitMQ

```bash
# Очистить данные RabbitMQ
docker volume rm team-6_rabbitmq_data

# Пересоздать контейнер
docker-compose up --force-recreate rabbitmq
```

### Проблемы со сборкой

```bash
# Очистить все образы и пересобрать
docker-compose down
docker system prune -a
docker-compose up --build -d
```

## 🔍 Мониторинг

### Проверка статуса сервисов

```bash
# Статус всех контейнеров
docker-compose ps

# Использование ресурсов
docker stats
```

### Логи в реальном времени

```bash
# Все сервисы
docker-compose logs -f

# Фильтрация по уровню
docker-compose logs -f | findstr ERROR
docker-compose logs -f | findstr WARN
```

## 🚀 Развертывание

### Для продакшн среды

1. **Измените пароли** в `docker-compose.yml`
2. **Настройте домены** в nginx конфигурации
3. **Добавьте SSL сертификаты** для HTTPS
4. **Настройте мониторинг** и алерты

### Для разработки

1. Используйте `docker-compose.dev.yml` для инфраструктуры
2. Запускайте .NET сервисы локально
3. Используйте `npm run dev` для фронтенда
4. Настройте hot reload для быстрой разработки

## 📚 Дополнительные ресурсы

- [Docker Compose документация](https://docs.docker.com/compose/)
- [.NET Docker образы](https://hub.docker.com/_/microsoft-dotnet)
- [Nginx конфигурация](https://nginx.org/en/docs/)
- [PostgreSQL Docker](https://hub.docker.com/_/postgres)
- [RabbitMQ Docker](https://hub.docker.com/_/rabbitmq)
