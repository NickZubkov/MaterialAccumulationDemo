# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Текущее состояние проекта

Это **пустой шаблон Unity URP**, а не работающая кодовая база. На данный момент в `Assets/` лежат только:

- `Assets/Scenes/SampleScene.unity` — стандартная сцена шаблона (Main Camera, Directional Light, Global Volume), единственная сцена в Build Settings;
- `Assets/Settings/` — три URP-профиля качества (`URP-Performant`, `URP-Balanced`, `URP-HighFidelity`) с соответствующими Renderer-ассетами;
- `Assets/UniversalRenderPipelineGlobalSettings.asset`.

**Скриптов (`.cs`), asmdef-файлов и тестов в проекте нет.** Название `MaterialAccumulationDemo` отражает замысел, но механика накопления материалов ещё не реализована — не предполагай наличие соответствующей архитектуры, её нужно спроектировать с нуля.

Следствие: `Assembly-CSharp.csproj` устарел — он содержит ссылку на удалённый `Assets\TutorialInfo\Scripts\Readme.cs`. Файлы `.csproj`/`.sln` генерируются Unity, править их вручную не нужно; они перегенерируются после добавления первого скрипта.

## Git: репозиторий — родительская папка

Критично для любых операций с git. Корень репозитория — не этот проект, а `D:\UnityProjects`, где рядом лежат десятки других Unity-проектов (`Forest`, `CrazyPawns`, `Excavator`, `MergeMechanicDemo` и т. д.).

- `MaterialAccumulationDemo` **полностью не отслеживается** git (0 файлов в индексе).
- **В репозитории нет ни одного `.gitignore`.** При этом `Library/` в этом проекте занимает ~1.9 ГБ, `Logs/` — ~1.4 МБ.

Поэтому: никогда не выполняй `git add .` или `git add -A` — это затянет в индекс гигабайты генерируемых Unity папок (`Library/`, `Temp/`, `Logs/`, `obj/`) сразу по нескольким проектам. Добавляй только конкретные пути внутри `Assets/`, `Packages/`, `ProjectSettings/`. Если проект нужно поставить под контроль версий — сначала заведи `.gitignore` для Unity.

`git status` из этой директории показывает изменения по всем соседним проектам; фильтруй вывод путём (`git status MaterialAccumulationDemo`).

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
