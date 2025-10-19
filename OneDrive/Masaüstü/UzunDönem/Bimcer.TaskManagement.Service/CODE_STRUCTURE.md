# Repository Pattern - Kod Yapısı

## 📁 Dosya Hiyerarşisi

```
src/
│
├── Bimcer.TaskManagement.Service.Core/
│   └── Abstractions/
│       ├── IEntity.cs (moved to Entity)
│       ├── IEntityRepository.cs ✅ UPDATED
│       │   └── Generic repository interface
│       │       - GetByIdAsync<T>()
│       │       - FirstOrDefaultAsync<T>()
│       │       - GetAllAsync<T>()
│       │       - AddAsync<T>()
│       │       - Update<T>()
│       │       - Delete<T>()
│       │       - SaveChangesAsync()
│       │
│       ├── IUserRepository.cs ✅ UPDATED
│       │   └── User-specific interface
│       │       - GetByEmailAsync()
│       │
│       └── ITaskRepository.cs ✅ UPDATED
│           └── Task-specific interface
│               - GetByUserAsync()
│
├── Bimcer.TaskManagement.Service.Entity/
│   ├── Abstractions/
│   │   └── IEntity.cs ✅ NEW
│   │       └── Marker interface
│   │
│   └── Entities/
│       ├── User.cs
│       ├── TaskItem.cs
│       └── RefreshToken.cs
│
├── Bimcer.TaskManagement.Service.DataAccess/
│   ├── Repositories/ ✅ NEW FOLDER
│   │   ├── EntityRepository.cs ✅ NEW
│   │   │   └── Generic repository implementation
│   │   │       - Implements IEntityRepository<T>
│   │   │       - Base class for all repositories
│   │   │       - CRUD operations
│   │   │       - Query helpers
│   │   │
│   │   ├── UserRepository.cs ✅ NEW
│   │   │   └── User repository implementation
│   │   │       - Extends EntityRepository<User>
│   │   │       - Implements IUserRepository
│   │   │       - GetByEmailAsync()
│   │   │
│   │   ├── TaskRepository.cs ✅ NEW
│   │   │   └── Task repository implementation
│   │   │       - Extends EntityRepository<TaskItem>
│   │   │       - Implements ITaskRepository
│   │   │       - GetByUserAsync()
│   │   │
│   │   └── RefreshTokenRepository.cs ✅ NEW
│   │       └── RefreshToken repository
│   │           - Extends EntityRepository<RefreshToken>
│   │           - GetActiveTokensByUserAsync()
│   │           - GetByTokenAsync()
│   │
│   ├── TaskManagementDbContext.cs
│   │   └── EF Core DbContext
│   │
│   └── ServiceCollectionExtensions.cs ✅ UPDATED
│       └── DI registration
│           - AddScoped<IUserRepository, UserRepository>()
│           - AddScoped<ITaskRepository, TaskRepository>()
│           - AddScoped<RefreshTokenRepository>()
│
├── Bimcer.TaskManagement.Service.Business/
│   └── (Services will use repositories)
│
└── Bimcer.TaskManagement.Service.WebAApi/
    └── (Controllers will use services)
```

---

## 🔗 Bağımlılık Grafiği

```
WebAPI Layer
    ↓
Business Layer (Services)
    ↓
DataAccess Layer (Repositories)
    ├─ EntityRepository<T> (Generic)
    ├─ UserRepository
    ├─ TaskRepository
    └─ RefreshTokenRepository
    ↓
Core Layer (Abstractions)
    ├─ IEntityRepository<T>
    ├─ IUserRepository
    └─ ITaskRepository
    ↓
Entity Layer (Domain Models)
    ├─ User
    ├─ TaskItem
    └─ RefreshToken
    ↓
Database (SQL Server)
```

---

## 📋 Sınıf Diyagramı

```
┌─────────────────────────────────────────┐
│      IEntityRepository<T>               │
│  (Core.Abstractions)                    │
├─────────────────────────────────────────┤
│ + GetByIdAsync(id, includes)            │
│ + FirstOrDefaultAsync(predicate, ...)   │
│ + GetAllAsync(predicate, orderBy, ...)  │
│ + AddAsync(entity)                      │
│ + Update(entity)                        │
│ + Delete(entity)                        │
│ + SaveChangesAsync()                    │
└─────────────────────────────────────────┘
           ▲
           │ implements
           │
┌─────────────────────────────────────────┐
│    EntityRepository<T>                  │
│  (DataAccess.Repositories)              │
├─────────────────────────────────────────┤
│ # _context: DbContext                   │
│ # _dbSet: DbSet<T>                      │
├─────────────────────────────────────────┤
│ + GetByIdAsync(id, includes)            │
│ + FirstOrDefaultAsync(predicate, ...)   │
│ + GetAllAsync(predicate, orderBy, ...)  │
│ + AddAsync(entity)                      │
│ + Update(entity)                        │
│ + Delete(entity)                        │
│ + SaveChangesAsync()                    │
└─────────────────────────────────────────┘
      ▲              ▲              ▲
      │              │              │
      │ extends      │ extends      │ extends
      │              │              │
┌──────────────┐ ┌──────────────┐ ┌──────────────────┐
│ UserRepository│ │TaskRepository│ │RefreshTokenRepo  │
│              │ │              │ │                  │
├──────────────┤ ├──────────────┤ ├──────────────────┤
│ +GetByEmail()│ │+GetByUser()  │ │+GetActiveTokens()│
│              │ │              │ │+GetByToken()     │
└──────────────┘ └──────────────┘ └──────────────────┘
      ▲              ▲
      │              │
      │implements    │implements
      │              │
┌──────────────┐ ┌──────────────┐
│IUserRepository│ │ITaskRepository│
│              │ │              │
├──────────────┤ ├──────────────┤
│+GetByEmail()│ │+GetByUser()  │
└──────────────┘ └──────────────┘
```

