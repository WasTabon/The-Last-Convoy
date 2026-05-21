# Итерация 4: Оружие турели — стрельба

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Configs/TurretWeaponConfig.cs` | Настройки (урон, скорострельность, звук, прицел) |
| `Scripts/Gameplay/Models/TurretWeaponModel.cs` | Модель стрельбы |
| `Scripts/Gameplay/Presenters/TurretWeaponPresenter.cs` | Raycast, урон |
| `Scripts/Views/Player/TurretWeaponView.cs` | Партиклы, звук |
| `Scripts/Views/Player/TurretCrosshairView.cs` | Прицел |

### Изменённые файлы

| Путь | Что изменено |
|------|--------------|
| `Scripts/Gameplay/Installers/PlayerCarInstaller.cs` | Добавлены биндинги оружия + Camera |

---

## Как настроить

### Шаг 1: Скопировать/заменить файлы

```
Assets/_Project/Scripts/
├── Configs/
│   └── TurretWeaponConfig.cs           [ДОБАВИТЬ]
├── Gameplay/
│   ├── Installers/
│   │   └── PlayerCarInstaller.cs       [ЗАМЕНИТЬ]
│   ├── Models/
│   │   └── TurretWeaponModel.cs        [ДОБАВИТЬ]
│   └── Presenters/
│       └── TurretWeaponPresenter.cs    [ДОБАВИТЬ]
└── Views/
    └── Player/
        ├── TurretWeaponView.cs         [ДОБАВИТЬ]
        └── TurretCrosshairView.cs      [ДОБАВИТЬ]
```

### Шаг 2: Создать Config

1. ПКМ → Create → LastConvoy → Configs → TurretWeapon
2. Назови `TurretWeaponConfig`
3. Назначь **Fire Clip** (звук выстрела)
4. Настрой параметры

### Шаг 3: Создать партикл дульной вспышки

1. Создай ParticleSystem для вспышки на дуле пулемёта
2. Или используй готовый prefab

### Шаг 4: Добавить TurretWeaponView

1. На объект турели (где TurretView) добавь `TurretWeaponView`
2. Назначь поля:
   - **Muzzle Flash** → ParticleSystem вспышки
   - **Muzzle Position** → Transform на конце ствола
   - **Impact Pool** → BulletImpactPool (тот же что для миниган или новый)

### Шаг 5: Добавить TurretCrosshairView

1. На любой объект машины добавь `TurretCrosshairView`
2. Zenject автоматически подтянет конфиг

### Шаг 6: Обновить Installer

1. В `PlayerCarInstaller` появились новые поля:
   - **Turret Weapon Config** → созданный конфиг
   - **Main Camera** → перетащи камеру (или оставь пустым — найдёт автоматически)

---

## Структура машины (полная)

```
PlayerCar
├── [GameObjectContext]
├── [PlayerCarInstaller]
├── [PlayerCarView]
├── [TurretCrosshairView]
├── Body
├── CameraRig
│   ├── [PlayerCarCameraView]
│   └── Main Camera
└── TurretBase
    ├── [TurretView]
    ├── [TurretWeaponView]
    └── HorizontalPivot
        └── VerticalPivot
            └── GunModel
                └── MuzzlePoint (пустой объект на конце ствола)
                    └── MuzzleFlash (ParticleSystem)
```

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Виден прицел в центре экрана
3. Зажми ЛКМ → пулемёт стреляет
4. При выстреле:
   - Вспышка на дуле
   - Звук выстрела
   - При попадании — искры (если есть Impact Pool)

### Проверки:

| Тест | Ожидание |
|------|----------|
| ЛКМ зажата | Стрельба с интервалом 0.2сек |
| ЛКМ отпущена | Стрельба прекращается |
| Прицел виден | Белый крестик в центре экрана |
| Попадание по врагу | Враг получает урон |
| Попадание по объекту | Искры (если Impact Pool) |

### Если не работает:

| Проблема | Решение |
|----------|---------|
| Нет звука | Проверь Fire Clip в конфиге |
| Нет вспышки | Проверь Muzzle Flash в TurretWeaponView |
| Не наносит урон | Проверь HitLayers в конфиге |
| Нет прицела | Проверь TurretCrosshairView на машине |
| Raycast не попадает | Проверь что камера правильно назначена |

---

## Параметры TurretWeaponConfig

### Firing

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Fire Rate | Интервал между выстрелами (сек) | 0.2 |
| Damage Per Shot | Урон за выстрел | 10 |
| Raycast Range | Дальность стрельбы | 300 |
| Hit Layers | Какие слои можно поразить | Everything |

### Audio

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Fire Clip | Звук выстрела | — |
| Fire Volume | Громкость | 0.8 |
| Fire Pitch Min/Max | Вариация питча | 0.95-1.05 |

### Crosshair

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Crosshair Color | Цвет прицела | Белый |
| Crosshair Size | Длина линий | 20 |
| Crosshair Thickness | Толщина линий | 2 |
| Crosshair Gap | Расстояние от центра | 8 |

---

## Impact Pool

Используй тот же `BulletImpactPool` что и для миниган. Если его нет на сцене:

1. Создай пустой объект `ImpactPool`
2. Добавь компонент `BulletImpactPool` (из namespace LastConvoy.Views.Effects)
3. Назначь prefab эффекта попадания
4. Перетащи в поле Impact Pool в TurretWeaponView
