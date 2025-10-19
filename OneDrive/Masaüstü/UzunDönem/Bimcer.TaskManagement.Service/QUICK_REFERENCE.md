# 🚀 Repository Pattern - Hızlı Referans

## 📍 Dosya Konumları

```
src/Bimcer.TaskManagement.Service.DataAccess/Repositories/
├── EntityRepository.cs          ← Generic repository
├── UserRepository.cs            ← User-specific
├── TaskRepository.cs            ← Task-specific
└── RefreshTokenRepository.cs    ← RefreshToken-specific
```

---

## 🔧 Temel Metotlar

### Generic Repository (EntityRepository<T>)

```csharp
// READ
await repo.GetByIdAsync(id);
await repo.GetByIdAsync(id, u => u.Tasks);  // Include ile
await repo.FirstOrDefaultAsync(u => u.Email == "test@test.com");
await repo.GetAllAsync();
await repo.GetAllAsync(u => u.IsActive);  // Filtreleme
await repo.GetAllAsync(orderBy: q => q.OrderBy(u => u.Name));  // Sıralama

// WRITE
await repo.AddAsync(entity);
repo.Update(entity);
repo.Delete(entity);
await repo.SaveChangesAsync();
```

### UserRepository

```csharp
// Özel metot
await userRepo.GetByEmailAsync("user@example.com");
```

### TaskRepository

```csharp
// Özel metot
await taskRepo.GetByUserAsync(userId);
```

### RefreshTokenRepository

```csharp
// Özel metotlar
await tokenRepo.GetActiveTokensByUserAsync(userId);
await tokenRepo.GetByTokenAsync(tokenString);
```

---

## 💉 Dependency Injection

### Program.cs'de Kayıt
```csharp
services.AddDataAccessServices(configuration);
```

### Service'de Kullanım
```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
```

---

## 📝 Yaygın Kullanım Örnekleri

### 1. Tüm Veriyi Getir
```csharp
var users = await _userRepository.GetAllAsync();
```

### 2. Filtreleme ile Getir
```csharp
var activeUsers = await _userRepository.GetAllAsync(
    predicate: u => u.IsActive
);
```

### 3. Sıralama ile Getir
```csharp
var users = await _userRepository.GetAllAsync(
    orderBy: q => q.OrderByDescending(u => u.CreatedAtUtc)
);
```

### 4. Include ile Getir
```csharp
var user = await _userRepository.GetByIdAsync(
    id: 1,
    u => u.Tasks,
    u => u.RefreshTokens
);
```

### 5. Filtreleme + Sıralama + Include
```csharp
var tasks = await _taskRepository.GetAllAsync(
    predicate: t => t.UserId == userId && t.Status == "pending",
    orderBy: q => q.OrderByDescending(t => t.DueDateUtc),
    asNoTracking: true,
    t => t.User
);
```

### 6. Yeni Veri Ekle
```csharp
var user = new User { FirstName = "John", Email = "john@test.com" };
await _userRepository.AddAsync(user);
await _userRepository.SaveChangesAsync();
```

### 7. Veri Güncelle
```csharp
user.FirstName = "Jane";
_userRepository.Update(user);
await _userRepository.SaveChangesAsync();
```

### 8. Veri Sil
```csharp
_userRepository.Delete(user);
await _userRepository.SaveChangesAsync();
```

### 9. Email ile Kullanıcı Getir
```csharp
var user = await _userRepository.GetByEmailAsync("user@example.com");
```

### 10. Kullanıcının Görevlerini Getir
```csharp
var tasks = await _taskRepository.GetByUserAsync(userId);
```

---

## ⚙️ Parametreler

### GetAllAsync Parametreleri
```csharp
GetAllAsync(
    predicate: Expression<Func<T, bool>>? = null,      // WHERE
    orderBy: Func<IQueryable<T>, IOrderedQueryable<T>>? = null,  // ORDER BY
    asNoTracking: bool = true,                          // Tracking
    includes: params Expression<Func<T, object>>[]      // INCLUDE
)
```

### FirstOrDefaultAsync Parametreleri
```csharp
FirstOrDefaultAsync(
    predicate: Expression<Func<T, bool>>,               // WHERE
    asNoTracking: bool = true,                          // Tracking
    includes: params Expression<Func<T, object>>[]      // INCLUDE
)
```

---

## 🎯 Best Practices

✅ **Async/Await Kullan**
```csharp
var user = await _userRepository.GetByIdAsync(id);  // ✅ Doğru
var user = _userRepository.GetByIdAsync(id).Result;  // ❌ Yanlış
```

✅ **SaveChangesAsync Çağır**
```csharp
await _userRepository.AddAsync(user);
await _userRepository.SaveChangesAsync();  // ✅ Gerekli
```

✅ **AsNoTracking Kullan (Sadece Okuma)**
```csharp
var users = await _userRepository.GetAllAsync(asNoTracking: true);  // ✅ Performans
```

✅ **Include Kullan (N+1 Problemini Önle)**
```csharp
var user = await _userRepository.GetByIdAsync(id, u => u.Tasks);  // ✅ Doğru
```

✅ **Null Check Yap**
```csharp
var user = await _userRepository.GetByIdAsync(id);
if (user != null)
{
    _userRepository.Delete(user);
    await _userRepository.SaveChangesAsync();
}
```

---

## 🔍 Hata Yönetimi

```csharp
try
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new NotFoundException("User not found");
    
    return user;
}
catch (ArgumentNullException ex)
{
    _logger.LogError(ex, "Repository context is null");
    throw;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting user");
    throw;
}
```

---

## 📊 Performans İpuçları

| İpucu | Açıklama |
|-------|----------|
| **AsNoTracking** | Sadece okuma işlemleri için |
| **Include** | N+1 query problemini önler |
| **Select** | Sadece gerekli alanları getir |
| **Take/Skip** | Pagination için |
| **Any** | Count yerine kullan |

---

## 🧪 Test Örneği

```csharp
[TestClass]
public class UserRepositoryTests
{
    private Mock<TaskManagementDbContext> _mockContext;
    private UserRepository _repository;
    
    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<TaskManagementDbContext>();
        _repository = new UserRepository(_mockContext.Object);
    }
    
    [TestMethod]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Email = email };
        
        // Act
        var result = await _repository.GetByEmailAsync(email);
        
        // Assert
        Assert.IsNotNull(result);
    }
}
```

---

## 📚 Kaynaklar

- 📄 REPOSITORY_PATTERN_IMPLEMENTATION.md
- 📄 REPOSITORY_USAGE_EXAMPLES.md
- 📄 CODE_STRUCTURE.md
- 📄 README_STEP4.md

---

## 🎓 Öğrenme Yolu

1. **Generic Repository** - Temel CRUD işlemleri
2. **Özel Repository'ler** - Entity-specific işlemler
3. **Dependency Injection** - Service'lerde kullanım
4. **Business Logic** - Service layer'da implementasyon
5. **API Endpoints** - Controller'larda kullanım
6. **Testing** - Unit test'ler yazma

---

**Son Güncelleme:** 2025-10-16

