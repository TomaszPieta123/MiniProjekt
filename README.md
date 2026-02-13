# MiniProjekt

## REST API – ASP.NET Core (.NET 8)

Projekt „MiniProjekt” to aplikacja typu REST API stworzona w technologii **ASP.NET Core (.NET 8)** z wykorzystaniem:

* Entity Framework Core
* Microsoft Identity
* JWT Authentication
* SQLite (baza danych)

Celem projektu było stworzenie systemu do zarządzania projektami i zadaniami z obsługą użytkowników oraz autoryzacją opartą na tokenach JWT.

---

## Funkcjonalności

* rejestracja i logowanie użytkowników (Identity + JWT),
* autoryzacja dostępu do endpointów API,
* zarządzanie projektami (CRUD),
* zarządzanie zadaniami przypisanymi do projektów,
* przypisywanie zadań użytkownikom,
* dostęp użytkownika do:

  * projektów, których jest właścicielem,
  * projektów, w których ma przypisane zadania.

API zostało zaprojektowane zgodnie z zasadami REST – komunikacja jest bezstanowa, a uwierzytelnianie odbywa się poprzez token JWT przesyłany w nagłówku Authorization.

---

## Technologie

* .NET 8 / ASP.NET Core Web API
* Entity Framework Core
* Microsoft Identity
* JWT Bearer Authentication
* SQLite
* Swagger (OpenAPI)

---

## Uruchomienie projektu

1. Sklonuj repozytorium:

```
git clone https://github.com/TomaszPieta123/MiniProjekt.git
```

2. Wykonaj migracje bazy danych:

```
Update-Database
```

lub:

```
dotnet ef database update
```

3. Uruchom aplikację:

```
dotnet run
```

4. Swagger dostępny pod adresem:

```
/swagger
```
---

## Autor

Tomasz Pięta
