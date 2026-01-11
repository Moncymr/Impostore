# 📋 Guida: Come Abilitare GitHub Pages

## Prerequisiti
- Hai fatto il merge di questa Pull Request nel branch `main`
- Sei il proprietario del repository o hai permessi di amministratore

## Passi per Abilitare GitHub Pages

### 1. Vai alle Impostazioni del Repository
1. Apri il repository su GitHub: https://github.com/Moncymr/Impostore
2. Clicca sulla tab **"Settings"** (Impostazioni) in alto a destra

### 2. Configura GitHub Pages
1. Nel menu laterale sinistro, scorri verso il basso e clicca su **"Pages"**
2. Nella sezione **"Build and deployment"**:
   - **Source**: Seleziona **"Deploy from a branch"**
   - **Branch**: Seleziona **"main"**
   - **Folder**: Seleziona **"/docs"**
3. Clicca su **"Save"** (Salva)

### 3. Attendi il Deploy
- GitHub impiegherà alcuni minuti per pubblicare il sito
- Vedrai un messaggio che dice "Your site is live at..."
- Il sito sarà disponibile all'indirizzo: **https://moncymr.github.io/Impostore/**

### 4. Verifica il Deploy (Opzionale)
1. Vai alla tab **"Actions"** nel repository
2. Dovresti vedere il workflow **"Deploy to GitHub Pages"** in esecuzione o completato
3. Una volta completato con successo (✅ verde), il sito è online!

## 🎉 Fatto!

La tua landing page è ora pubblicata su GitHub Pages e sarà accessibile pubblicamente.

### Note Importanti

#### ⚠️ L'App Completa Non È su GitHub Pages
Ricorda che questa è solo una **landing page statica** che mostra le informazioni del gioco. 

L'applicazione vera e propria (con multiplayer in tempo reale) deve essere deployata su un servizio che supporta .NET e SignalR:
- **Azure App Service** (https://azure.microsoft.com/)
- **Railway** (https://railway.app/)
- **Render** (https://render.com/)

Consulta il README per le istruzioni di deploy complete.

#### 🔄 Aggiornamenti Automatici
Ogni volta che fai modifiche ai file nella cartella `/docs` e li pushai su GitHub:
- Il workflow GitHub Actions si attiverà automaticamente
- La landing page verrà aggiornata in pochi minuti
- Non devi fare nulla manualmente!

## 🎨 Personalizzazione della Landing Page

Se vuoi modificare la landing page:
1. Modifica il file `/docs/index.html`
2. Fai commit e push su GitHub
3. Il sito si aggiornerà automaticamente

## ❓ Problemi?

Se la landing page non si carica:
1. Controlla che GitHub Pages sia configurato correttamente (vedi Step 2)
2. Verifica che il workflow nelle Actions sia completato con successo
3. Attendi qualche minuto - il primo deploy può richiedere tempo
4. Prova a ricaricare la pagina con Ctrl+F5 (refresh completo)

## 📚 Risorse Utili

- [Documentazione GitHub Pages](https://docs.github.com/en/pages)
- [Documentazione GitHub Actions](https://docs.github.com/en/actions)
- [Repository Impostore](https://github.com/Moncymr/Impostore)
