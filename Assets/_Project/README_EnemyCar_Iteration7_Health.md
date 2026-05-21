# Ітерація 7: Ворог-машина — здоров'я і смерть

## Що додано

### Нові файли

| Файл | Шлях | Опис |
|------|------|------|
| `EnemyCarDeadState.cs` | `Scripts/AI/` | Стан смерті |
| `EnemyCarHitbox.cs` | `Scripts/` | Перенаправляє урон на контролер |
| `EnemyCarController.cs` | `Scripts/` | **ЗАМІНИТИ** — тепер з IDamageable |

---

## Як налаштувати

### Крок 1: Скопіювати файли

```
Assets/_Project/Scripts/
├── AI/
│   └── EnemyCarDeadState.cs    [ДОДАТИ]
├── EnemyCarController.cs       [ЗАМІНИТИ]
└── EnemyCarHitbox.cs           [ДОДАТИ]
```

### Крок 2: Перевірити IDamageable

Переконайся що інтерфейс `IDamageable` існує:

```csharp
public interface IDamageable
{
    void TakeDamage(float damage);
}
```

### Крок 3: Налаштувати в інспекторі

На `EnemyCarController` з'явились нові поля:

**Health:**
| Поле | Опис | Рекомендація |
|------|------|--------------|
| Max Health | Максимальне здоров'я | 100 |

**Death:**
| Поле | Опис | Рекомендація |
|------|------|--------------|
| Explosion Prefab | Префаб вибуху (ParticleSystem) | Створи або пропусти |
| Explosion Sound | Звук вибуху | Додай або пропусти |
| Explosion Volume | Гучність вибуху | 1 |
| Destroy Delay | Затримка перед Destroy | 0.1 |

### Крок 4: Налаштувати Hitbox (опційно)

Якщо хочеш щоб попадання в різні частини машини рахувались:

1. На дочірні об'єкти (Body, Wheels) додай `EnemyCarHitbox`
2. Переконайся що на них є Collider
3. `EnemyCarHitbox` автоматично знайде `EnemyCarController` в parent

---

## FSM логіка

```
┌─────────────┐                           ┌──────────────┐
│   DRIVING   │ ◀────────────────────────▶│  ATTACKING   │
└─────────────┘                           └──────────────┘
       │                                         │
       │         Health <= 0                     │
       └──────────────────┬──────────────────────┘
                          ▼
                   ┌─────────────┐
                   │    DEAD     │
                   │  (взрыв)    │
                   └─────────────┘
```

---

## Як тестувати

1. Запусти гру
2. Стріляй по машині ворога
3. Після достатньої кількості попадань — вибух і знищення

### Перевірки

| Тест | Очікування |
|------|------------|
| Стрільба по ворогу | Урон наноситься |
| Здоров'я = 0 | Вибух (партикли + звук) |
| Після вибуху | GameObject знищується |

---

## Структура ворога

```
EnemyCar
├── [Rigidbody]
├── [Collider] + [EnemyCarHitbox] (або IDamageable на контролері)
├── [EnemyCarController]
├── Body
│   └── [Collider] + [EnemyCarHitbox] (опційно)
├── Wheels/
└── MuzzlePoint
    └── MuzzleFlash
```
