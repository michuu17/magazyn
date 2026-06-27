# Sprawozdanie z projektu

## Temat

System Magazynowy. Aplikacja backendowa REST API w technologii ASP.NET Core Web API z architektura warstwowa.

## Repozytorium Git

Adres repozytorium: (tutaj wstaw link do swojego repozytorium)

## Opis domeny

Glowna encja to Produkt przechowywany w magazynie. Encja powiazana to Dostawca, ktory dostarcza produkty do magazynu. Relacja miedzy nimi jest typu jeden do wielu. Jeden dostawca moze byc powiazany z wieloma produktami, a kazdy produkt ma dokladnie jednego dostawce.

## Funkcje dodatkowe

1. Paginacja
2. Soft Delete

## Etap I. Fundamenty

W pierwszym etapie powstal szkielet aplikacji i podzial na foldery (Controllers, Models, DTOs). Zdefiniowane zostaly modele danych Produkt oraz Dostawca wraz z relacja miedzy nimi. Utworzone zostaly obiekty DTO oddzielajace dane wejsciowe i wyjsciowe od modelu wewnetrznego. Dodane zostaly pierwsze endpointy odczytu i zapisu (GET wszystkich, GET po id, POST).

## Etap II. Architektura warstwowa i reguly biznesowe

W drugim etapie cala logika zostala przeniesiona z kontrolerow do warstwy serwisow. Kontrolery odpowiadaja tylko za odebranie zadania i zwrocenie odpowiedzi. Serwisy zostaly zarejestrowane w kontenerze Dependency Injection jako Scoped (interfejsy IProductService oraz ISupplierService wraz z implementacjami).

Zaimplementowane reguly biznesowe:

1. Kod SKU produktu musi byc unikalny. Przy dodawaniu produktu serwis sprawdza, czy istnieje juz aktywny produkt o tym samym kodzie. Jesli tak, zostaje zgloszony wyjatek domenowy.
2. Stan magazynowy nie moze byc ujemny. Operacja wydania towaru sprawdza, czy zadana liczba sztuk nie przekracza dostepnego stanu. W przeciwnym razie operacja zostaje zablokowana.
3. Nie mozna usunac dostawcy, ktory ma przypisane aktywne produkty.

Walidacja danych wejsciowych odbywa sie przy pomocy atrybutow Data Annotations w klasach DTO (Required, StringLength, Range, EmailAddress, Phone). Dodane zostaly endpointy aktualizacji i usuwania (PUT, DELETE).

Globalna obsluga bledow zostala zrealizowana jako wlasny middleware. Przechwytuje on wyjatki i zamienia je na czytelna odpowiedz JSON. Wyjatek NotFoundException zwraca kod 404, wyjatek DomainException zwraca kod 409, a pozostale bledy zwracaja kod 500.

## Etap III. Persystencja i funkcje dodatkowe

W trzecim etapie lista w pamieci zostala zastapiona trwala baza danych. Wykorzystany zostal Entity Framework Core z dostawca SQLite. Kontekst bazy danych (WarehouseDbContext) definiuje zestawy danych oraz konfiguracje encji i relacji. Baza tworzona jest automatycznie przy starcie aplikacji i wypelniana przykladowymi danymi.

Zaimplementowane funkcje dodatkowe:

Paginacja. Endpoint zwracajacy liste produktow przyjmuje parametry page oraz pageSize. Dane zwracane sa stronami. W odpowiedzi znajduja sie rowniez informacje o numerze strony, rozmiarze strony, calkowitej liczbie elementow oraz liczbie stron. Rozmiar strony jest ograniczony od gory, aby uniknac pobierania zbyt duzej liczby rekordow naraz.

Soft Delete. Usuwanie produktow i dostawcow nie usuwa rekordow fizycznie z bazy danych. Zamiast tego ustawiana jest flaga IsDeleted. Wszystkie zapytania odczytu pomijaja rekordy oznaczone jako usuniete. Dzieki temu dane historyczne pozostaja w bazie i moga byc dalej wykorzystywane.

Dokumentacja API zostala przygotowana w Swagger z opisami endpointow pobieranymi z komentarzy XML. Dodatkowo udokumentowane sa mozliwe kody odpowiedzi przy pomocy atrybutow ProducesResponseType. Do projektu dolaczony zostal plik README z instrukcja uruchomienia.

## Podsumowanie

Aplikacja realizuje pelny cykl operacji na produktach i dostawcach, pilnuje regul magazynowych i zapisuje dane w bazie SQLite. Kod zostal podzielony na warstwy, a logika biznesowa znajduje sie w serwisach.
