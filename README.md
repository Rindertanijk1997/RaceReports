# RaceReports

## Om projektet
Detta projekt är en **examinationsuppgift i .NET** där fokus har varit att bygga en fungerande **Blogg** i **ASP.NET Core Web API** enligt givna krav.

Efter att backenden var färdig och testad valde jag att även lägga på en **Blazor Server-frontend** för att lära mig hur det fungerar.

---

## Backend – ASP.NET Core Web API
Backenden innehåller all affärslogik och databasåtkomst och kan köras fristående.

### Tekniker
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- DTOs  
- Swagger  

### Funktionalitet
- Registrera, logga in, uppdatera och ta bort användare
- Skapa, uppdatera och ta bort blogginlägg
- Endast skaparen av ett inlägg kan uppdatera eller ta bort det
- Kategorier lagras i egen tabell i databasen
- Kommentera andra användares inlägg
- Sök på titel och kategori

Lösenord hanteras säkert med hashning och API:t returnerar korrekta HTTP-statuskoder.

---

## Frontend – Blazor Server
Blazor används som frontend för att testa och lära mig hur API:t används i praktiken.

### Funktionalitet
- Registrera konto och logga in
- Visa alla inlägg 
- Söka på titel och kategori
- Skapa, redigera och ta bort egna inlägg
- Kommentera andras inlägg

Frontend kommunicerar med backend via `HttpClient`.



