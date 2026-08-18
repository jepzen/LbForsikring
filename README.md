### Tilvalg
Bruger dotnet Aspire. Det giver en automatisk side med statistik og metrics.
( .AddHttpClientInstrumentation() i ServiceDefaults )



### Fravalg
Ignorer git history. Jeg går normalt meget op i at gøre disse pæne men en god beskrivelse så venligst ignorer dette. 

Ikke en nice struktur. Bruger normalt ./src ./tests 

Vil ikke bruge så meget tid på at udforske statistik endpointet så PT er året hardcodet til 2024. Det kan måske være at det skulle håndteres som en fejl og retunres med tomme svar

Jeg samler telemetry men jeg har ikke konfigereret lang tid opbevaring af dette. I produktion ville jeg eksportere det til Grafana eller lignende


Strategi for robust external calls
Med 2 services ville man kunne lave et try catch med divereexceptions

timeout -> prøv igen
internal error -> prøv igen
not found -> prøv ikke igen


