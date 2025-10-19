# Repository Pattern - Kullanım Örnekleri

## 1. UserRepository Kullanımı

### Kullanıcı Getir (Email ile)
```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
}
```

### Kullanıcı Getir (ID ile)
```csharp
public async Task<User?> GetUserByIdAsync(int userId)
{
    // Include ile ilişkili görevleri de getir
    return await _userRepository.GetByIdAsync(userId, u => u.Tasks);
}
```

### Tüm Kullanıcıları Getir
```csharp
public async Task<List<User>> GetAllActiveUsersAsync()
{
    return await _userRepository.GetAllAsync(
        predicate: u => u.IsActive,
        orderBy: q => q.OrderByDescending(u => u.CreatedAtUtc),
        asNoTracking: true
    );
}
```

### Yeni Kullanıcı Oluştur
```csharp
public async Task CreateUserAsync(User user)
{
    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();
}
```

### Kullanıcı Güncelle
```csharp
public async Task UpdateUserAsync(User user)
{
    _userRepository.Update(user);
    await _userRepository.SaveChangesAsync();
}
```

### Kullanıcı Sil
```csharp
public async Task DeleteUserAsync(int userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user != null)
    {
        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}
```

---

## 2. TaskRepository Kullanımı

### Kullanıcının Görevlerini Getir
```csharp
public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    
    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    
    public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
    {
        return await _taskRepository.GetByUserAsync(userId);
    }
}
```

### Görev Getir (ID ile)
```csharp
public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
{
    return await _taskRepository.GetByIdAsync(taskId, t => t.User);
}
```

### Belirli Statüdeki Görevleri Getir
```csharp
public async Task<List<TaskItem>> GetUserTasksByStatusAsync(int userId, string status)
{
    return await _taskRepository.GetAllAsync(
        predicate: t => t.UserId == userId && t.Status == status,
        orderBy: q => q.OrderByDescending(t => t.DueDateUtc),
        asNoTracking: true
    );
}
```

### Yeni Görev Oluştur
```csharp
public async Task CreateTaskAsync(TaskItem task)
{
    await _taskRepository.AddAsync(task);
    await _taskRepository.SaveChangesAsync();
}
```

### Görev Güncelle
```csharp
public async Task UpdateTaskAsync(TaskItem task)
{
    _taskRepository.Update(task);
    await _taskRepository.SaveChangesAsync();
}
```

### Görev Sil
```csharp
public async Task DeleteTaskAsync(int taskId)
{
    var task = await _taskRepository.GetByIdAsync(taskId);
    if (task != null)
    {
        _taskRepository.Delete(task);
        await _taskRepository.SaveChangesAsync();
    }
}
```

---

## 3. RefreshTokenRepository Kullanımı

### Aktif Token'ları Getir
```csharp
public class AuthService
{
    private readonly RefreshTokenRepository _refreshTokenRepository;
    
    public AuthService(RefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<List<RefreshToken>> GetActiveTokensAsync(int userId)
    {
        return await _refreshTokenRepository.GetActiveTokensByUserAsync(userId);
    }
}
```

### Token String'i ile Getir
```csharp
public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
{
    return await _refreshTokenRepository.GetByTokenAsync(token);
}
```

### Yeni Token Oluştur
```csharp
public async Task CreateRefreshTokenAsync(RefreshToken token)
{
    await _refreshTokenRepository.AddAsync(token);
    await _refreshTokenRepository.SaveChangesAsync();
}
```

---

## 4. Generic Repository Özellikleri

### Include ile İlişkili Veri Getir
```csharp
// Tek include
var user = await _userRepository.GetByIdAsync(1, u => u.Tasks);

// Birden fazla include
var user = await _userRepository.GetByIdAsync(1, 
    u => u.Tasks, 
    u => u.RefreshTokens
);
```

### AsNoTracking ile Performans Optimizasyonu
```csharp
// Tracking olmadan getir (sadece okuma işlemleri için)
var users = await _userRepository.GetAllAsync(
    asNoTracking: true
);

// Tracking ile getir (güncelleme yapacaksanız)
var user = await _userRepository.FirstOrDefaultAsync(
    u => u.Id == 1,
    asNoTracking: false
);
```

### Sıralama ile Getir
```csharp
var tasks = await _taskRepository.GetAllAsync(
    predicate: t => t.UserId == userId,
    orderBy: q => q.OrderByDescending(t => t.CreatedAtUtc)
        .ThenBy(t => t.Status)
);
```

---

## 5. Dependency Injection Kurulumu

### Program.cs'de Kayıt
```csharp
// ServiceCollectionExtensions.cs'de otomatik olarak kaydedilir
services.AddDataAccessServices(configuration);
```

### Service'de Kullanım
```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;
    
    // Constructor injection
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
```

---

## 6. Best Practices

✅ **Async/Await Kullan** - Tüm repository metotları async'tir
✅ **SaveChangesAsync Çağır** - Değişiklikleri kaydetmek için
✅ **AsNoTracking Kullan** - Sadece okuma işlemleri için performans
✅ **Include Kullan** - N+1 query problemini önlemek için
✅ **Null Check Yap** - Silme işlemlerinden önce
✅ **CancellationToken Kullan** - Uzun işlemler için

---

## 7. Hata Yönetimi

```csharp
public async Task<User?> GetUserSafelyAsync(int userId)
{
    try
    {
        return await _userRepository.GetByIdAsync(userId);
    }
    catch (ArgumentNullException ex)
    {
        // Context null ise
        _logger.LogError(ex, "Repository context is null");
        return null;
    }
    catch (Exception ex)
    {
        // Diğer hatalar
        _logger.LogError(ex, "Error getting user");
        return null;
    }
}
```

