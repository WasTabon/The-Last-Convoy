# Итерация 3: Здоровье врага и получение урона

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Gameplay/IDamageable.cs` | Интерфейс для объектов которые могут получать урон |
| `Scripts/Views/Enemy/EnemyHelicopterHitbox.cs` | Компонент на коллайдере врага, получает урон |

### Изменённые файлы

| Путь | Что изменено |
|------|--------------|
| `Scripts/Configs/EnemyHelicopterConfig.cs` | Добавлен параметр Max Health |
| `Scripts/Gameplay/Models/EnemyHelicopterModel.cs` | Добавлена система здоровья, события OnDamaged и OnDied |
| `Scripts/Configs/WeaponConfig.cs` | Добавлен параметр Damage Per Shot |
| `Scripts/Gameplay/Presenters/WeaponPresenter.cs` | Добавлена проверка IDamageable при попадании |

---

## Как настроить

### Шаг 1: Скопировать/заменить файлы

```
Assets/_Project/Scripts/
├── Configs/
│   ├── EnemyHelicopterConfig.cs    [ЗАМЕНИТЬ]
│   └── WeaponConfig.cs             [ЗАМЕНИТЬ]
├── Gameplay/
│   ├── IDamageable.cs              [ДОБАВИТЬ]
│   ├── Models/
│   │   └── EnemyHelicopterModel.cs [ЗАМЕНИТЬ]
│   └── Presenters/
│       └── WeaponPresenter.cs      [ЗАМЕНИТЬ]
└── Views/
    └── Enemy/
        └── EnemyHelicopterHitbox.cs [ДОБАВИТЬ]
```

### Шаг 2: Настроить конфиги

**EnemyHelicopterConfig:**
- Открой свой asset
- Появилось новое поле **Max Health** (дефолт: 100)
- Настрой по желанию

**WeaponConfig:**
- Открой свой asset оружия игрока
- Появилось новое поле **Damage Per Shot** (дефолт: 5)
- При 100 HP врага и 5 урона за выстрел = 20 попаданий для убийства

### Шаг 3: Добавить Collider на врага

1. Открой prefab вражеского вертолёта
2. На корневой объект (или дочерний который охватывает модель) добавь **Collider**:
   - `Box Collider` — самый простой вариант
   - Или `Mesh Collider` если нужна точность
3. Настрой размер чтобы он охватывал корпус вертолёта
4. **Убедись что Collider НЕ является триггером** (Is Trigger = false)

### Шаг 4: Добавить EnemyHelicopterHitbox

1. На тот же объект где Collider добавь компонент `EnemyHelicopterHitbox`
2. Zenject автоматически инъектирует модель

### Шаг 5: Проверить Layer

Убедись что Layer вражеского вертолёта входит в **Hit Layers** в `WeaponConfig`:
- Если враг на Layer "Enemy" — добавь его в маску
- Или используй "Everything" (-1) для попадания во всё

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Наведи прицел на вражеский вертолёт
3. Начни стрелять
4. В консоли должны появляться сообщения (если добавишь Debug.Log в TakeDamage)
5. После достаточного количества попаданий — событие OnDied (пока визуально ничего не произойдёт, это в следующей итерации)

### Как проверить что урон работает:

**Вариант 1: Временный Debug.Log**

В `EnemyHelicopterHitbox.TakeDamage()` добавь временно:
```csharp
public void TakeDamage(float damage)
{
    Debug.Log($"[Enemy] Took {damage} damage, health: {_model.CurrentHealth}");
    _model.TakeDamage(damage);
}
```

**Вариант 2: Подписка на события в Installer**

В `EnemyHelicopterInstaller.Start()` добавь:
```csharp
public override void Start()
{
    base.Start();
    var model = Container.Resolve<EnemyHelicopterModel>();
    model.OnDamaged += (health) => Debug.Log($"[Enemy] Health: {health}");
    model.OnDied += () => Debug.Log("[Enemy] DIED!");
}
```

### Проверки:

| Тест | Ожидание |
|------|----------|
| Стрельба по врагу | В консоли видно уменьшение здоровья |
| Стрельба мимо врага | Ничего не происходит (только impact эффект) |
| Много попаданий | Событие OnDied когда здоровье <= 0 |
| Стрельба после смерти | Урон не наносится (IsDead = true) |

### Если не работает:

| Проблема | Решение |
|----------|---------|
| Урон не наносится | Проверь что Collider есть на враге |
| Ошибка "Model is null" | EnemyHelicopterHitbox должен быть на объекте с GameObjectContext или его дочернем |
| Raycast не попадает | Проверь Hit Layers в WeaponConfig |
| Попадания не регистрируются | Убедись что Collider не триггер |

---

## Архитектура системы урона

```
WeaponPresenter.PerformRaycast()
        │
        ▼
Physics.Raycast → hit.collider
        │
        ▼
GetComponent<IDamageable>()
        │
        ▼
EnemyHelicopterHitbox.TakeDamage(damage)
        │
        ▼
EnemyHelicopterModel.TakeDamage(damage)
        │
        ├──► OnDamaged(currentHealth)
        │
        └──► OnDied() [если health <= 0]
```

---

## События EnemyHelicopterModel

| Событие | Когда вызывается | Параметры |
|---------|------------------|-----------|
| `OnDamaged` | При каждом получении урона | `float currentHealth` |
| `OnDied` | Когда здоровье падает до 0 | нет |

Эти события будут использоваться в следующей итерации для падения и взрыва.
