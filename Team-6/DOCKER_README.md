# Docker Setup для Team-6

Этот документ описывает, как запустить весь проект Team-6 в Docker контейнерах.

## Архитектура

Проект состоит из следующих сервисов:

### Инфраструктура
- **PostgreSQL** (порт 5432) - основная база данных
- **RabbitMQ** (порты 5672, 15672) - брокер сообщений
- **Seq** (порт 5341) - централизованное логирование

### Микросервисы
- **API Gateway** (порт 5003) - единая точка входа
- **Auth Service** (порт 56468) - аутентификация и авторизация
- **Conversation Service** (порт 55822) - управление обращениями
- **Channel Settings** (порт 7220) - настройки каналов
- **Conversation Distributed** - распределение обращений

### Фронтенд
- **Frontend** (порт 3000) - React приложение с Nginx

## Быстрый старт

### 1. Разработка (только инфраструктура)

```powershell
# Запустить только инфраструктурные сервисы
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

### 2. Продакшн (все в Docker)

```powershell
# Запустить все сервисы в Docker
.\scripts\start-prod.ps1
```

### 3. Остановка

```powershell
# Остановить все сервисы
.\scripts\stop.ps1
```

## Ручное управление

### Запуск продакшн среды

```bash
# Сборка и запуск всех сервисов
docker-compose up --build -d

# Просмотр логов
docker-compose logs -f [service-name]

# Остановка
docker-compose down
```

### Запуск только инфраструктуры

```bash
# Только база данных, RabbitMQ и Seq
docker-compose -f docker-compose.dev.yml up -d

# Остановка
docker-compose -f docker-compose.dev.yml down
```

## Доступ к сервисам

После запуска продакшн среды:

- **Фронтенд**: http://localhost:3000
- **API Gateway**: http://localhost:5003
- **Auth Service**: http://localhost:56468
- **Conversation Service**: http://localhost:55822
- **Channel Settings**: http://localhost:7220
- **RabbitMQ Management**: http://localhost:15672 (admin/admin)
- **Seq Logging**: http://localhost:5341

## Переменные окружения

Основные переменные окружения настраиваются в `docker-compose.yml`:

- `POSTGRES_DB` - имя базы данных
- `POSTGRES_USER` - пользователь PostgreSQL
- `POSTGRES_PASSWORD` - пароль PostgreSQL
- `RABBITMQ_DEFAULT_USER` - пользователь RabbitMQ
- `RABBITMQ_DEFAULT_PASS` - пароль RabbitMQ
- `VITE_API_URL` - URL API Gateway для фронтенда

## Volumes

Данные сохраняются в Docker volumes:

- `postgres_data` - данные PostgreSQL
- `rabbitmq_data` - данные RabbitMQ
- `seq_data` - логи Seq

## Troubleshooting

### Проблемы с портами

Если порты заняты, измените их в `docker-compose.yml`:

```yaml
ports:
  - "3001:80"  # Вместо 3000:80
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

### Просмотр логов

```bash
# Все сервисы
docker-compose logs -f

# Конкретный сервис
docker-compose logs -f api-gateway
docker-compose logs -f frontend
```

## Разработка

Для разработки рекомендуется:

1. Запустить только инфраструктуру через `docker-compose.dev.yml`
2. Запускать .NET сервисы локально для быстрой отладки
3. Запускать фронтенд локально через `npm run dev`

Это позволяет:
- Быстро перезапускать сервисы при изменениях
- Использовать отладчик Visual Studio
- Видеть изменения фронтенда в реальном времени
