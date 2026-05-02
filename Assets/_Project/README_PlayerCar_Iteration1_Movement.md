# Итерация 1: Машина игрока — движение

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Configs/PlayerCarConfig.cs` | ScriptableObject с настройками машины |
| `Scripts/Gameplay/Models/PlayerCarModel.cs` | Модель с логикой движения |
| `Scripts/Gameplay/Presenters/PlayerCarPresenter.cs` | Обработка WASD, обновление модели |
| `Scripts/Gameplay/Installers/PlayerCarInstaller.cs` | MonoInstaller для машины |
| `Scripts/Views/Player/PlayerCarView.cs` | Синхронизация transform |

### Изменённые файлы

| Путь | Что изменено |
|------|--------------|
| `Scripts/Services/Input/IInputService.cs` | Добавлены Vertical и Horizontal |
| `Scripts/Services/Input/InputService.cs` | Реализация Vertical и Horizontal |

---

## Как настроить

### Шаг 1: Скопировать/заменить файлы

```
Assets/_Project/Scripts/
├── Configs/
│   └── PlayerCarConfig.cs              [ДОБАВИТЬ]
├── Gameplay/
│   ├── Installers/
│   │   └── PlayerCarInstaller.cs       [ДОБАВИТЬ]
│   ├── Models/
│   │   └── PlayerCarModel.cs           [ДОБАВИТЬ]
│   └── Presenters/
│       └── PlayerCarPresenter.cs       [ДОБАВИТЬ]
├── Services/
│   └── Input/
│       ├── IInputService.cs            [ЗАМЕНИТЬ]
│       └── InputService.cs             [ЗАМЕНИТЬ]
└── Views/
    └── Player/
        └── PlayerCarView.cs            [ДОБАВИТЬ]
```

### Шаг 2: Создать Config

1. ПКМ → Create → LastConvoy → Configs → PlayerCar
2. Назови `PlayerCarConfig`
3. Настрой параметры (дефолтные уже рабочие)

### Шаг 3: Настроить машину на сцене

1. Создай/возьми 3D модель машины
2. На корневой объект добавь:
   - `GameObjectContext` (Zenject)
   - `PlayerCarInstaller`
   - `PlayerCarView`

3. В `PlayerCarInstaller`:
   - Перетащи созданный `PlayerCarConfig`

4. В `GameObjectContext` → Mono Installers:
   - Добавь `PlayerCarInstaller` (этот же объект)

### Шаг 4: Расположить машину

1. Поставь машину на сцену в нужное место
2. Убедись что под ней есть поверхность (terrain, plane)

---

## Управление

| Клавиша | Действие |
|---------|----------|
| W | Газ (вперёд) |
| S | Тормоз / Задний ход |
| A | Поворот влево |
| D | Поворот вправо |

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Нажми W — машина едет вперёд
3. Нажми S — машина тормозит, потом едет назад
4. A/D — машина поворачивает (только если едет)
5. Отпусти все клавиши — машина плавно останавливается

### Проверки:

| Тест | Ожидание |
|------|----------|
| W зажат | Машина ускоряется до максимума |
| S на скорости | Сначала тормозит, потом едет назад |
| A/D стоя на месте | Ничего (нужна скорость для поворота) |
| A/D на скорости | Машина поворачивает |
| Быстрая езда + поворот | Поворот менее резкий |
| Отпустить газ | Плавная остановка |

### Если не работает:

| Проблема | Решение |
|----------|---------|
| Машина не двигается | Проверь GameObjectContext и Installer |
| Ошибка про Transform | Проверь что Installer на том же объекте |
| Нет реакции на WASD | Проверь что GameplayState активен |
| Машина проваливается | Добавь Collider + Rigidbody (kinematic) |

---

## Параметры PlayerCarConfig

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Max Forward Speed | Максимальная скорость вперёд | 25 |
| Max Reverse Speed | Максимальная скорость назад | 10 |
| Acceleration | Ускорение | 15 |
| Brake Force | Сила торможения | 20 |
| Deceleration | Замедление без газа | 8 |
| Turn Speed | Скорость поворота (градусы/сек) | 80 |
| Turn Speed Reduction | Снижение поворота на скорости (0-1) | 0.3 |
| Min Speed To Turn | Минимальная скорость для поворота | 1 |

### Пояснения:

- **Turn Speed Reduction = 0.3** означает что на максимальной скорости поворот будет 70% от базового
- **Min Speed To Turn** — машина не поворачивает стоя на месте (как настоящая)

---

## Физика (упрощённая)

Машина использует "аркадную" физику:
- Нет Rigidbody физики
- Позиция и поворот управляются напрямую через Model
- При заднем ходе поворот инвертируется (как в реальности)
