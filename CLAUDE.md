# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Текущее состояние проекта

Это **шаблон Unity URP с установленным DI-контейнером**, а не работающая кодовая база. Содержимое `Assets/`:

- `Assets/Scenes/SampleScene.unity` — стандартная сцена шаблона (Main Camera, Directional Light, Global Volume), единственная сцена в Build Settings;
- `Assets/Settings/` — три URP-профиля качества (`URP-Performant`, `URP-Balanced`, `URP-HighFidelity`) с соответствующими Renderer-ассетами;
- `Assets/UniversalRenderPipelineGlobalSettings.asset`;
- `Assets/Plugins/Zenject/` — DI-фреймворк (сторонний код, ~1.7 тыс. файлов); подробности в разделе «DI-контейнер» ниже.

**Собственного игрового кода пока нет** — все присутствующие `.cs` и `.asmdef` принадлежат Zenject. Название `MaterialAccumulationDemo` отражает замысел, но механика накопления материалов ещё не реализована — не предполагай наличие соответствующей архитектуры, её нужно спроектировать с нуля.

Следствие: `Assembly-CSharp.csproj` устарел — он содержит ссылку на удалённый `Assets\TutorialInfo\Scripts\Readme.cs`. Файлы `.csproj`/`.sln` генерируются Unity, править их вручную не нужно; они перегенерируются после добавления первого скрипта.

## Git: свой репозиторий, вложенный в чужой

**`git push` — только по явной команде пользователя.** Не отправляй изменения в `origin` по собственной инициативе: ни когда задача выглядит законченной, ни когда «логично сразу запушить», ни после успешной проверки. Локальные коммиты в рамках задачи допустимы, публикация — нет. Если считаешь, что пора отправлять, скажи об этом и дождись ответа. Команда — это прямая просьба вида «запушь», «отправь», «push»; согласие на пуш в одной задаче не переносится на следующую.

У проекта есть собственный репозиторий: **https://github.com/NickZubkov/MaterialAccumulationDemo** (приватный), ветка `main`, `origin` настроен, рабочее дерево отслеживает `origin/main`. В корне лежит `.gitignore` для Unity — `Library/`, `Temp/`, `Logs/`, `UserSettings/`, а также генерируемые `*.csproj`/`*.sln` исключены.

Отдельно исключена папка **`Docs/`** в корне проекта (правило `/Docs/`): там лежат документы и инструкции для внутреннего пользования. Её содержимое намеренно не попадает в репозиторий — на GitHub этих файлов нет. Читать их можно, но не предлагай их коммитить и не ссылайся на них из кода или документации как на общедоступные. Ведущий слэш ограничивает правило корнем, так что возможная папка `Docs/` внутри `Assets/` отслеживаться не перестанет.

Важная особенность: папка проекта физически лежит **внутри другого git-репозитория** — `D:\UnityProjects`, где рядом расположены десятки посторонних Unity-проектов (`Forest`, `CrazyPawns`, `Excavator` и др.). Это два независимых репозитория, вложенных друг в друга.

Из-за этого:

- Перед любой git-операцией убедись, в каком репозитории находишься: `git rev-parse --show-toplevel`. Из директории проекта должно возвращаться `D:/UnityProjects/MaterialAccumulationDemo`, а не `D:/UnityProjects`.
- Родительский репозиторий видит проект как одну untracked-запись `MaterialAccumulationDemo/` и внутрь не заходит. Добавлять её туда не нужно — это продублирует проект как gitlink.
- В родительском репозитории `.gitignore` по-прежнему отсутствует, а его `Library/`-папки весят гигабайты. Никогда не выполняй `git add .` или `git add -A`, находясь в `D:\UnityProjects`.

## DI-контейнер: Zenject (Extenject) 9.2.0

Установлен в `Assets/Plugins/Zenject/` из `.unitypackage`, **не через Package Manager** — в `Packages/manifest.json` его нет. Следствие: обновление только ручным переимпортом, и вся папка лежит в репозитории (~24 МБ).