---

## 🔄 Veri Akışı

### Create (Oluştur)
```
Controller
    ↓
Service
    ↓
Repository.AddAsync(entity)
    ↓
DbContext.Add(entity)
    ↓
Repository.SaveChangesAsync()
    ↓
DbContext.SaveChangesAsync()
    ↓
Database
```

### Read (Oku)
```
Controller
    ↓
Service
    ↓
Repository.GetByIdAsync(id)
    ↓
DbContext.Set<T>().FirstOrDefaultAsync()
    ↓
Database
    ↓
Entity
```

### Update (Güncelle)
```
Controller
    ↓
Service
    ↓
Repository.Update(entity)
    ↓
DbContext.Update(entity)
    ↓
Repository.SaveChangesAsync()
    ↓
DbContext.SaveChangesAsync()
    ↓
Database
```

### Delete (Sil)
```
Controller
    ↓
Service
    ↓
Repository.Delete(entity)
    ↓
DbContext.Remove(entity)
    ↓
Repository.SaveChangesAsync()
    ↓
DbContext.SaveChangesAsync()
    ↓
Database
```

---

## 📊 Metot Özeti

### EntityRepository<T> - 7 Metot

| Metot | Parametre | Dönüş | Amaç |
|-------|-----------|-------|------|
| GetByIdAsync | int id, includes | T? | ID ile getir |
| FirstOrDefaultAsync | predicate, asNoTracking, includes | T? | Koşula göre getir |
| GetAllAsync | predicate, orderBy, asNoTracking, includes | List<T> | Tümünü getir |
| AddAsync | T entity, CancellationToken | Task | Ekle |
| Update | T entity | void | Güncelle |
| Delete | T entity | void | Sil |
| SaveChangesAsync | CancellationToken | Task<int> | Kaydet |

### UserRepository - 1 Metot

| Metot | Parametre | Dönüş | Amaç |
|-------|-----------|-------|------|
| GetByEmailAsync | string email, asNoTracking | Task<User?> | Email ile getir |

### TaskRepository - 1 Metot

| Metot | Parametre | Dönüş | Amaç |
|-------|-----------|-------|------|
| GetByUserAsync | int userId, asNoTracking | Task<List<TaskItem>> | Kullanıcının görevleri |

### RefreshTokenRepository - 2 Metot

| Metot | Parametre | Dönüş | Amaç |
|-------|-----------|-------|------|
| GetActiveTokensByUserAsync | int userId, asNoTracking | Task<List<RefreshToken>> | Aktif token'lar |
| GetByTokenAsync | string token, asNoTracking | Task<RefreshToken?> | Token ile getir |

---

## 🎯 Tasarım Desenleri

### 1. Generic Repository Pattern
- Tüm entity'ler için ortak CRUD işlemleri
- Code reusability
- Maintenance kolaylığı

### 2. Specification Pattern (Hazır)
- GetAllAsync ile predicate desteği
- Filtreleme, sıralama, include

### 3. Unit of Work Pattern (Hazır)
- SaveChangesAsync ile transaction yönetimi
- DbContext ile entegrasyon

### 4. Dependency Injection
- Interface-based abstraction
- Loose coupling
- Testability

---

## ✨ Özellikler

### Performance
- ✅ AsNoTracking desteği
- ✅ Include (eager loading)
- ✅ Lazy loading desteği
- ✅ Query optimization

### Flexibility
- ✅ Generic repository
- ✅ Özel repository'ler
- ✅ Custom queries
- ✅ Expression support

### Maintainability
- ✅ Clean code
- ✅ XML documentation
- ✅ Error handling
- ✅ Validation

### Testability
- ✅ Interface-based
- ✅ Mock-friendly
- ✅ Dependency injection
- ✅ Async support

---

## 🚀 Kullanım Örneği

```csharp
// Service'de
public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    // Kullanıcı getir
    public async Task<User?> GetUserAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id, u => u.Tasks);
    }
    
    // Email ile getir
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
    
    // Yeni kullanıcı
    public async Task CreateAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
```

---

**Oluşturulma Tarihi:** 2025-10-16

