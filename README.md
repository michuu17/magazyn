# System Magazynowy API

Aplikacja backendowa typu REST API do zarzadzania magazynem. Pozwala prowadzic ewidencje produktow oraz dostawcow, przyjmowac i wydawac towar ze stanu, a takze pilnuje podstawowych regul magazynowych.

Projekt zostal podzielony na trzy etapy: fundamenty, logika biznesowa oraz persystencja wraz z funkcjami dodatkowymi. Wszystkie etapy znajduja sie w jednym rozwiazaniu.

## Domena

Glowna encja to Produkt, czyli towar znajdujacy sie w magazynie. Encja powiazana to Dostawca, ktory dostarcza produkty. Jeden dostawca moze dostarczac wiele produktow, a kazdy produkt nalezy do jednego dostawcy.

## Stos technologiczny

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8
- Baza danych SQLite
- Swagger (Swashbuckle) z opisami endpointow z komentarzy XML

## Wymagania

- Zainstalowany .NET SDK w wersji 8.0 lub nowszej

Sprawdzenie wersji:

```
dotnet --version
```

## Uruchomienie

1. Wejdz do katalogu projektu (tam gdzie znajduje sie plik WarehouseApi.csproj).
2. Pobierz zaleznosci:

```
dotnet restore
```

3. Uruchom aplikacje:

```
dotnet run
```

4. Otworz w przegladarce dokumentacje Swagger:

```
http://localhost:5080/swagger
```

Baza danych SQLite (plik warehouse.db) tworzy sie automatycznie przy pierwszym uruchomieniu i zostaje wypelniona przykladowymi danymi (dwoch dostawcow i czworka produktow).

## Struktura projektu

- Controllers: kontrolery REST, cienka warstwa odbierajaca zadania
- Services: logika biznesowa i reguly magazynowe
- Models: encje bazodanowe
- DTOs: obiekty transferowe oddzielajace API od modelu danych
- Data: kontekst EF Core oraz dane poczatkowe
- Exceptions: wlasne wyjatki domenowe
- Middleware: globalna obsluga bledow

## Reguly biznesowe

1. Kod SKU produktu musi byc unikalny. Proba dodania produktu z istniejacym kodem konczy sie bledem.
2. Nie mozna wydac ze stanu wiecej sztuk niz aktualnie dostepne. Stan magazynowy nie moze zejsc ponizej zera.
3. Nie mozna usunac dostawcy, ktory ma przypisane aktywne produkty.

## Funkcje dodatkowe

- Paginacja. Lista produktow zwracana jest stronami. Parametry page oraz pageSize steruja numerem strony i jej rozmiarem. Odpowiedz zawiera numer strony, rozmiar strony, liczbe wszystkich elementow oraz liczbe stron. Rozmiar strony jest ograniczony od gory do 100.
- Soft Delete. Usuwanie produktow i dostawcow nie kasuje rekordow z bazy. Ustawiana jest flaga IsDeleted, a usuniete rekordy sa pomijane przy odczycie. Dzieki temu historia danych zostaje zachowana.

## Endpointy

Produkty:

- GET /api/products?page=1&pageSize=10
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}
- POST /api/products/{id}/receive
- POST /api/products/{id}/issue

Dostawcy:

- GET /api/suppliers
- GET /api/suppliers/{id}
- POST /api/suppliers
- PUT /api/suppliers/{id}
- DELETE /api/suppliers/{id}

## Przyklad dodania produktu

```
POST /api/products
{
  "sku": "STL-010",
  "name": "Podkladka M8",
  "description": "Podkladka plaska",
  "quantity": 200,
  "unitPrice": 0.15,
  "supplierId": 1
}
```

## Uwaga o bazie danych

Domyslnie baza tworzona jest metoda EnsureCreated przy starcie aplikacji. Jesli chcesz korzystac z migracji EF Core, zainstaluj narzedzie dotnet-ef, usun wywolanie EnsureCreated w pliku Program.cs i wykonaj:

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```
