# Итерация 1: Базовый враг — движение и лопасти

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Configs/EnemyHelicopterConfig.cs` | ScriptableObject с настройками движения и лопастей врага |
| `Scripts/Gameplay/Models/EnemyHelicopterModel.cs` | Модель с логикой движения по waypoints |
| `Scripts/Gameplay/Presenters/EnemyHelicopterPresenter.cs` | Presenter, обновляет модель каждый кадр |
| `Scripts/Gameplay/Installers/EnemyHelicopterInstaller.cs` | MonoInstaller для prefab врага |
| `Scripts/Views/Enemy/EnemyHelicopterView.cs` | Синхронизирует transform с моделью |
| `Scripts/Views/Enemy/EnemyHelicopterBladesView.cs` | Вращает лопасти |

---

## Как настроить

### Шаг 1: Скопировать файлы

Скопируй файлы в соответствующие папки проекта:
```
Assets/_Project/Scripts/
├── Configs/
│   └── EnemyHelicopterConfig.cs
├── Gameplay/
│   ├── Installers/
│   │   └── EnemyHelicopterInstaller.cs
│   ├── Models/
│   │   └── EnemyHelicopterModel.cs
│   └── Presenters/
│       └── EnemyHelicopterPresenter.cs
└── Views/
    └── Enemy/
        ├── EnemyHelicopterView.cs
        └── EnemyHelicopterBladesView.cs
```

### Шаг 2: Создать Config

1. В папке `Assets/_Project/Configs/` (или где удобно)
2. ПКМ → Create → LastConvoy → Configs → EnemyHelicopter
3. Назови `EnemyHelicopterConfig`
4. Настрой параметры по желанию (дефолтные уже рабочие)

### Шаг 3: Создать Waypoints для врага

1. Создай пустой GameObject на сцене, назови `EnemyWaypoints`
2. Создай дочерние пустые объекты (минимум 2), расставь их в воздухе там где враг должен летать
3. Нумерация: `Waypoint_0`, `Waypoint_1`, `Waypoint_2` и т.д.

### Шаг 4: Настроить Prefab врага

1. Возьми модель вражеского вертолёта
2. Добавь на **корневой объект**:
   - `GameObjectContext` (Zenject)
   - `EnemyHelicopterInstaller`
   - `EnemyHelicopterView`

3. В `EnemyHelicopterInstaller`:
   - **Config** — перетащи созданный `EnemyHelicopterConfig`
   - **Waypoints Parent** — перетащи `EnemyWaypoints` с твоими waypoints

4. На объект **лопастей** (main rotor) добавь:
   - `EnemyHelicopterBladesView`

5. Сохрани как Prefab

### Шаг 5: Разместить на сцене

1. Перетащи prefab врага на сцену
2. Позиция prefab'а не важна — враг телепортируется к первому waypoint при старте

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Вражеский вертолёт должен:
   - Появиться на позиции первого waypoint
   - Начать двигаться к следующему waypoint
   - Плавно поворачивать в сторону движения
   - Наклоняться (pitch/roll) при повороте
   - Слегка покачиваться (oscillation)
   - Лопасти вращаются

3. **Проверь:**
   - Враг летит по циклу waypoints (после последнего возвращается к первому)
   - Движение плавное, без рывков
   - Лопасти крутятся постоянно

### Если что-то не работает:

| Проблема | Решение |
|----------|---------|
| Враг не двигается | Проверь что GameObjectContext есть на prefab'е |
| Враг стоит на месте | Проверь что waypoints назначены в Installer |
| Лопасти не крутятся | Проверь что EnemyHelicopterBladesView на объекте лопастей |
| Ошибки в консоли | Читай текст ошибки — там написано что не назначено |

---

## Параметры EnemyHelicopterConfig

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Cruise Speed | Скорость полёта | 15 |
| Acceleration | Плавность набора скорости | 2 |
| Waypoint Reach Distance | Расстояние до waypoint для переключения | 10 |
| Yaw Speed | Скорость поворота | 0.6 |
| Banking Speed | Скорость наклона при повороте | 1.2 |
| Bank Return Speed | Скорость возврата из наклона | 0.8 |
| Max Pitch Angle | Максимальный наклон вперёд | 10 |
| Max Roll Angle | Максимальный боковой наклон | 18 |
| Blade Rotation Speed | Скорость вращения лопастей | 1800 |
