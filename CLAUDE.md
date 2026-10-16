# CLAUDE.md

REST-API (ASP.NET Core) für das Assecor-Backend-Assessment: Personen und Lieblingsfarben aus einer CSV lesen und per HTTP ausliefern. Die Aufgabenstellung steht in `README.md` und ist maßgeblich.

## Struktur

- `src/PersonsApi/` – Web-API
  - `Controllers/` – `PersonsController` (`GET /persons`, `GET /persons/{id}`, `GET /persons/color/{color}`, `POST /persons`)
  - `Core/` – Modell `Person`, `ColorMapper`, Abstraktion `IPersonRepository`
  - `Infrastructure/` – `CsvPersonRepository` (liest CSV, neue Personen nur im Speicher)
  - `Data/sample-input.csv` – Eingabedaten, wird ins Output-Verzeichnis kopiert
- `PersonsApi.Tests/` – xUnit-Tests (Moq für den Controller, `TestData/persons.csv` für das Repository)
- `*.ps1` – Build-/Test-Skripte, werden auch von `azure-pipelines.yml` genutzt

## Befehle

Im Repo-Root ausführen (PowerShell):

```powershell
dotnet restore PersonsApi.sln
dotnet build PersonsApi.sln -c Release
dotnet test PersonsApi.sln -c Release
dotnet test PersonsApi.sln -c Release --filter "FullyQualifiedName~PersonsControllerTests"   # einzelne Klasse
dotnet run --project src/PersonsApi                                                          # API starten
```

Gleichwertig über die Skripte: `.\build.ps1`, `.\test.ps1`, `.\build-and-test.ps1`.

Start: Launch-Profil `PersonsApi` in `src/PersonsApi/Properties/launchSettings.json` (`https://localhost:63683`, `http://localhost:63684`, Environment `Development`). Swagger unter `/swagger`, nur in `Development` aktiv.

## Konventionen

- Zielframework: **net8.0** für API und Tests. Neue Projekte ebenfalls auf net8.0 legen.
- `Nullable` und `ImplicitUsings` sind aktiv. In der API gilt `TreatWarningsAsErrors` – Build muss warnungsfrei sein.
- `GenerateDocumentationFile` ist an: öffentliche Typen und Member bekommen XML-Doc-Kommentare (`/// <summary>`).
- Datenzugriff nur über `IPersonRepository`, per Dependency Injection in `Program.cs` registriert. Controller kennen keine konkrete Datenquelle.
- IDs sind die CSV-Zeilennummern; per `POST` angelegte Personen bekommen `max(Id) + 1` und werden nicht in die CSV geschrieben.
- **`src/PersonsApi/Data/sample-input.csv` nicht verändern** (Akzeptanzkriterium 6). Für Tests `PersonsApi.Tests/TestData/persons.csv` verwenden.
- Tests: xUnit, Arrange/Act/Assert, Methodennamen `Methode_Erwartung`. Jede Verhaltensänderung braucht einen Test.
- Dateikodierung beachten: `ColorMapper.cs` und Tests enthalten Umlaute (grün, türkis, weiß). Beim Bearbeiten die vorhandene Kodierung beibehalten und danach prüfen, dass `dotnet test` weiter grün ist.
- Commits klein halten, ein Thema pro Commit; Diff vor dem Commit durchsehen.
- Die Lösung nicht öffentlich machen (Hinweis aus der Aufgabenstellung).
