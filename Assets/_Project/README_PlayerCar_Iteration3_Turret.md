# Итерация 3: Турель — визуал и вращение

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Configs/TurretConfig.cs` | Настройки (углы, скорость вращения) |
| `Scripts/Gameplay/Models/TurretModel.cs` | Модель с текущими углами |
| `Scripts/Gameplay/Presenters/TurretPresenter.cs` | Обработка ввода мыши |
| `Scripts/Views/Player/TurretView.cs` | Вращение 3D модели |

### Изменённые файлы

| Путь | Что изменено |
|------|--------------|
| `Scripts/Gameplay/Installers/PlayerCarInstaller.cs` | Добавлены биндинги турели |

---

## Как настроить

### Шаг 1: Скопировать/заменить файлы

```
Assets/_Project/Scripts/
├── Configs/
│   └── TurretConfig.cs                 [ДОБАВИТЬ]
├── Gameplay/
│   ├── Installers/
│   │   └── PlayerCarInstaller.cs       [ЗАМЕНИТЬ]
│   ├── Models/
│   │   └── TurretModel.cs              [ДОБАВИТЬ]
│   └── Presenters/
│       └── TurretPresenter.cs          [ДОБАВИТЬ]
└── Views/
    └── Player/
        └── TurretView.cs               [ДОБАВИТЬ]
```

### Шаг 2: Создать Config

1. ПКМ → Create → LastConvoy → Configs → Turret
2. Назови `TurretConfig`
3. Настрой параметры (дефолтные уже рабочие)

### Шаг 3: Структура турели

Турель должна иметь два пивота для раздельного вращения:

```
PlayerCar
├── ...
└── TurretBase (на капоте)
    └── HorizontalPivot (вращается влево-вправо)
        └── VerticalPivot (вращается вверх-вниз)
            └── GunModel (3D модель пулемёта)
```

**Создание:**
1. Создай пустой объект `TurretBase` на капоте машины
2. Внутри создай пустой `HorizontalPivot`
3. Внутри него создай пустой `VerticalPivot`
4. Внутри положи 3D модель пулемёта

### Шаг 4: Добавить TurretView

1. На объект `TurretBase` добавь компонент `TurretView`
2. Назначь поля:
   - **Horizontal Pivot** → объект `HorizontalPivot`
   - **Vertical Pivot** → объект `VerticalPivot`

### Шаг 5: Обновить Installer

1. В `PlayerCarInstaller` появилось новое поле **Turret Config**
2. Перетащи созданный `TurretConfig`

---

## Структура машины (полная)

```
PlayerCar
├── [GameObjectContext]
├── [PlayerCarInstaller]
├── [PlayerCarView]
├── Body (модель машины)
├── CameraRig
│   ├── [PlayerCarCameraView]
│   └── Main Camera
└── TurretBase
    ├── [TurretView]
    └── HorizontalPivot
        └── VerticalPivot
            └── GunModel
```

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Двигай мышь влево-вправо → пулемёт поворачивается горизонтально
3. Двигай мышь вверх-вниз → пулемёт наклоняется вертикально
4. Достигни края лимита → пулемёт останавливается (не уходит за ±90° по горизонтали)

### Проверки:

| Тест | Ожидание |
|------|----------|
| Мышь влево | Пулемёт поворачивается влево |
| Мышь вправо | Пулемёт поворачивается вправо |
| Мышь вверх | Пулемёт поднимается |
| Мышь вниз | Пулемёт опускается |
| Крайний левый/правый | Стоп на ±90° |
| Крайний верх | Стоп на 30° вверх |
| Крайний низ | Стоп на 15° вниз |
| Резкое движение | Плавное следование (smoothing) |

### Если не работает:

| Проблема | Решение |
|----------|---------|
| Пулемёт не вращается | Проверь что Pivots назначены в TurretView |
| Вращается криво | Проверь что Pivot объекты имеют rotation (0,0,0) |
| Не реагирует на мышь | Проверь что TurretConfig назначен в Installer |
| Вращается слишком медленно | Увеличь Horizontal/Vertical Speed в Config |

---

## Параметры TurretConfig

### Rotation Speed

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Horizontal Speed | Скорость горизонтального вращения | 100 |
| Vertical Speed | Скорость вертикального вращения | 80 |

### Horizontal Limits

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Min Horizontal Angle | Левый лимит | -90 |
| Max Horizontal Angle | Правый лимит | 90 |

### Vertical Limits

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Min Vertical Angle | Нижний лимит (вниз) | -15 |
| Max Vertical Angle | Верхний лимит (вверх) | 30 |

### Smoothing

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Rotation Smoothing | Плавность вращения | 10 |

---

## Важно: Ориентация Pivot'ов

При создании Pivot объектов убедись что:
- Все rotation = (0, 0, 0)
- Forward (синяя стрелка в Unity) смотрит вперёд машины
- Up (зелёная стрелка) смотрит вверх

Иначе вращение будет работать неправильно.
