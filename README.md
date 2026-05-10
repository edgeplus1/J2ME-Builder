# 📱 J2ME Builder

> **Релиз исходного кода** | Версия: 1.0.0.0 | Лицензия: MIT  
> ⚠️ **ВАЖНО:** Требуется строго **Java JDK 1.8.0** (другие версии не поддерживаются)

---

## 📋 Оглавление

1. [О проекте](#-о-проекте)
2. [⚠️ Критическое требование: Только Java 1.8.0](#-критическое-требование-только-java-180)
3. [📦 Источники для загрузки JDK 1.8.0](#-источники-для-загрузки-jdk-180)
4. [📋 Системные требования](#-системные-требования)
5. [🚀 Быстрый старт](#-быстрый-старт)
6. [📦 Установка и настройка](#-установка-и-настройка)
7. [🗂️ Структура проекта](#-структура-проекта)
8. [🎮 Использование](#-использование)
9. [🧩 Архитектура кода](#-архитектура-кода)
10. [🔧 Сборка из исходников](#-сборка-из-исходников)
11. [🐛 Устранение неполадок](#-устранение-неполадок)
12. [⚖️ Лицензия и правовая информация](#-лицензия-и-правовая-информация)
13. [🤝 Вклад в проект](#-вклад-в-проект)
14. [❓ Часто задаваемые вопросы](#-часто-задаваемые-вопросы)
15. [🙏 Благодарности](#-благодарности)

---

## 📖 О проекте

**J2ME Builder** — это графическая утилита с открытым исходным кодом для разработки, сборки и управления проектами под платформу Java 2 Micro Edition (J2ME/MIDP/CLDC).

### 💡 История создания

Идея создания программы возникла благодаря увлечению автора коллекционированием классических кнопочных телефонов и желанием писать для них собственные приложения. Инструмент призван упростить процесс разработки под устаревшую, но всё ещё интересную платформу.

### ✨ Возможности

- 🆕 **Создание проектов** из готового шаблона с правильной структурой папок
- 🔨 **Сборка JAR/JAD** с автоматической компиляцией, преверификацией и генерацией манифеста
- 📚 **Управление библиотеками** — подключение JSR-библиотек (Bluetooth, Media, Messaging)
- 🔄 **Список недавних проектов** с быстрым доступом и контекстным меню
- 🎨 **Поддержка иконок** — встраивание PNG-иконок в MIDlet
- 🌐 **Онлайн-установка JDK 8** — встроенный загрузчик OpenJDK через ojdkbuild
- 🔄 **Проверка обновлений** — автоматический поиск новых версий на GitHub
- 🖥️ **Интеграция с VS Code** — открытие проектов в современном редакторе
- 🌍 **Двуязычный интерфейс** — русский и английский языки

### 🎯 Для кого этот проект

- Энтузиасты ретро-разработки под J2ME
- Коллекционеры кнопочных телефонов (Nokia, Sony Ericsson, Samsung)
- Исследователи истории мобильной разработки
- Преподаватели, демонстрирующие эволюцию мобильных платформ

---

## ⚠️ Критическое требование: Только Java 1.8.0

### 🔴 Почему именно JDK 1.8.0?

| Компонент | Требование | Причина |
|-----------|-----------|---------|
| **javac** | `-source 1.3 -target 1.1` | JDK 9+ удалил поддержку генерации байт-кода версии 1.1, необходимого для CLDC |
| **preverify.exe** | Формат class ≤ JDK 1.4 | Утилита из Wireless Toolkit не распознаёт новый формат байт-кода |
| **bootclasspath** | Подмена rt.jar | В новых JDK механизм работает иначе и вызывает конфликты |
| **Устройства** | Проверка версии class | Реальные телефоны отвергают классы с версией > 45.3 (Java 1.1) |

### ❌ НЕ ПОДОЙДЁТ:
```
✗ Java 7 (jdk1.7.x) — устаревший, проблемы с кодировкой
✗ Java 11, 17, 21+ — ошибка: "invalid target release: 1.1"
✗ JRE (Java Runtime) — не содержит javac.exe, jar.exe
✗ Графические установщики с "обновлённой" версией
```

### ✅ ПОДОЙДЁТ:
```
✓ Oracle JDK 1.8.0_XXX
✓ Adoptium Temurin 8 — https://adoptium.net/
✓ Azul Zulu 8 — https://www.azul.com/downloads/
✓ Amazon Corretto 8 — https://aws.amazon.com/corretto/
✓ ojdkbuild (используется во встроенном загрузчике)
```

### 🔍 Проверка установки:
```cmd
> java -version
java version "1.8.0_XXX"

> javac -version
javac 1.8.0_XXX
```

> 📌 **Важно:** Обе команды должны показывать `1.8.0_XXX`. Если версии различаются — в PATH приоритет у другой установки.

---

## 📦 Источники для загрузки JDK 1.8.0

```text
# ============================================================================
# ОФИЦИАЛЬНЫЕ И ДОВЕРЕННЫЕ РЕПОЗИТОРИИ JDK 1.8.0
# ============================================================================

# Oracle JDK 8 (архив, требуется аккаунт Oracle)
https://www.oracle.com/java/technologies/javase/javase8-archive-downloads.html

# Adoptium Temurin 8 (открытая лицензия, РЕКОМЕНДУЕТСЯ)
https://adoptium.net/temurin/releases/?version=8
https://github.com/adoptium/temurin8-binaries/releases

# Azul Zulu 8
https://www.azul.com/downloads/?package=jdk#zulu
https://github.com/azul/zulu-builds/releases

# Amazon Corretto 8
https://docs.aws.amazon.com/corretto/latest/corretto-8-ug/downloads-list.html
https://github.com/corretto/corretto-8/releases

# ojdkbuild (используется во встроенном загрузчике Form5)
https://github.com/ojdkbuild/ojdkbuild/releases/tag/1.8.0.151-1
https://github.com/ojdkbuild/ojdkbuild/releases

# Liberica JDK 8 (BellSoft)
https://bell-sw.com/pages/downloads/#/java-8-lts
https://github.com/bell-sw/Liberica/releases

# Eclipse Foundation (исторические билды)
https://archive.eclipse.org/eclipse/downloads/

# ============================================================================
# ПРЯМЫЕ ССЫЛКИ НА WINDOWS x64 (ПРИМЕРЫ)
# ============================================================================

# ojdkbuild — MSI-установщик (используется в Form5)
https://github.com/ojdkbuild/ojdkbuild/releases/download/1.8.0.151-1/java-1.8.0-openjdk-1.8.0.151-1.b12.ojdkbuild.windows.x86_64.msi

# ojdkbuild — ZIP-архив (портативная версия)
https://github.com/ojdkbuild/ojdkbuild/releases/download/1.8.0.151-1/java-1.8.0-openjdk-1.8.0.151-1.b12.ojdkbuild.windows.x86_64.zip

# Adoptium Temurin 8 — MSI
https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u402-b06/OpenJDK8U-jdk_x64_windows_hotspot_8u402b06.msi

# Azul Zulu 8 — MSI
https://cdn.azul.com/zulu/bin/zulu8.76.0.17-ca-jdk8.0.392-win_x64.msi

# Amazon Corretto 8 — MSI
https://corretto.aws/downloads/resources/8.402.08/amazon-corretto-8.402.08.1-windows-x64-jdk.msi

# ============================================================================
# ПРОВЕРКА ЦЕЛОСТНОСТИ ФАЙЛА
# ============================================================================

# После скачивания проверьте хэш-сумму (если предоставлен издателем):
certutil -hashfile "jdk-8-ojdkbuild-x64.msi" SHA256

# Сравните вывод с официальным хэшем на странице релиза.
# Совпадение хэшей гарантирует, что файл не был изменён при загрузке.

# ============================================================================
# УСТАНОВКА В НЕСТАНДАРТНУЮ ПАПКУ
# ============================================================================

# Для MSI-установщиков (тихий режим с указанием пути):
msiexec /i "jdk-8-ojdkbuild-x64.msi" INSTALLDIR="C:\JDK8" /quiet

# Для ZIP-архивов (распаковка в нужную папку):
mkdir C:\JDK8
tar -xf "jdk-8-ojdkbuild-x64.zip" -C C:\JDK8 --strip-components=1

# Настройка переменных среды после установки:
setx JAVA_HOME "C:\JDK8"
setx PATH "%JAVA_HOME%\bin;%PATH%"

# ============================================================================
# ПРИМЕР КОДА: АВТОМАТИЧЕСКИЙ ПОИСК JDK 8 В C#
# ============================================================================

private string FindJDK8()
{
    // 1. Проверка переменной среды JAVA_HOME
    string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
    if (!string.IsNullOrEmpty(javaHome) && 
        File.Exists(Path.Combine(javaHome, "bin", "javac.exe")))
    {
        string versionOutput = RunCommand(Path.Combine(javaHome, "bin", "java.exe"), "-version");
        if (versionOutput.Contains("1.8.0"))
            return javaHome;
    }
    
    // 2. Стандартные пути установки
    string[] bases = { 
        @"C:\Program Files\Java", 
        @"C:\Program Files (x86)\Java",
        @"C:\JDK8",
        @"D:\Java"
    };
    
    foreach (var baseDir in bases)
    {
        if (!Directory.Exists(baseDir)) continue;
        
        var candidates = Directory.GetDirectories(baseDir, "jdk1.8*")
            .Concat(Directory.GetDirectories(baseDir, "jdk-8*"))
            .Concat(Directory.GetDirectories(baseDir, "zulu8*"))
            .Concat(Directory.GetDirectories(baseDir, "corretto8*"));
        
        foreach (var jdkDir in candidates)
        {
            if (File.Exists(Path.Combine(jdkDir, "bin", "javac.exe")))
                return jdkDir;
        }
    }
    
    return null;
}

private string RunCommand(string file, string args)
{
    var psi = new ProcessStartInfo(file, args)
    {
        UseShellExecute = false,
        RedirectStandardError = true,
        CreateNoWindow = true
    };
    using (var p = Process.Start(psi))
    {
        p.WaitForExit();
        return p.StandardError.ReadToEnd();
    }
}

# ============================================================================
# ВАЖНО: СОВМЕСТИМОСТЬ С J2ME
# ============================================================================
# При использовании сторонних сборок убедитесь, что они поддерживают флаги:
#   -source 1.3 -target 1.1
# Все перечисленные выше источники предоставляют совместимые сборки.
```

---

## 📋 Системные требования

### Программное обеспечение:
| Компонент | Версия | Статус | Примечание |
|-----------|--------|--------|------------|
| **ОС** | Windows 7/8/10/11 (x86/x64) | ✅ Обязательно | WinForms приложение |
| **.NET Framework** | 4.7.2 или выше | ✅ Обязательно | Проверка: `reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"` |
| **Java JDK** | **1.8.0_XXX (строго!)** | 🔴 КРИТИЧНО | Должен содержать `bin\javac.exe`, `bin\jar.exe` |
| **Visual Studio Code** | 1.60+ | ⚪ Рекомендуется | Для редактирования кода |

### Файлы ресурсов (`res/`):
```
📁 J2MEBuilder/
 ├── 📁 res/
 │   ├── midpapi20.jar          # ✅ MIDP 2.0 API — обязательно
 │   ├── cldcapi11.jar          # ✅ CLDC 1.1 API — обязательно
 │   ├── preverify.exe          # ✅ Утилита преверификации — обязательно
 │   ├── build40.bat            # ⚪ Маркер проекта — опционально
 │   ├── jsr82.jar              # ⚪ Bluetooth API — опционально
 │   ├── mmapi.jar              # ⚪ Media API — опционально
 │   └── ...                    # Другие совместимые библиотеки
 │
 ├── 📁 src/                    # Исходный код C# проекта
 ├── J2MEBuilder.sln            # Решение Visual Studio
 ├── README.md                  # Этот файл
 └── LICENSE                    # Лицензия MIT
```

---

## 🚀 Быстрый старт

### Для пользователей (готовая сборка):

1. **Скачайте** релиз из [Releases](../../releases)
2. **Распакуйте** в удобную папку, например `C:\Apps\J2MEBuilder\`
3. **Убедитесь**, что установлен **JDK 1.8.0** (или используйте встроенный загрузчик)
4. **Запустите** `J2MEBuilder.exe`
5. **Нажмите** «Создать проект» → введите имя → выберите папку → «Создать»
6. **Редактируйте** код в открывшемся VS Code
7. **Вернитесь** в Builder → «Собрать проект» → укажите версию → «Собрать»
8. **Готово!** Файлы `.jar` и `.jad` в папке `build/{Тип}/v{Версия}/`

### Для разработчиков (сборка из исходников):

```cmd
:: 1. Клонируйте репозиторий
git clone https://github.com/edgeplus1/J2ME-Builder.git
cd J2ME-Builder

:: 2. Откройте решение в Visual Studio 2019/2022
J2MEBuilder.sln

:: 3. Установите пакеты NuGet:
   - Newtonsoft.Json
   - RestSharp (опционально)

:: 4. Убедитесь, что целевая платформа: .NET Framework 4.7.2

:: 5. Соберите в конфигурации Release
::    Меню: Сборка → Сборка решения (Ctrl+Shift+B)

:: 6. Скопируйте папку res/ в выходную директорию (bin\Release\)

:: 7. Запустите и протестируйте
```

---

## 📦 Установка и настройка

### Шаг 1: Установка JDK 1.8.0

#### Вариант A: Через встроенный загрузчик (рекомендуется)

В окне «О программе» нажмите **«Установить JDK 8»**:
- Программа автоматически скачает ojdkbuild для Windows x64
- Запустится стандартный установщик MSI
- После установки перезапустите J2ME Builder

#### Вариант B: Вручную

```cmd
1. Скачайте JDK 8 с одного из источников (см. раздел выше)

2. Установите, запомните путь (например, C:\Program Files\Java\jdk1.8.0_XXX)

3. Настройте переменные среды:
   - Создайте/отредактируйте: JAVA_HOME = C:\Program Files\Java\jdk1.8.0_XXX
   - Добавьте в Path: %JAVA_HOME%\bin

4. Откройте НОВОЕ окно cmd и проверьте:
   > javac -version
   javac 1.8.0_XXX
```

### Шаг 2: Подготовка папки `res/`

```cmd
:: Создайте структуру рядом с J2MEBuilder.exe:
cd C:\Apps\J2MEBuilder
mkdir res

:: Скопируйте обязательные файлы:
copy "C:\WTK2.5.2\lib\midpapi20.jar" "res\"
copy "C:\WTK2.5.2\lib\cldcapi11.jar" "res\"
copy "C:\WTK2.5.2\bin\preverify.exe" "res\"

:: (Опционально) дополнительные библиотеки:
copy "C:\WTK2.5.2\lib\jsr82.jar" "res\"
copy "C:\WTK2.5.2\lib\mmapi.jar" "res\"
```

> 💡 **Где взять файлы?**  
> Установите [Sun Java Wireless Toolkit 2.5.2](https://www.oracle.com/java/technologies/j2me-downloads.html) или извлеките из любого готового J2ME-проекта.

### Шаг 3: Проверка работоспособности

```cmd
:: Запустите J2MEBuilder.exe
:: Выполните тестовый сценарий:

1. "🆕 Создать проект" → имя: TestApp → папка: Документы → "Создать"
2. Откроется VS Code с шаблоном MIDlet
3. Вернитесь в Builder → "🔨 Собрать проект"
4. Выберите папку проекта → версия: 1.0.0 → "Собрать"

:: Ожидаемый результат в логе:
[✓] Запуск сборщика J2ME...
[✓] [1/6] Поиск JDK 8...
[✓]     Найден: C:\Program Files\Java\jdk1.8.0_XXX
[✓] [3/6] Компиляция Java файлов...
[✓] [4/6] Преверификация (preverify.exe)...
[✓] [6/6] Создание JAR и JAD...
[✓] СБОРКА УСПЕШНА!

:: Откроется папка с результатом:
build/Release/v1.0.0/
├── TestApp_v1.0.0.jar   ← Готовое приложение
└── TestApp_v1.0.0.jad   ← Дескриптор для OTA-установки
```

---

## 🗂️ Структура проекта

### Ожидаемая структура каталогов:

```
📁 MyJ2MEProject/
 │
 ├── 📁 src/                              # Исходный код Java (ОБЯЗАТЕЛЬНО)
 │   │
 │   └── 📁 com/
 │       └── 📁 mycompany/
 │           ├── 📄 Main.java            # Точка входа, наследник MIDlet
 │           ├── 📄 GameCanvas.java      # Игровой экран
 │           └── 📄 /res/                # Ресурсы внутри пакета
 │               ├── 🖼️ logo.png
 │               └── 🎵 sound.wav
 │
 ├── 📁 lib/                              # Внешние библиотеки .jar (ОПЦИОНАЛЬНО)
 │   ├── 📦 jsr82.jar                    # Bluetooth API
 │   └── 📦 custom-lib.jar               # Пользовательская библиотека
 │
 ├── 📁 build/                            # Выходные файлы (СОЗДАЁТСЯ АВТОМАТИЧЕСКИ)
 │   ├── 📁 Release/
 │   │   └── 📁 v1.0.0/
 │   │       ├── 📦 MyApp_v1.0.0.jar
 │   │       └── 📄 MyApp_v1.0.0.jad
 │   └── 📁 Debug/
 │
 ├── 📄 build40.bat                       # Маркер проекта (ОПЦИОНАЛЬНО)
 ├── 📁 .vscode/                          # Настройки редактора
 │   └── 📄 settings.json
 ├── 📄 README.md
 └── 📄 LICENSE
```

### Алгоритм распознавания J2ME-проекта (`IsJ2MEProject`):

```csharp
private bool IsJ2MEProject(string path)
{
    bool hasSrc = Directory.Exists(Path.Combine(path, "src"));
    bool hasJavaFiles = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories).Length > 0;
    bool hasLib = Directory.Exists(Path.Combine(path, "lib"));
    bool hasBuildScript = File.Exists(Path.Combine(path, "build40.bat"));

    // Достаточно выполнения хотя бы одного условия:
    if (hasSrc && hasJavaFiles) return true;   // Есть исходники
    if (hasBuildScript) return true;           // Есть скрипт сборки
    if (hasLib && hasSrc) return true;         // Есть lib+src

    return false;
}
```

---

## 🎮 Использование

### 🏠 Главная форма (Form1)

| Кнопка | Действие | Описание |
|--------|----------|----------|
| 🆕 Создать проект | Открывает Form3 | Создание нового проекта из шаблона |
| 🔨 Собрать проект | Открывает Form2 | Компиляция и упаковка существующего проекта |
| 📂 Открыть проект | Диалог выбора папки | Открытие внешнего проекта в VS Code |
| 📋 Список недавних | listBox1 | Быстрый доступ к последним проектам |
| ℹ️ О программе | Открывает Form4 | Информация, проверка обновлений, установка JDK |

#### Работа со списком недавних проектов:
- **Двойной клик** → открыть проект
- **ПКМ** → контекстное меню:
  - «Удалить из списка» — убрать выбранный проект
  - «Очистить весь список» — удалить все записи

### 🔨 Форма сборки (Form2)

#### Пошаговый процесс:

```
1️⃣ Выбор папки проекта
   → Проверка на валидность через IsJ2MEProject()

2️⃣ Настройка параметров
   → Версия, производитель, тип сборки (Release/Beta/Debug)
   → Иконка приложения (опционально, PNG)

3️⃣ Запуск сборки (кнопка "Собрать")
   → Асинхронное выполнение в фоновом потоке
   → Прогресс-бар и лог в реальном времени

4️⃣ Этапы сборки:
   [1/6] Поиск JDK 8
   [2/6] Подготовка директорий (build/{Type}/v{Version}/)
   [2.5/6] Анализ библиотек из lib/
   [3/6] Компиляция .java через javac с флагами:
         -source 1.3 -target 1.1 -bootclasspath midp;cldc
   [4/6] Преверификация через preverify.exe
   [5/6] Копирование ресурсов (не-.java файлы + иконка)
   [6/6] Создание JAR через jar.exe + генерация JAD

5️⃣ Завершение
   → Открытие папки с результатом
   → Сообщение об успехе или ошибке
```

#### Ключевые параметры компиляции:

```cmd
javac.exe ^
  -bootclasspath "res\midpapi20.jar;res\cldcapi11.jar" ^
  -classpath "lib\custom1.jar;lib\custom2.jar" ^
  -d "build\Release\v1.0.0\classes" ^
  -encoding UTF-8 ^
  -source 1.3 ^
  -target 1.1 ^
  -g:none ^
  "src\com\myapp\Main.java"
```

| Флаг | Значение | Зачем нужен |
|------|----------|-------------|
| `-bootclasspath` | midpapi20.jar;cldcapi11.jar | Подменяет rt.jar на J2ME API |
| `-classpath` | Пути к пользовательским .jar | Подключение сторонних библиотек |
| `-source 1.3` | Версия синтаксиса | Только конструкции Java 1.3 |
| `-target 1.1` | Версия байт-кода | Совместимость с виртуальной машиной CLDC |
| `-encoding UTF-8` | Кодировка исходников | Корректная обработка кириллицы |
| `-g:none` | Без отладочной информации | Уменьшение размера JAR |

### 🆕 Мастер создания проекта (Form3)

#### Что создаётся:

```
1. Структура папок:
   src/com/{CleanName}/
   lib/
   .vscode/

2. Шаблон Java-класса (UTF-8 без BOM):
   package com.{className};
   import javax.microedition.midlet.*;
   import javax.microedition.lcdui.*;
   
   public class {className} extends MIDlet implements CommandListener {
       // ... базовая реализация MIDlet ...
   }

3. Настройки VS Code:
   {
       "java.project.referencedLibraries": ["lib/*.jar"],
       "java.validation.enabled": false,
       "java.errors.incompleteClasspath.severity": "ignore"
   }

4. Инструменты сборки:
   preverify.exe, build40.bat → в корень проекта

5. Выбранные библиотеки → в lib/
```

#### Почему без BOM?

```
BOM (Byte Order Mark) — три байта EF BB BF в начале UTF-8 файла.

Проблемы с BOM в J2ME:
❌ Некоторые эмуляторы не распознают файлы с BOM
❌ Старые телефоны могут не загружать приложение
❌ preverify.exe может некорректно обрабатывать BOM

Решение: new UTF8Encoding(false)  // false = не добавлять BOM
```

### ℹ️ Форма "О программе" (Form4)

| Функция | Описание |
|---------|----------|
| 📋 Информация | Автор, версия, лицензия, правовые уведомления |
| 🔄 Проверка обновлений | Запрос к GitHub API, сравнение версий |
| 📥 Установить JDK 8 | Встроенный загрузчик ojdkbuild |
| 🔗 Документация | Ссылка на документацию |
| 🌐 Исходный код | Ссылка на репозиторий GitHub |

### 📥 Форма загрузки JDK (Form5)

#### Алгоритм работы:

```
1. Пользователь нажимает "Скачать JDK 8"
2. WebClient загружает MSI-файл с ojdkbuild через GitHub
3. Прогресс-бар и лог обновляются в реальном времени
4. По завершении: диалог "Запустить установщик?"
   → Да: запуск msiexec.exe /i с правами администратора
   → Нет: файл остаётся в %TEMP% для ручного запуска
5. Обработка ошибок: сеть, права доступа, отмена пользователем
```

---

## 🧩 Архитектура кода

### 📁 Структура решения:

```
J2MEBuilder.sln
│
├── 📄 Form1.cs          # Главная форма: навигация, список проектов
├── 📄 Form2.cs          # Сборщик: компиляция, преверификация, упаковка
├── 📄 Form3.cs          # Мастер: создание проекта из шаблона
├── 📄 Form4.cs          # О программе: информация, обновления, установка JDK
├── 📄 Form5.cs          # Загрузчик JDK: скачивание ojdkbuild
│
├── 📄 Program.cs        # Точка входа: Application.Run(new Form1())
├── 📁 Properties/
│   ├── AssemblyInfo.cs  # Метаданные сборки
│   ├── Resources.resx   # Строки и изображения
│   └── Settings.settings # Настройки (language и др.)
│
├── 📁 res/              # Ресурсы: API, инструменты, библиотеки
├── 📁 .vscode/          # Настройки редактора для разработки самого Builder
│
├── 📄 README.md         # Документация
├── 📄 LICENSE           # Лицензия MIT
└── 📄 .gitignore        # Исключения для Git
```

### 🔑 Ключевые паттерны:

#### 1. Асинхронная сборка без блокировки UI:
```csharp
private async void button3_Click(object sender, EventArgs e)
{
    string version = textBox2.Text.Trim();
    string vendor = textBox4.Text.Trim();
    
    button3.Enabled = false;
    SetProgress(0);
    
    await Task.Run(() => BuildProcess(version, vendor, releaseType));
    
    button3.Enabled = true;
}
```

#### 2. Потокобезопасное обновление UI:
```csharp
private void Log(string msg)
{
    Action append = () => {
        richTextBox1.AppendText(msg + Environment.NewLine);
        richTextBox1.ScrollToCaret();
    };
    
    if (richTextBox1.InvokeRequired)
        richTextBox1.Invoke(append);
    else
        append();
}
```

#### 3. Запуск внешних процессов:
```csharp
private int RunProcessSync(string file, string args, out string error)
{
    var psi = new ProcessStartInfo(file, args)
    {
        UseShellExecute = false,
        RedirectStandardError = true,
        CreateNoWindow = true
    };
    
    using (var p = Process.Start(psi))
    {
        error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        return p.ExitCode;
    }
}
```

#### 4. Поиск JDK 8:
```csharp
private string FindJDK8()
{
    string[] bases = { 
        @"C:\Program Files\Java", 
        @"C:\Program Files (x86)\Java" 
    };
    
    foreach (var b in bases)
    {
        if (!Directory.Exists(b)) continue;
        
        var dirs = Directory.GetDirectories(b, "jdk1.8*")
                      .Concat(Directory.GetDirectories(b, "jdk-8*"));
        foreach (var d in dirs)
            if (File.Exists(Path.Combine(d, "bin", "javac.exe"))) 
                return d;
    }
    return null;
}
```

---

## 🔧 Сборка из исходников

### Требования для разработки:

| Компонент | Версия | Примечание |
|-----------|--------|------------|
| **Visual Studio** | 2019/2022 | Community Edition достаточно |
| **.NET Framework** | 4.7.2 SDK | Устанавливается с VS |
| **NuGet пакеты** | Newtonsoft.Json, RestSharp | Установить через Package Manager |
| **Git** | Любая | Для клонирования репозитория |

### Пошаговая инструкция:

```cmd
:: 1. Клонирование
git clone https://github.com/edgeplus1/J2ME-Builder.git
cd J2ME-Builder

:: 2. Открытие решения
::    Двойной клик: J2MEBuilder.sln

:: 3. Восстановление пакетов NuGet:
::    Tools → NuGet Package Manager → Manage NuGet Packages for Solution
::    Установить:
::    - Newtonsoft.Json
::    - RestSharp (для Form4, опционально)

:: 4. Проверка целевой платформы:
::    ПКМ по проекту → Properties → Application → Target framework: .NET Framework 4.7.2

:: 5. Сборка:
::    Меню: Build → Build Solution (Ctrl+Shift+B)
::    Конфигурация: Release
::    Платформа: Any CPU

:: 6. Подготовка ресурсов:
::    Скопировать папку res/ из корня репозитория в:
::    bin\Release\res\

:: 7. Запуск:
::    bin\Release\J2MEBuilder.exe
```

### Сборка релизного пакета:

```cmd
:: 1. Соберите в Release
:: 2. Создайте папку для дистрибутива:
mkdir J2MEBuilder_v1.0_Source

:: 3. Скопируйте:
::    - Все файлы из корня репозитория (кроме bin/, obj/, .git/)
::    - Папку res/ с обязательными файлами
::    - Файлы: README.md, LICENSE

:: 4. Создайте архив и опубликуйте в GitHub Releases с тегом: v1.0
```

---

## 🐛 Устранение неполадок

### ❌ "JDK 8 не найден! Проверьте установку."

**Диагностика:**
```cmd
> where javac
> javac -version
> echo %JAVA_HOME%
```

**Решение:**
- Установите JDK 1.8.0 (не JRE!)
- Настройте JAVA_HOME и PATH
- Или используйте встроенный загрузчик в окне "О программе"

### ❌ "invalid target release: 1.1"

**Причина:** Используется JDK 9+ вместо 1.8.0

**Решение:**
- Убедитесь, что в PATH приоритет у JDK 8
- Временно: `set JAVA_HOME=C:\Program Files\Java\jdk1.8.0_XXX`

### ❌ "preverify.exe не найден в папке res/"

**Решение:**
- Скопируйте preverify.exe из WTK 2.5.2 в папку res/
- Или скачайте из доверенного источника

### ❌ Прогресс загрузки не двигается

**Возможные причины:**
- Брандмауэр блокирует доступ к GitHub
- Антивирус блокирует .msi файлы
- Нет подключения к интернету

**Решение:**
- Проверьте доступ к ссылке на JDK в браузере
- Добавьте исключения в антивирус
- Попробуйте скачать JDK вручную и установить

### ❌ Не сохраняется список недавних проектов

**Причина:** Нет прав на запись в папку с .exe

**Решение:**
- Запустите от имени администратора
- Или переместите J2MEBuilder в папку с правами записи: `%USERPROFILE%\Apps\J2MEBuilder\`

### ❌ VS Code не открывается

**Решение:**
- Установите расширение "code" в PATH из VS Code (Ctrl+Shift+P → "shell command")
- Или отредактируйте OpenProjectInVSCode() с вашим путём к Code.exe

---

## ⚖️ Лицензия и правовая информация

### 🔹 О платформе Java ME

Платформа **Java ME (Java Platform, Micro Edition)**, включая спецификации MIDP, CLDC, а также торговые марки `Java`, `J2ME`, `MIDlet`, `Wireless Toolkit` и связанные логотипы, являются собственностью компании **Oracle Corporation** (ранее принадлежали Sun Microsystems). Все права защищены.

Данный проект **не аффилирован**, не спонсируется и не одобрен Oracle Corporation или любыми другими производителями мобильных устройств.

### 🔹 Лицензия на J2ME Builder

```
MIT License

Copyright (c) 2026 Edge+ Ramzayev Hermann

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### 🔹 Сторонние компоненты

| Компонент | Правообладатель | Лицензия |
|-----------|----------------|----------|
| `midpapi20.jar`, `cldcapi11.jar` | Oracle / Sun Microsystems | Oracle Technology Network License |
| `preverify.exe` | Oracle / Sun Microsystems | Wireless Toolkit License |
| ojdkbuild | ojdkbuild project | Apache 2.0 / GPL v2 |
| Newtonsoft.Json | James Newton-King | MIT |
| RestSharp | RestSharp Community | Apache 2.0 |

> ⚠️ **Важно:** При распространении готовых приложений убедитесь, что использование встроенных API-библиотек соответствует их лицензионным ограничениям.

---

## 🤝 Вклад в проект

### 🐛 Сообщить об ошибке:
1. Убедитесь, что ошибка воспроизводится с последней версией
2. Проверьте раздел [Устранение неполадок](#-устранение-неполадок)
3. Создайте [новый Issue](../../issues/new) с меткой `bug`
4. Укажите: ОС, версию JDK, шаги воспроизведения, лог ошибки

### 💡 Предложить улучшение:
1. Создайте Issue с меткой `enhancement` или обсудите в [Discussions](../../discussions)
2. Опишите идею, преимущества и возможную реализацию
3. Если готовы реализовать — создайте форк и отправьте Pull Request

### 📝 Правила для Pull Request:
```
✅ Обязательно:
- Код соответствует стилю проекта (отступы 4 пробела, PascalCase/camelCase)
- Все изменения протестированы вручную
- Добавлены комментарии к изменённому коду
- Обновлён README при изменении требований

✅ Желательно:
- Добавлены юнит-тесты для новой логики
- Обновлены примеры в документации

❌ Не принимается:
- Изменения, ломающие обратную совместимость без обсуждения
- Удаление требования JDK 1.8.0 без веской причины
- Изменения, не прошедшие тестирование
```

---

## ❓ Часто задаваемые вопросы

### 🤔 Почему нельзя использовать более новую версию Java?

**Короткий ответ:** Виртуальная машина старых телефонов (CLDC 1.1) понимает только байт-код версии 1.1, который могут генерировать только компиляторы до JDK 8 включительно.

**Подробно:** При компиляции `javac -target 1.1` генерирует class-файл с `major version: 45`. ВМ на телефоне проверяет эту версию при загрузке. Если версия выше (например, 55 для Java 11), загрузка прерывается с ошибкой `Unsupported class version`. Начиная с JDK 9, `javac` удалил поддержку `-target` ниже 1.6.

### 🤔 Можно ли собрать приложение для современного Android?

**Нет.** J2ME и Android — разные платформы с несовместимыми ВМ, API и форматами пакетов.

**Альтернативы:**
- Для запуска старых игр на Android: [J2ME Loader](https://github.com/nikita36078/J2ME-Loader)
- Для портирования логики: переписать код на Java для Android
- Для кроссплатформенной разработки: [Codename One](https://www.codenameone.com/)

### 🤔 Как отлаживать приложение на реальном устройстве?

```
1. Передайте .jad + .jar на устройство (через USB, карту памяти, Bluetooth или OTA)
2. На телефоне откройте файловый менеджер и запустите .jad-файл
3. Для отладки: выводите логи через System.out.println() 
   (на некоторых устройствах попадают в системный лог)
4. Используйте визуальные подсказки: текст на экране, вибрация

⚠️ Предупреждение: Не все эмуляторы точно воспроизводят поведение реальных устройств.
```

### 🤔 Как добавить поддержку новых библиотек (JSR)?

```
1. Получите библиотеку (официальные спецификации: https://jcp.org/en/jsr/table)
2. Поместите .jar в папку res/ рядом с J2MEBuilder.exe
3. Обновите список knownLibs в Form2.cs для авто-обнаружения
4. Протестируйте: создайте проект с новой библиотекой и убедитесь, 
   что компиляция и преверификация проходят без ошибок
```

---

## 🙏 Благодарности

```
🙇 Спасибо следующим проектам и сообществам:

🔹 KEmulator
— Легковесный эмулятор J2ME для отладки приложений, интегрированный в процесс разработки

🔹 J2ME Loader (https://github.com/nikita36078/J2ME-Loader)
   — Современный эмулятор для Android, сохраняющий наследие платформы

🔹 Wireless Toolkit 2.5.2 (Oracle/Sun Microsystems)
   — Официальный набор инструментов, источник preverify.exe и API

🔹 ojdkbuild (https://github.com/ojdkbuild/ojdkbuild)
   — Открытые сборки OpenJDK для Windows

🔹 Сообщество ретро-разработчиков
   — За сохранение интереса к классической мобильной разработке

🔹 Вы, пользователь
   — За интерес к платформе, которая заложила основы мобильной индустрии
```

---

> 🎮 *J2ME Builder создан с уважением к эпохе, когда 2 МБ памяти было много,  
> а игры распространялись через WAP-порталы по 50 рублей за СМС.*  
>  
> ☕ *Требует строгого соблюдения: Java 1.8.0 — ни больше, ни меньше.*  
>  
> 🔖 **Релиз исходного кода: `v1.0.0.0`**

*Последнее обновление документации: Май 2026*  
*Совместимость проверена для: J2ME Builder v1.0.0.0, JDK 1.8.0_XXX, Windows 10 x64*

---

**Автор:** Edge+ Ramzayev Hermann  
**Репозиторий:** https://github.com/edgeplus1/J2ME-Builder  
**Лицензия:** MIT (см. файл LICENSE)
