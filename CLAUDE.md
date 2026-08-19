# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Текущее состояние проекта

Это **пустой шаблон Unity URP**, а не работающая кодовая база. На данный момент в `Assets/` лежат только:

- `Assets/Scenes/SampleScene.unity` — стандартная сцена шаблона (Main Camera, Directional Light, Global Volume), единственная сцена в Build Settings;
- `Assets/Settings/` — три URP-профиля качества (`URP-Performant`, `URP-Balanced`, `URP-HighFidelity`) с соответствующими Renderer-ассетами;
- `Assets/UniversalRenderPipelineGlobalSettings.asset`.

**Скриптов (`.cs`), asmdef-файлов и тестов в проекте нет.** Название `MaterialAccumulationDemo` отражает замысел, но механика накопления материалов ещё не реализована — не предполагай наличие соответствующей архитектуры, её нужно спроектировать с нуля.

Следствие: `Assembly-CSharp.csproj` устарел — он содержит ссылку на удалённый `Assets\TutorialInfo\Scripts\Readme.cs`. Файлы `.csproj`/`.sln` генерируются Unity, править их вручную не нужно; они перегенерируются после добавления первого скрипта.

## Git: свой репозиторий, вложенный в чужой

У проекта есть собственный репозиторий: **https://github.com/NickZubkov/MaterialAccumulationDemo** (приватный), ветка `main`, `origin` настроен, рабочее дерево отслеживает `origin/main`. В корне лежит `.gitignore` для Unity — `Library/`, `Temp/`, `Logs/`, `UserSettings/`, а также генерируемые `*.csproj`/`*.sln` исключены.

Важная особенность: папка проекта физически лежит **внутри другого git-репозитория** — `D:\UnityProjects`, где рядом расположены десятки посторонних Unity-проектов (`Forest`, `CrazyPawns`, `Excavator` и др.). Это два независимых репозитория, вложенных друг в друга.

Из-за этого:

- Перед любой git-операцией убедись, в каком репозитории находишься: `git rev-parse --show-toplevel`. Из директории проекта должно возвращаться `D:/UnityProjects/MaterialAccumulationDemo`, а не `D:/UnityProjects`.
- Родительский репозиторий видит проект как одну untracked-запись `MaterialAccumulationDemo/` и внутрь не заходит. Добавлять её туда не нужно — это продублирует проект как gitlink.
- В родительском репозитории `.gitignore` по-прежнему отсутствует, а его `Library/`-папки весят гигабайты. Никогда не выполняй `git add .` или `git add -A`, находясь в `D:\UnityProjects`.

## Конфигурация

| Параметр | Значение |
|---|---|
| Unity | 2022.3.62f3 (установлен: `C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe`) |
| Render Pipeline | URP 14.0.12 |
| C# / API level | C# 9.0, .NET Standard 2.1 |
| Input | легаси Input Manager (`activeInputHandler: 0`), не Input System |
| Целевая платформа | Standalone Windows x64 |

На машине стоят и другие версии редактора (2019.1.0f1, 2020.3.48f1, 2021.3.33f1, 2022.3.62f1/f2, 6000.3.19f1) — открывать проект нужно именно **2022.3.62f3**, иначе Unity предложит апгрейд и перезапишет ассеты.

Подключён `com.unity.test-framework` 1.1.33, но тестовых сборок нет. Чтобы тесты заработали, потребуется создать asmdef с ссылками на `UnityEngine.TestRunner` / `UnityEditor.TestRunner` (обычно через Window → General → Test Runner → Create Test Assembly Folder).

## Команды

Unity не имеет отдельного шага «сборки» для разработки — компиляция происходит при открытии редактора. Всё нижеперечисленное запускается только при **закрытом редакторе** (иначе Unity упрётся в блокировку `Library/`).

Компиляция и выход (быстрая проверка, что код собирается):

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -nographics `
  -projectPath "D:\UnityProjects\MaterialAccumulationDemo" `
  -logFile "$env:TEMP\unity_compile.log"
```

Прогон тестов (после того как тестовые сборки появятся):

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -runTests -batchmode -nographics `
  -projectPath "D:\UnityProjects\MaterialAccumulationDemo" `
  -testPlatform EditMode `
  -testResults "$env:TEMP\results.xml" `
  -logFile "$env:TEMP\unity_tests.log"
```

`-testPlatform PlayMode` — для PlayMode-тестов. Один тест или группа: добавь `-testFilter "Namespace.Class.Method"` (принимает регулярное выражение). Batch-mode ничего не пишет в stdout — результаты читай из `-testResults` XML, ход выполнения из `-logFile`.

Сборка плеера требует статического C#-метода и ключа `-executeMethod`; готового build-скрипта в проекте нет.
