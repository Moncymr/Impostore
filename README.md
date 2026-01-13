# 🎭 Impostore - Gioco Online in Tempo Reale

Un'applicazione web gratuita per giocare al gioco "Impostore" (simile a Spyfall) in tempo reale con amici.

---

## ⚠️ ERRORE DI BUILD "net10.0"?

Se ottieni l'errore: `Assets file doesn't have a target for 'net10.0'`

👉 **[LEGGI QUESTA GUIDA URGENTE](URGENT_FIX.md)** 👈

Il tuo file di progetto locale è stato modificato per errore. La guida ti mostra come correggere.

---

## 📋 Descrizione

Impostore è un gioco di deduzione sociale dove un giocatore è l'impostore che non conosce la parola segreta, mentre tutti gli altri la conoscono. I giocatori devono discutere e votare per scoprire chi è l'impostore.

## 🚀 Tecnologie Utilizzate

- **Frontend**: Blazor Server (.NET 8)
- **Backend**: ASP.NET Core 8
- **Realtime**: SignalR
- **Database**: Entity Framework Core con In-Memory Database
- **UI**: Bootstrap 5 + CSS personalizzato
- **Hosting**: Compatibile con piani gratuiti (Azure, Heroku, Railway, ecc.)

## ✨ Funzionalità

### 1. Gestione Partita
- ✅ Creazione partita con codice univoco
- ✅ Host può approvare/rifiutare giocatori
- ✅ Minimo 3 giocatori per iniziare

### 2. Autenticazione
- ✅ Solo nickname, nessuna registrazione richiesta
- ✅ Nessun login o password

### 3. Regole di Gioco
- ✅ Un giocatore casuale diventa l'Impostore
- ✅ L'Impostore NON riceve la parola segreta
- ✅ Altri giocatori ricevono la stessa parola segreta
- ✅ 40+ parole predefinite in 5 categorie (Animali, Cibo, Sport, Professioni, Luoghi)

### 4. Flusso di Gioco
- ✅ **Lobby**: Attesa giocatori
- ✅ **Fase Turni**: Messaggi uno alla volta
- ✅ **Fase Discussione**: Chat libera per tutti
- ✅ **Votazione**: Voto anonimo per identificare l'impostore
- ✅ **Risultati**: Visualizzazione del vincitore

### 5. Chat in Tempo Reale
- ✅ Chat realtime con SignalR
- ✅ Messaggi di sistema
- ✅ Timestamp sui messaggi
- ✅ **Chat vocale integrata con WebRTC** 🎤
- ✅ **Controlli mute/unmute**
- ✅ **Indicatori di chi sta parlando**

### 6. UI/UX
- ✅ Design responsive (desktop + mobile)
- ✅ Interfaccia moderna e colorata
- ✅ Indicatori di stato di gioco
- ✅ Feedback visivo per tutte le azioni

## 🎮 Come Giocare

1. **Inserisci il tuo nickname** nella homepage
2. **Crea una partita** o **unisciti** inserendo un codice
3. **L'host approva i giocatori** che richiedono di entrare
4. **Quando ci sono almeno 3 giocatori**, l'host può iniziare la partita
5. **Ogni giocatore riceve il suo ruolo**:
   - L'Impostore: NON conosce la parola
   - Altri giocatori: Conoscono la parola segreta
6. **Fase Turni**: I giocatori parlano a turno, dando indizi senza rivelare troppo
7. **Chat Vocale** (opzionale): Clicca "Connetti" per parlare con gli altri giocatori 🎤
8. **Fase Discussione**: Tutti possono parlare liberamente
9. **Votazione**: Tutti votano chi pensano sia l'impostore
10. **Risultati**: 
    - Se l'impostore è quello più votato → I giocatori vincono!
    - Altrimenti → L'impostore vince!

## 📚 Documentazione

- 🚀 **[Guida Rapida](GUIDA_RAPIDA.md)** - Inizia a giocare in 5 minuti! (con screenshot)
- 📖 **[Manuale Utente Completo](MANUALE_UTENTE.md)** - Istruzioni dettagliate per ogni aspetto del gioco
- 🎮 **[Regole del Gioco](GAME_RULES.md)** - Documentazione completa delle meccaniche di gioco
- 🔧 **[Risoluzione Problemi Build](TROUBLESHOOTING_BUILD.md)** - Soluzioni per errori di compilazione e pubblicazione

## 🛠️ Installazione e Avvio

### Prerequisiti
- .NET 8 SDK

### Comandi

```bash
# Clone il repository
git clone https://github.com/Moncymr/Impostore.git
cd Impostore

