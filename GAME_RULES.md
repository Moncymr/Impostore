# 🎭 Impostore - Regole del Gioco e Documentazione Completa

## Descrizione dell'Applicazione

**Impostore** è un gioco online multiplayer in tempo reale basato su deduzione sociale, simile a giochi come "Spyfall" o "Among Us" in formato testuale. I giocatori devono identificare l'impostore nascosto tra loro attraverso la discussione e l'osservazione.

### Tecnologie Utilizzate
- **Frontend/Backend**: Blazor Server (.NET 8)
- **Comunicazione Real-time**: SignalR
- **Database**: Entity Framework Core con In-Memory Database
- **UI**: Bootstrap 5 + CSS personalizzato

---

## 🎮 Come Funziona il Gioco

### Fase 1: Lobby e Preparazione

#### Creazione Partita
1. Un giocatore (**Host**) crea una nuova partita inserendo:
   - Il proprio nickname
   - Una password (richiesta per creare partite)
2. Il sistema genera un **codice partita univoco** (es: DOJG9G)
3. L'host condivide questo codice con gli amici

#### Join dei Giocatori
1. Altri giocatori inseriscono il loro nickname
2. Inseriscono il codice partita ricevuto dall'host
3. Vengono aggiunti alla **lista in attesa** della lobby
4. **Solo l'host** vede le richieste di accesso e può:
   - ✅ **Accettare** il giocatore (lo approva per giocare)
   - ❌ **Rifiutare** il giocatore (lo rimuove dalla partita)

