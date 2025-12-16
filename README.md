# Metro API C#
Metro API to backend stworzony w C# (ASP.NET Core), który udostępnia dane dla aplikacji frontendowej.  
Projekt udostępnia informacje o stacjach, liniach i użytkownikach systemu.

---

## Funkcjonalności

- Zarządzanie użytkownikami i rolami (ASP.NET Identity)
- Udostępnianie danych o stacjach i liniach metra
- Obsługa własnych i współdzielonych schematów
- Bezpieczne API dostępne przez HTTPS


Adres API dla front-endu: `https://localhost:7098`

## Uruchamianie z Docker

Aby uruchomić API za pomocą Dockera, wykonaj następujące komendy w katalogu projektu:

```
docker compose build
docker compose up