# Ripristina le dipendenze
dotnet restore

# Avvia l'applicazione
dotnet run

# Apri nel browser
# http://localhost:5000
```

## 🏗️ Struttura del Progetto

```
Impostore/
├── Components/
│   ├── Pages/
│   │   ├── Home.razor          # Homepage con nickname
│   │   ├── Lobby.razor         # Lobby di attesa
│   │   └── GamePlay.razor      # Interfaccia di gioco
│   └── Layout/                 # Layout dell'app
├── Models/
│   ├── Game.cs                 # Modello della partita
│   ├── Player.cs               # Modello del giocatore
│   ├── ChatMessage.cs          # Modello dei messaggi
│   ├── Vote.cs                 # Modello dei voti
│   └── Word.cs                 # Modello delle parole
├── Services/
│   ├── GameService.cs          # Logica di gestione partite
│   └── WordService.cs          # Gestione parole segrete
├── Hubs/
│   └── GameHub.cs              # Hub SignalR per realtime
├── Data/
│   └── GameDbContext.cs        # Context EF Core con seed
└── wwwroot/
    └── app.css                 # Stili personalizzati
```

## 🎨 Screenshot

### Homepage
![Homepage](https://github.com/user-attachments/assets/aab1d4fe-c80a-4c11-92ae-3fe069a45ef6)

### Lobby (Desktop)
![Lobby Desktop](https://github.com/user-attachments/assets/9d1fdf60-0ac8-43ef-b96e-183d4a6c5955)

### Lobby (Mobile)
![Lobby Mobile](https://github.com/user-attachments/assets/39c009a7-8c6a-4b1c-abbd-c6c9f07c075e)

## 🚢 Deploy

### Azure App Service (Free Tier)
```bash
# Pubblica su Azure
az webapp up --name impostore-game --resource-group myResourceGroup
```

### Railway
1. Connetti il repository GitHub
2. Railway rileverà automaticamente il progetto .NET
3. Deploy automatico ad ogni push

### Render
1. Crea un nuovo Web Service
2. Connetti il repository
3. Build Command: `dotnet build`
4. Start Command: `dotnet run`

## 📝 Note Tecniche

- **Database In-Memory**: I dati non persistono al riavvio (ideale per giochi veloci)
- **SignalR**: Gestisce la comunicazione realtime tra giocatori
- **Blazor Server**: Renderizzazione lato server con aggiornamenti in tempo reale
- **WebRTC**: Chat vocale peer-to-peer integrata
- **Nessuna autenticazione complessa**: Solo nickname per facilità d'uso

## 🔮 Possibili Miglioramenti Futuri

- [ ] Persistenza con SQLite per storico partite
- [ ] Categorie personalizzate per le parole
- [ ] Timer per i turni
- [ ] Statistiche giocatori
- [ ] Stanze private con password
- [ ] Modalità spettatore
- [x] Chat vocale integrata ✅
- [ ] Personalizzazione avatar
- [ ] Peer-to-peer voice routing per migliori prestazioni

## 📄 Licenza

Questo progetto è open source e disponibile sotto la licenza MIT.

## 👥 Contributori

- Moncymr - Creatore iniziale

## 🤝 Contribuire

Le pull request sono benvenute! Per modifiche importanti, apri prima un issue per discutere cosa vorresti cambiare.

## 📞 Supporto

Per domande o problemi, apri un issue su GitHub.

---

**Buon divertimento con Impostore! 🎭🎮**