#### Visibilità nella Lobby
- **Tutti i giocatori** vedono l'elenco completo:
  - Giocatori approvati (con badge "Host" per l'host)
  - Giocatori in attesa (con badge "In Attesa")
- Il conteggio mostra: `Giocatori (3)` = approvati + in attesa
- **Solo l'host** vede i pulsanti Accetta/Rifiuta

#### Requisiti per Iniziare
- Minimo **3 giocatori approvati**
- Solo l'host può premere il pulsante "🚀 Inizia Partita"

---

### Fase 2: Assegnazione Ruoli e Parola Segreta

Quando l'host avvia la partita:

#### Selezione dell'Impostore
1. Il sistema sceglie **casualmente** un giocatore come **Impostore**
2. Gli altri giocatori diventano **Giocatori Normali**

#### Assegnazione della Parola Segreta
1. Il sistema sceglie casualmente una parola da un database di 40+ parole italiane
2. Le parole sono organizzate in 5 categorie:
   - 🐾 **Animali**: Gatto, Cane, Elefante, Leone, Tigre, Delfino, Pinguino, Farfalla
   - 🍕 **Cibo**: Pizza, Pasta, Gelato, Panino, Sushi, Cioccolato, Hamburger, Torta
   - ⚽ **Sport**: Calcio, Tennis, Basket, Nuoto, Pallavolo, Sci, Boxe, Yoga
   - 👔 **Professioni**: Dottore, Insegnante, Poliziotto, Cuoco, Pilota, Cantante, Attore, Ingegnere
   - 🏖️ **Luoghi**: Spiaggia, Montagna, Scuola, Cinema, Ristorante, Ospedale, Parco, Biblioteca

#### Distribuzione delle Informazioni
- **Giocatori Normali** ricevono:
  - ✅ La **parola segreta** esatta (es: "Gatto")
  - 📝 Istruzione: "Non dire la parola esatta, ma dai indizi!"
  
- **Impostore** riceve:
  - ❌ **NON** conosce la parola segreta
  - 💡 Riceve solo un **suggerimento/categoria** (es: "Animale domestico")
  - 🎭 Badge "SEI L'IMPOSTORE!"

#### Notifica Inizio Turni
- Il sistema sceglie casualmente quale giocatore inizia
- Tutti ricevono una notifica: "È il turno di [Nome]!"
- Messaggio di sistema in chat: "La partita è iniziata!"

---

### Fase 3: Turni di Gioco (InProgress)

#### Meccanica dei Turni
1. **Solo il giocatore del turno corrente** può scrivere messaggi nella chat
2. Il giocatore deve dare indizi sulla parola senza rivelarla direttamente
3. Gli altri giocatori possono solo leggere i messaggi
4. Il placeholder della chat mostra:
   - Per il giocatore attivo: "È il tuo turno! Scrivi il tuo messaggio..."
   - Per gli altri: "È il turno di [Nome]. Attendi il tuo turno..."

#### Obiettivi dei Giocatori
- **Giocatori Normali**: 
  - Dare indizi abbastanza chiari da far capire agli altri che conoscono la parola
  - Ma abbastanza vaghi da non rivelare troppo all'impostore
  - Osservare gli indizi degli altri per identificare l'impostore
  
- **Impostore**:
  - Dare indizi credibili basandosi solo sul suggerimento
  - Cercare di mimetizzarsi tra i giocatori normali
  - Dedurre la parola segreta ascoltando gli indizi degli altri
  - Evitare di essere scoperto

#### Controlli dell'Host
- **Turno Successivo**: Passa al giocatore successivo
- **Passa alla Discussione**: Salta i turni rimanenti e inizia la discussione

#### Notifiche di Cambio Turno
- Ogni cambio turno genera:
  - Evento SignalR `TurnChanged` a tutti i giocatori
  - Messaggio di sistema: "È il turno di [Nome]!"
  - Aggiornamento UI per mostrare chi può scrivere

---

### Fase 4: Discussione (Discussion)

#### Avvio della Fase
- L'host può avviare manualmente la discussione
- Oppure si avvia automaticamente dopo che tutti hanno giocato il loro turno

#### Meccanica della Discussione
- **Tutti i giocatori** possono scrivere liberamente nella chat
- Nessuna restrizione sui messaggi
- I giocatori discutono apertamente per identificare l'impostore

#### Sistema "Pronto a Votare"
- Ogni giocatore vede il pulsante: **"✋ Sono Pronto a Votare"**
- Quando un giocatore dichiara di essere pronto:
  - Il suo stato cambia: badge passa da ⏳ a ✓
  - Tutti vedono l'aggiornamento in tempo reale
- Display dello stato:
  - "Giocatori pronti: 2 / 3"
  - Lista dei giocatori con badge: "Mario ✓", "Luigi ⏳", "Peach ✓"

#### Transizione Automatica alla Votazione
- Quando **TUTTI** i giocatori hanno dichiarato di essere pronti
- Il sistema avvia automaticamente la **Fase di Votazione**
- Non c'è più controllo manuale dell'host per questa transizione

---

### Fase 5: Votazione (Voting)

#### Meccanica della Votazione per Nome

**Per ogni giocatore non votato:**
1. Appare un campo di testo: "Scrivi il nome di chi pensi sia l'impostore"
2. Sotto il campo, viene mostrata la lista di tutti i giocatori come riferimento
3. Il giocatore scrive il nome dell'impostore sospetto
4. Il sistema fa un match **case-insensitive** con i nomi dei giocatori
5. Il voto viene registrato con:
   - Il nome scritto dal giocatore
   - L'ID del giocatore corrispondente (se trovato)

**Dopo aver votato:**
- Conferma: "✓ Hai votato per: [Nome]"
- Messaggio: "In attesa degli altri..."
- Conteggio: "Voti ricevuti: 2 / 3"

#### Restrizioni
- Ogni giocatore può votare **una sola volta**
- Non può votare per se stesso (il proprio nome non è nella lista)
- Una volta votato, non può cambiare il voto

#### Fine della Votazione
- La votazione termina automaticamente quando **tutti** hanno votato
- Il sistema conta i voti e determina il vincitore

---

### Fase 6: Risultati e Fine Partita (Finished)

#### Calcolo del Vincitore
Il sistema conta i voti per ogni giocatore:
1. **Se l'Impostore è il più votato**:
   - 🎉 **I Giocatori Vincono!**
   - Messaggio: "I giocatori hanno vinto! L'impostore era [Nome]"
   
2. **Se un altro giocatore è il più votato**:
   - 🎭 **L'Impostore Vince!**
   - Messaggio: "L'impostore [Nome] ha vinto!"

#### Schermata dei Risultati
Mostra:
- Il vincitore della partita
- Chi era l'impostore (con badge "Impostore" nella lista giocatori)
- La parola segreta
- Il giocatore più votato

#### Opzioni Post-Partita

**Per l'Host:**
- 🔄 **"Nuova Partita"**: Avvia una rematch automatica
  - Tutti i giocatori tornano alla lobby
  - Nessuno deve lasciare e rientrare
  - Ruoli, parola, voti e messaggi vengono resettati
  - Giocatori mantengono lo stato "approvato"
  - Host mantiene il ruolo di host

**Per tutti:**
- 🏠 **"Torna alla Home"**: Esce dalla partita

**Per i Non-Host:**
- Messaggio: "In attesa che l'host inizi una nuova partita..."

---

## 🔐 Gestione delle Connessioni

### Sistema di ConnectionId
- Ogni giocatore ha un `ConnectionId` SignalR univoco
- Usato per inviare notifiche mirate (es: solo all'host)
- Tracciato quando:
  - Il giocatore entra nella lobby
  - Il giocatore entra nella partita
  - Il giocatore si disconnette

### Notifiche Mirate
- **PlayerJoinRequest**: Solo all'host (via ConnectionId)
- **TurnChanged**: A tutti i giocatori del gruppo
- **PlayerReadyUpdated**: A tutti i giocatori del gruppo
- **GameUpdated**: A tutti i giocatori del gruppo
- **RematchStarted**: A tutti i giocatori del gruppo

### Gestione Disconnessioni
- Il sistema traccia se un giocatore è connesso o meno
- `UpdatePlayerConnection` aggiorna lo stato
- Cleanup automatico quando il componente viene distrutto

---

## 📊 Stati del Gioco (GameState)

Il gioco ha 6 stati possibili:

1. **Lobby**: 
   - Attesa giocatori
   - Approvazione/rifiuto richieste
   - Chat disabilitata

2. **Starting**: 
   - Inizializzazione della partita
   - Assegnazione ruoli e parola
   - Chat disabilitata

3. **InProgress**: 
   - Fase turni
   - Solo il giocatore del turno può scrivere
   - Chat con restrizioni

4. **Discussion**: 
   - Discussione libera
   - Tutti possono scrivere
   - Sistema "pronto a votare"

5. **Voting**: 
   - Votazione attiva
   - Tutti possono scrivere
   - Input per nome dell'impostore

6. **Finished**: 
   - Partita terminata
   - Risultati visibili
   - Chat disabilitata
   - Opzione rematch

---

## 🎯 Strategie di Gioco

### Per i Giocatori Normali
1. **Dare indizi chiari ma non troppo ovvi**
   - ✅ "Vive in casa con le persone" (per Gatto)
   - ❌ "Fa le fusa" (troppo specifico)
   - ❌ "È un animale" (troppo vago)

2. **Osservare chi dà indizi vaghi o incongruenti**
   - L'impostore potrebbe dare indizi generici
   - Potrebbero contraddirsi tra loro

3. **Collaborare senza rivelare troppo**
   - Non dire la parola esatta
   - Non dare indizi che permettano all'impostore di capire subito

### Per l'Impostore
1. **Usare il suggerimento in modo intelligente**
   - Se il suggerimento è "Animale domestico"
   - Dare indizi che vadano bene per vari animali
   - "Ha quattro zampe", "Vive con le famiglie"

2. **Ascoltare attentamente gli altri**
   - Dedurre la parola dagli indizi degli altri
   - Adattare i propri indizi di conseguenza

3. **Evitare di essere troppo generico**
   - Se tutti danno indizi specifici e tu sei vago, sembri sospetto
   - Bilanciare tra essere credibile e non rivelare che non conosci la parola

---

## 🔧 Caratteristiche Tecniche

### Prevenzione Duplicati
- Guard `isInitialized` in Lobby.razor
- Previene doppia inizializzazione del componente
- Evita richieste duplicate di join

### Votazione Intelligente
- Match case-insensitive dei nomi
- Memorizza sia il nome scritto che l'ID del giocatore
- Permette analisi dei voti anche se il nome è scritto male

### Rematch Seamless
- Reset completo dello stato di gioco
- Preserva i giocatori approvati
- Pulisce: voti, messaggi, ruolo impostore, parola segreta
- Nessun bisogno di lasciare e rientrare

### Chat Context-Aware
- Placeholder diversi per ogni stato del gioco
- Disabilitazione automatica quando non si può scrivere
- Restrizioni basate sul turno in fase InProgress

---

## 📝 Eventi SignalR Implementati

| Evento | Direzione | Scopo |
|--------|-----------|-------|
| `PlayerJoinRequest` | Server → Host | Notifica richiesta di join |
| `PlayerApproved` | Server → All | Giocatore approvato |
| `PlayerRejected` | Server → Player | Giocatore rifiutato |
| `GameStarted` | Server → All | Partita iniziata |
| `GameUpdated` | Server → All | Stato gioco aggiornato |
| `TurnChanged` | Server → All | Cambio turno |
| `DiscussionStarted` | Server → All | Discussione iniziata |
| `PlayerReadyUpdated` | Server → All | Giocatore pronto a votare |
| `VotingStarted` | Server → All | Votazione iniziata |
| `VoteCast` | Server → All | Voto registrato |
| `GameFinished` | Server → All | Partita finita |
| `RematchStarted` | Server → All | Rematch iniziato |
| `ReceiveMessage` | Server → All | Nuovo messaggio in chat |
| `PlayerConnectionUpdated` | Server → All | Stato connessione aggiornato |

---

## 🎨 Convenzioni UI

### Badge
- **Host**: Blu - indica il creatore della partita
- **In Attesa**: Giallo - giocatore non ancora approvato
- **Impostore**: Rosso - mostrato solo a fine partita

### Stati Ready
- **✓ (verde)**: Giocatore pronto a votare
- **⏳ (giallo)**: Giocatore non ancora pronto

### Messaggi di Sistema
- Formattati con classe CSS speciale
- Colore diverso dai messaggi normali
- Esempi:
  - "La partita è iniziata!"
  - "È il turno di Mario!"
  - "Fase di discussione iniziata!"
  - "Fase di votazione iniziata!"

---

## 🔮 Funzionalità Future Possibili

Basandosi sui nuovi requisiti menzionati:

1. **Selezione Parole con AI (Gemini)**
   - Integrazione con Gemini API
   - Generazione dinamica di parole e suggerimenti
   - Parole più varie e contestuali

2. **Ruolo Sempre Visibile**
   - Card persistente con ruolo e parola/suggerimento
   - Sempre visibile in tutte le fasi del gioco
   - Non solo durante InProgress/Discussion/Voting

3. **Categorie Personalizzate**
   - Permettere all'host di scegliere categorie
   - Aggiungere parole personalizzate

4. **Timer per i Turni**
   - Limite di tempo per ogni turno
   - Avanzamento automatico

5. **Statistiche Giocatori**
   - Tracciare vittorie/sconfitte
   - Percentuale di successo come impostore
   - Storico partite

---

## 📄 Licenza e Crediti

- **Progetto**: Impostore Game
- **Tecnologia**: Blazor Server + SignalR
- **Autore Originale**: Moncymr
- **Licenza**: Open Source (MIT)
