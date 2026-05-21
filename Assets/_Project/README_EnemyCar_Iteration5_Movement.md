# Ітерація 5: Ворог-машина — базовий рух

## Що додано

### Нові файли

| Шлях | Опис |
|------|------|
| `Scripts/Configs/EnemyCarConfig.cs` | Налаштування (швидкість, здоров'я, колеса) |
| `Scripts/Gameplay/Models/EnemyCarModel.cs` | Модель з позицією, швидкістю |
| `Scripts/Gameplay/Presenters/EnemyCarPresenter.cs` | Рух по waypoints |
| `Scripts/Gameplay/Installers/EnemyCarInstaller.cs` | MonoInstaller |
| `Scripts/Views/Enemy/EnemyCarView.cs` | Синхронізація transform |
| `Scripts/Views/Enemy/EnemyCarWheelsView.cs` | Обертання коліс |

---

## Як налаштувати

### Крок 1: Скопіювати файли

```
Assets/_Project/Scripts/
├── Configs/
│   └── EnemyCarConfig.cs               [ДОДАТИ]
├── Gameplay/
│   ├── Installers/
│   │   └── EnemyCarInstaller.cs        [ДОДАТИ]
│   ├── Models/
│   │   └── EnemyCarModel.cs            [ДОДАТИ]
│   └── Presenters/
│       └── EnemyCarPresenter.cs        [ДОДАТИ]
└── Views/
    └── Enemy/
        ├── EnemyCarView.cs             [ДОДАТИ]
        └── EnemyCarWheelsView.cs       [ДОДАТИ]
```

### Крок 2: Створити Config

1. ПКМ → Create → LastConvoy → Configs → EnemyCar
2. Назви `EnemyCarConfig`
3. Налаштуй параметри

### Крок 3: Створити waypoints для ворога

1. Створи пустий об'єкт `EnemyWaypoints`
2. Всередині створи дочірні об'єкти (WP_0, WP_1, WP_2...)
3. Розташуй їх вздовж дороги **попереду гравця**

### Крок 4: Налаштувати prefab ворога

1. Створи/візьми 3D модель машини
2. На корінь додай:
   - `GameObjectContext`
   - `EnemyCarInstaller`
   - `EnemyCarView`

3. На об'єкт з колесами додай:
   - `EnemyCarWheelsView`
   - В поле **Wheels** перетягни трансформи коліс

4. В `EnemyCarInstaller` назнач:
   - **Config** → створений EnemyCarConfig
   - **Waypoints Parent** → об'єкт EnemyWaypoints
   - **Player Car Transform** → об'єкт машини гравця

5. В `GameObjectContext` → Mono Installers → додай `EnemyCarInstaller`

---

## Структура ворога-машини

```
EnemyCar
├── [GameObjectContext]
├── [EnemyCarInstaller]
├── [EnemyCarView]
├── Body (модель)
└── Wheels
    ├── [EnemyCarWheelsView]
    ├── WheelFL
    ├── WheelFR
    ├── WheelRL
    └── WheelRR
```

---

## Як тестувати

### Очікуваний результат:

1. **Запусти гру**
2. Ворог їде по waypoints
3. Колеса обертаються
4. Ворог тримається попереду гравця:
   - Якщо гравець наближається — ворог прискорюється
   - Якщо гравець відстає — ворог сповільнюється

### Перевірки:

| Тест | Очікування |
|------|------------|
| Ворог на сцені | Їде по waypoints |
| Колеса | Обертаються відповідно до швидкості |
| Гравець наближається | Ворог прискорюється |
| Гравець відстає | Ворог сповільнюється |
| Досягнув waypoint | Переходить до наступного |

### Якщо не працює:

| Проблема | Рішення |
|----------|---------|
| Ворог не рухається | Перевір waypoints та GameObjectContext |
| Колеса не крутяться | Перевір що колеса назначені в EnemyCarWheelsView |
| Помилка про PlayerCarTransform | Назнач машину гравця в EnemyCarInstaller |
| Ворог їде в стіну | Перевір позиції waypoints |

---

## Параметри EnemyCarConfig

### Health

| Параметр | Опис | Дефолт |
|----------|------|--------|
| Max Health | Максимальне здоров'я | 80 |

### Movement

| Параметр | Опис | Дефолт |
|----------|------|--------|
| Speed | Базова швидкість | 18 |
| Waypoint Reach Distance | Дистанція досягнення waypoint | 5 |

### Wheels

| Параметр | Опис | Дефолт |
|----------|------|--------|
| Wheel Rotation Speed | Швидкість обертання коліс | 360 |

### Stay Ahead

| Параметр | Опис | Дефолт |
|----------|------|--------|
| Min Distance From Player | Мінімальна дистанція (прискорення) | 15 |
| Max Distance From Player | Максимальна дистанція (сповільнення) | 40 |
| Speed Adjustment Rate | Швидкість зміни швидкості | 5 |

---

## Логіка "тримайся попереду"

Ворог автоматично підлаштовує швидкість:

- **Дистанція < 15м** → швидкість × 1.3 (тікає)
- **Дистанція 15-40м** → базова швидкість
- **Дистанція > 40м** → швидкість × 0.7 (чекає)

Це забезпечує що ворог завжди видно з лобового скла.
