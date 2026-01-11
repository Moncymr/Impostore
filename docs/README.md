# GitHub Pages per Impostore

Questa cartella contiene una landing page statica per GitHub Pages.

## Perché una landing page invece dell'app completa?

Impostore è un'applicazione **Blazor Server** che richiede:
- Un server .NET attivo per eseguire il codice C#
- SignalR per la comunicazione in tempo reale tra giocatori
- WebSocket per le connessioni persistenti

GitHub Pages supporta solo **file statici** (HTML, CSS, JS) e non può eseguire codice server-side o gestire WebSocket.

## Opzioni per il Deploy

### Opzione 1: Landing Page su GitHub Pages + App su servizio cloud (CONSIGLIATO)
- Usa questa landing page su GitHub Pages come vetrina
- Deploya l'app vera su:
  - **Azure App Service** (ha tier gratuito)
  - **Railway** (ha tier gratuito)
  - **Render** (ha tier gratuito)
- Aggiungi il link all'app deployata nella landing page

### Opzione 2: Convertire a Blazor WebAssembly
- Richiederebbe riscrivere completamente l'architettura
- Perdita delle funzionalità multiplayer in tempo reale
- **NON CONSIGLIATO** per questo tipo di gioco

## Come abilitare GitHub Pages

1. Vai su Settings del repository GitHub
2. Nella sezione "Pages"
3. Seleziona Source: "Deploy from a branch"
4. Seleziona Branch: "main" e folder: "/docs"
5. Salva

Il sito sarà disponibile su: `https://moncymr.github.io/Impostore/`
