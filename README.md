###



### Mangler
Vil ikke bruge så meget tid på at udforske statistik endpointet så PT er året hardcodet til 2024. Det kan måske være at det skulle håndteres som en fejl og retunres med tomme svar

Jeg samler telemetry men jeg har ikke konfigereret lang tid opbevaring af dette. Men dette kan eksporteres til Grafana eller lignende
.AddHttpClientInstrumentation() i ServiceDefaults


Build with podman (be in root folder)
podman build -f .\LbForsikring\Dockerfile -t lbforsikring:latest .

Strategi for robust external calls
Med 2 services ville man kunne lave et try catch med divereexceptions

timeout -> prøv igen
internal error -> prøv igen
not found -> prøv ikke igen


