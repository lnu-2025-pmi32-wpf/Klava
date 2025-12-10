# Klava

## Платформа для менеджменту завдань та командної роботи

### Опис

Зручний Windows-додаток для студентів, який дозволяє організовувати навчальний процес: керувати командами, предметами, завданнями, дедлайнами та матеріалами в одному місці.

---

### Основні можливості

- **Управління командами** – користувач створює команду, інші учасники приєднуються за кодом, розподіл ролей (студент/голова команди).
- **Предмети** – організація предметів у командах з описами, статусами (тест/екзамен), матеріалами та файлами.
- **Завдання** – створення завдань до предметів з дедлайнами, описами та відсліджування статусу здачі.
- **Здачі робіт** – студенти подають розв'язки завдань, автоматичне оновлення статусу (чекає/виконано).
- **Робочі матеріали** – завантаження та розповсюдження файлів та ресурсів у предметі.
- **Оголошення** – додавання оголошень і подій для команди.
- **Управління користувачами** – реєстрація, аутентифікація, керування профілями.

---

### Технології та інструменти
- **Платформа** .NET 9
- **Entity Framework Core** - ORM для ефективної роботи з базою даних.
- **UI-фреймворк** - WPF (Windows Presentation Foundation) з MVVM провайдером. 
- **База даних** - PostgreSQL 12+
- **MVVM** - CommunityToolkit.Mvvm

---

### Архітектура проєкту

```
Klava.sln
├── Klava.Domain              # Сутності, енуми, основні типи
├── Klava.Infrastructure      # EF Core, DbContext, міграції, сховище файлів
├── Klava.Application         # Бізнес-логіка, сервіси, DTO
├── Klava.WPF                 # Десктоп-додаток (WPF)
└── Klava.DataSeeder          # Консольний інструмент для керування БД і тестовими даними
```

**Взаємодія шарів:**
- `WPF` → `Application` (сервіси) → `Infrastructure` (EF Core) → PostgreSQL
- `Domain` містить основні сутності, які використовуються всюди

---

###  Запуск додатку

#### Передумови

1. **PostgreSQL** – встановити локально або запустити Docker-контейнер:
   ```powershell
   docker run --name klava-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=admin -e POSTGRES_DB=klava_db -p 5432:5432 -d postgres:15
   ```

2. **.NET 9 SDK** – встановити з [dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

#### Кроки для запуску

**1. Підготовка бази даних**

Перейдіть до папки `src`:
```powershell
cd src
```

Виконайте SQL-скрипт для створення таблиць:
```powershell
# Через psql або PgAdmin
psql -U postgres -d klava_db -f sql/klava_database.sql
```

Або застосуйте EF Core міграції (якщо налаштовані):
```powershell
dotnet ef database update --project .\Klava.Infrastructure\Klava.Infrastructure.csproj --startup-project .\Klava.WPF\Klava.WPF.csproj
```

**2. (Опціонально) Заповнити тестовими даними**

```powershell
dotnet run --project .\Klava.DataSeeder\Klava.DataSeeder.csproj
```
Виберіть пункт "10. Seed Database with Test Data" у меню.

**3. Запуск веб-додатку**

```powershell
dotnet run --project .\Klava.UI\Klava.UI.csproj
```

- Додаток стартуватиме на локальному сервері (зазвичай `https://localhost:5001` або подібно)
- Консоль покаже точну URL
- Файли завантажуються в папку `Klava.UI/uploads`

**4. Запуск десктоп-додатку (Windows)**

Через Visual Studio:
```
Відкрийте Klava.sln → Set Klava.WPF as Startup Project → Run (F5)
```

Або через CLI:
```powershell
dotnet run --project .\Klava.WPF\Klava.WPF.csproj
```

WPF застосунок підключається до тієї ж PostgreSQL (див. `appsettings.json` у папці WPF).

---

### Налаштування

**Рядок підключення до БД** знаходиться в:
- `Klava.UI/appsettings.json` – для веб-додатку
- `Klava.WPF/appsettings.json` – для WPF
- `Klava.DataSeeder/Config.cs` – для сідера

За замовчуванням:
```
Host=localhost;Port=5432;Database=klava_db;Username=postgres;Password=admin
```

Змініть значення, якщо ваш PostgreSQL використовує інші параметри.

---

### Основні сервіси

| Сервіс | Функція |
|--------|---------|
| `AuthService` | Реєстрація, аутентифікація, керування користувачами |
| `TeamService` | Управління командами та їх кодами приєднання |
| `SubjectService` | Управління предметами та їх статусами |
| `TaskService` | Управління завданнями та дедлайнами |
| `SubmissionService` | Керування здачами завдань студентами |
| `SubjectFileService` | Завантаження та завантажування файлів |
| `MemberService` | Управління членами команд і ролями |

---

### DTO та моделі

Проєкт використовує DTO (Data Transfer Objects) для комунікації між шарами:
- `UserDto`, `TeamDto`, `SubjectDto`, `TaskDto`, `SubmissionStatus`, тощо – у папці `Klava.Application/DTOs`

---

### Основні сутності

| Сутність | Опис |
|----------|------|
| `User` | Користувач системи |
| `Team` | Команда студентів з унікальним кодом |
| `TeamMember` | Зв'язок користувача з командою (роль: студент/власник) |
| `Subject` | Предмет, який викладається в команді |
| `Task` | Завдання в межах предмету |
| `SubjectFile` | Файл, прикріплений до предмету |
| `Submission` | Здача студентом завдання |

---

### Команда
**Frontend Developers:**  
- Гевель Вікторія — Project Manager  
- Стасів Тарас  

**Backend Developers:**  
- Досяк Святослав  
- Тимо Оксана  
- Калиниченко Дмитро
