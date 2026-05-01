# Итерация 4: Падение и взрыв вражеского вертолёта

## Что добавлено

### Новые файлы

| Путь | Описание |
|------|----------|
| `Scripts/Views/Enemy/EnemyHelicopterCrashDetector.cs` | Детекция столкновения с землёй, спавн взрыва |

### Изменённые файлы

| Путь | Что изменено |
|------|--------------|
| `Scripts/Configs/EnemyHelicopterConfig.cs` | Добавлены параметры падения (Crash) и взрыва (Explosion Prefab) |
| `Scripts/Gameplay/Models/EnemyHelicopterModel.cs` | Добавлены состояния Flying/Crashing, логика падения, события OnCrashStarted и OnExploded |
| `Scripts/Gameplay/Presenters/EnemyHelicopterPresenter.cs` | Обработка состояния Crashing |
| `Scripts/Views/Enemy/EnemyHelicopterBladesView.cs` | Замедление лопастей при падении |

---

## Как настроить

### Шаг 1: Скопировать/заменить файлы

```
Assets/_Project/Scripts/
├── Configs/
│   └── EnemyHelicopterConfig.cs        [ЗАМЕНИТЬ]
├── Gameplay/
│   ├── Models/
│   │   └── EnemyHelicopterModel.cs     [ЗАМЕНИТЬ]
│   └── Presenters/
│       └── EnemyHelicopterPresenter.cs [ЗАМЕНИТЬ]
└── Views/
    └── Enemy/
        ├── EnemyHelicopterBladesView.cs    [ЗАМЕНИТЬ]
        └── EnemyHelicopterCrashDetector.cs [ДОБАВИТЬ]
```

### Шаг 2: Настроить Config

Открой `EnemyHelicopterConfig` asset и настрой новые поля:

**Crash (Падение):**

| Поле | Описание | Дефолт |
|------|----------|--------|
| Crash Fall Acceleration | Ускорение падения | 15 |
| Crash Max Fall Speed | Максимальная скорость падения | 30 |
| Crash Spin Speed | Скорость вращения вокруг оси (градусы/сек) | 180 |
| Crash Tilt Angle | Угол наклона при падении | 25 |
| Crash Tilt Speed | Скорость наклона | 2 |
| Crash Audio Fade Duration | Время затухания звука | 1.5 |

**Explosion (Взрыв):**

| Поле | Описание |
|------|----------|
| Explosion Prefab | Prefab партикла взрыва |

### Шаг 3: Добавить Rigidbody на врага

Для работы OnCollisionEnter нужен Rigidbody:

1. Открой prefab вражеского вертолёта
2. На корневой объект добавь **Rigidbody**
3. Настрой:
   - **Is Kinematic = true** (движение управляется кодом, не физикой)
   - **Use Gravity = false**

### Шаг 4: Добавить CrashDetector

1. На объект где есть Collider (тот же что Hitbox) добавь `EnemyHelicopterCrashDetector`
2. Или на корневой объект если коллайдер там

### Шаг 5: Создать prefab взрыва (если нет)

1. Создай prefab с ParticleSystem взрыва
2. Добавь звук взрыва (AudioSource с Play On Awake)
3. Добавь скрипт самоуничтожения через время (или используй ParticleSystem auto-destroy)
4. Назначь этот prefab в `EnemyHelicopterConfig → Explosion Prefab`

### Шаг 6: Настроить землю

Убедись что земля/terrain имеет Collider для детекции столкновения.

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти игру**
2. Стреляй по врагу пока здоровье не кончится
3. Когда HP = 0:
   - Вертолёт перестаёт лететь по маршруту
   - Начинает падать вниз
   - Вращается вокруг своей оси (tail spin)
   - Наклоняется
   - Лопасти замедляются
   - Звук ротора затухает
4. При контакте с землёй:
   - Появляется партикл взрыва
   - Вертолёт удаляется

### Проверки:

| Тест | Ожидание |
|------|----------|
| Убить врага | Начинает падать с вращением |
| Падение | Скорость падения увеличивается до максимума |
| Вращение | Вертолёт крутится вокруг вертикальной оси |
| Наклон | Вертолёт наклоняется на бок |
| Звук | Плавно затухает за 1.5 сек |
| Лопасти | Замедляются примерно до 30% скорости |
| Контакт с землёй | Взрыв + удаление вертолёта |

### Если не работает:

| Проблема | Решение |
|----------|---------|
| Вертолёт не падает | Проверь что здоровье дошло до 0 (Debug.Log в OnDied) |
| Не вращается при падении | Убедись что State меняется на Crashing |
| Нет столкновения с землёй | Добавь Rigidbody (Is Kinematic = true) на врага |
| Взрыв не появляется | Проверь что Explosion Prefab назначен в Config |
| Столкновение с самим собой | CrashDetector проверяет иерархию, должен игнорировать себя |
| Звук не затухает | Проверь что EnemyHelicopterAudioView на том же объекте или родителе |

---

## Физика падения

```
Состояние: Flying
    │
    ▼ [Health <= 0]
    │
Состояние: Crashing
    │
    ├─► Сохраняет горизонтальную инерцию (затухает 2%/кадр)
    ├─► Падает вниз с ускорением
    ├─► Вращается вокруг оси Y
    ├─► Наклоняется
    │
    ▼ [OnCollisionEnter]
    │
Взрыв + Destroy
```

---

## События EnemyHelicopterModel (обновлено)

| Событие | Когда вызывается |
|---------|------------------|
| `OnDamaged(float)` | При получении урона |
| `OnDied()` | Когда здоровье <= 0 |
| `OnCrashStarted()` | Когда начинается падение |
| `OnExploded(Vector3)` | При взрыве (позиция взрыва) |

---

## Параметры падения (рекомендации)

**Реалистичное падение:**
- Crash Fall Acceleration: 10-15
- Crash Spin Speed: 120-180
- Crash Tilt Angle: 20-30

**Быстрое аркадное падение:**
- Crash Fall Acceleration: 25-35
- Crash Spin Speed: 250-350
- Crash Tilt Angle: 35-45

**Медленное драматичное падение:**
- Crash Fall Acceleration: 5-8
- Crash Spin Speed: 80-120
- Crash Tilt Angle: 15-20