Формально это **Extenject** — актуальный форк Zenject: `package.json` объявляет `com.mathijsbakker.extenject`. Namespace и имя папки остались `Zenject`, но при поиске документации и issue имеет смысл учитывать оба названия.

Сборки (asmdef), на которые можно ссылаться из своего кода:

| Сборка | Назначение |
|---|---|
| `Zenject` | рантайм-ядро (`Assets/Plugins/Zenject/zenject.asmdef`) |
| `Zenject-Editor` | редакторная часть, только Editor |
| `Zenject-TestFramework` | база для тестов с контейнером |

Собственный код по умолчанию попадает в `Assembly-CSharp`, которая видит `Zenject` автоматически. Но как только заведёшь свой asmdef, добавь в него ссылку на `Zenject` явно — иначе типы не разрешатся.

Точки входа фреймворка: `ProjectContext` (глобальный контейнер, префаб в `Resources`), `SceneContext` (контейнер сцены), `MonoInstaller` / `ScriptableObjectInstaller` (регистрация биндингов).

**Текущее состояние интеграции: нулевое.** В `SampleScene` нет `SceneContext`, инсталлеров не написано, `ProjectContext` не создан. Zenject лежит в проекте, но ни к чему не подключён — при добавлении первой механики контейнер нужно поднимать с нуля.

`OptionalExtras/` — примеры, интеграционные тесты, `AutoMocking`, `ReflectionBaking`, `MemoryPoolMonitor`. Составляют основную часть объёма папки и для работы ядра не требуются; если решишь удалить, проверь, что ничего не ссылается на `Zenject-TestFramework`.

## Конфигурация

| Параметр | Значение |
|---|---|
| Unity | 2022.3.62f3 (установлен: `C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe`) |
| Render Pipeline | URP 14.0.12 |
| C# / API level | C# 9.0, .NET Standard 2.1 |
| Input | легаси Input Manager (`activeInputHandler: 0`), не Input System |
| Целевая платформа | Standalone Windows x64 |

На машине стоят и другие версии редактора (2019.1.0f1, 2020.3.48f1, 2021.3.33f1, 2022.3.62f1/f2, 6000.3.19f1) — открывать проект нужно именно **2022.3.62f3**, иначе Unity предложит апгрейд и перезапишет ассеты.

Подключён `com.unity.test-framework` 1.1.33. Собственных тестов нет, но Zenject приносит свои сборки (`Zenject-UnitTests-Editor`, `Zenject-IntegrationTests`), поэтому Test Runner не пуст — не принимай эти тесты за тесты проекта. Для своих тестов создай отдельный asmdef со ссылками на `UnityEngine.TestRunner` / `UnityEditor.TestRunner` (Window → General → Test Runner → Create Test Assembly Folder).

## Команды

Unity не имеет отдельного шага «сборки» для разработки — компиляция происходит при открытии редактора. Всё нижеперечисленное запускается только при **закрытом редакторе** (иначе Unity упрётся в блокировку `Library/`).

Компиляция и выход (быстрая проверка, что код собирается):

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -nographics `
  -projectPath "D:\UnityProjects\MaterialAccumulationDemo" `
  -logFile "$env:TEMP\unity_compile.log"
```

Прогон тестов:

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -runTests -batchmode -nographics `
  -projectPath "D:\UnityProjects\MaterialAccumulationDemo" `
  -testPlatform EditMode `
  -testResults "$env:TEMP\results.xml" `
  -logFile "$env:TEMP\unity_tests.log"
```

`-testPlatform PlayMode` — для PlayMode-тестов. Один тест или группа: добавь `-testFilter "Namespace.Class.Method"` (принимает регулярное выражение). Batch-mode ничего не пишет в stdout — результаты читай из `-testResults` XML, ход выполнения из `-logFile`.

Сборка плеера требует статического C#-метода и ключа `-executeMethod`; готового build-скрипта в проекте нет.
