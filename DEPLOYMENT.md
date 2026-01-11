# 🚀 Guida al Deployment - Hosting Gratuito per Impostore

Questa guida ti mostra come pubblicare il tuo sito Impostore online **gratuitamente** utilizzando diverse piattaforme di hosting.

## 📊 Confronto Piattaforme Gratuite

| Piattaforma | Gratuito | Facile da Usare | Supporto .NET | Raccomandato |
|-------------|----------|-----------------|---------------|--------------|
| **Railway** | ✅ $5/mese gratis | ⭐⭐⭐⭐⭐ | ✅ Eccellente | **🏆 MIGLIORE** |
| **Render** | ✅ 750h/mese | ⭐⭐⭐⭐ | ✅ Buono | ⭐⭐⭐⭐ |
| **Azure App Service** | ✅ Limitato | ⭐⭐⭐ | ✅ Eccellente | ⭐⭐⭐ |

---

## 🏆 Opzione 1: Railway (CONSIGLIATO)

Railway è la **scelta migliore** per pubblicare Impostore gratuitamente. Offre $5 di credito mensile gratuito e setup automatico.

### Vantaggi
- ✅ Setup automatico (rileva .NET automaticamente)
- ✅ $5 credito mensile gratuito
- ✅ Deploy automatico da GitHub
- ✅ SSL/HTTPS gratuito
- ✅ Dominio personalizzato gratuito
- ✅ Logs in tempo reale

### Passo 1: Crea un Account Railway

1. Vai su [railway.app](https://railway.app)
2. Clicca su **"Start a New Project"**
3. Effettua il login con GitHub

### Passo 2: Connetti il Repository

1. Nella dashboard di Railway, clicca su **"New Project"**
2. Seleziona **"Deploy from GitHub repo"**
3. Autorizza Railway ad accedere ai tuoi repository
4. Seleziona il repository **Impostore**

### Passo 3: Configura il Progetto

Railway rileverà automaticamente che si tratta di un progetto .NET. Se necessario, puoi personalizzare:

1. Nella sezione **Settings**, verifica:
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ImpostoreGame.dll`
   - **Port**: Railway imposterà automaticamente la variabile `PORT`

### Passo 4: Deploy

1. Railway inizierà automaticamente il build e il deploy
2. Attendi che il deploy sia completato (circa 2-5 minuti)
3. Clicca su **"Generate Domain"** per ottenere un URL pubblico
4. Il tuo sito sarà disponibile all'indirizzo: `https://impostore-production.up.railway.app`

### Passo 5: Deploy Automatici

Ogni volta che fai un `git push` sul branch principale, Railway farà automaticamente un nuovo deploy!

### Configurazione Avanzata (Opzionale)

Crea un file `railway.toml` nella root del progetto per personalizzare il deploy:

```toml
[build]
builder = "nixpacks"

[deploy]
startCommand = "dotnet out/ImpostoreGame.dll"
restartPolicyType = "on_failure"
restartPolicyMaxRetries = 10
```

---

## ⭐ Opzione 2: Render

Render offre 750 ore mensili gratuite e supporta bene .NET.

### Vantaggi
- ✅ 750 ore mensili gratuite
- ✅ SSL gratuito
- ✅ Deploy automatico da GitHub
- ⚠️ Il servizio gratuito "dorme" dopo 15 minuti di inattività

### Passo 1: Crea un Account Render

1. Vai su [render.com](https://render.com)
2. Clicca su **"Get Started"**
3. Registrati con GitHub

### Passo 2: Crea un Nuovo Web Service

1. Nella dashboard, clicca su **"New +"** → **"Web Service"**
2. Connetti il tuo repository GitHub **Impostore**
3. Clicca su **"Connect"**

### Passo 3: Configura il Servizio

Compila il form con questi valori:

- **Name**: `impostore-game` (o un nome a tua scelta)
- **Region**: Scegli la regione più vicina a te
- **Branch**: `main` (o il tuo branch principale)
- **Runtime**: Seleziona **".NET"**
- **Build Command**: 
  ```bash
  dotnet publish -c Release -o out
  ```
- **Start Command**: 
  ```bash
  cd out && dotnet ImpostoreGame.dll --urls http://0.0.0.0:$PORT
  ```
- **Plan**: Seleziona **"Free"**

### Passo 4: Variabili d'Ambiente

Aggiungi queste variabili d'ambiente nella sezione **Environment**:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

### Passo 5: Deploy

1. Clicca su **"Create Web Service"**
2. Render inizierà il build e il deploy (circa 5-10 minuti)
3. Una volta completato, il tuo sito sarà disponibile su: `https://impostore-game.onrender.com`

### Nota Importante sul Piano Gratuito

⚠️ Con il piano gratuito di Render, il tuo servizio:
- Si "addormenta" dopo 15 minuti di inattività
- Richiede ~30 secondi per "svegliarsi" alla prima richiesta
- Perfetto per demo e progetti personali

### Configurazione con File (Opzionale)

Crea un file `render.yaml` nella root del progetto:

```yaml
services:
  - type: web
    name: impostore-game
    runtime: docker
    plan: free
    buildCommand: dotnet publish -c Release -o out
    startCommand: cd out && dotnet ImpostoreGame.dll --urls http://0.0.0.0:$PORT
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:$PORT
```

---

## ☁️ Opzione 3: Azure App Service (Free Tier)

Azure offre un tier gratuito con limitazioni, ottimo per testing.

### Vantaggi
- ✅ Tier F1 gratuito disponibile
- ✅ Eccellente supporto .NET (piattaforma nativa Microsoft)
- ✅ Integrazione con Visual Studio
- ⚠️ Richiede carta di credito (non verrà addebitato nulla nel tier gratuito)
- ⚠️ Più complesso da configurare

### Prerequisiti

- Account Azure (registrati su [azure.microsoft.com](https://azure.microsoft.com))
- Azure CLI installato ([scarica qui](https://docs.microsoft.com/cli/azure/install-azure-cli))

### Passo 1: Installa Azure CLI

```bash
# Windows (con winget)
winget install Microsoft.AzureCLI

# macOS
brew install azure-cli

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### Passo 2: Login in Azure

```bash
az login
```

Questo aprirà il browser per l'autenticazione.

### Passo 3: Crea un Resource Group

```bash
az group create --name ImpostoreResourceGroup --location westeurope
```

### Passo 4: Crea un App Service Plan (Free Tier)

```bash
az appservice plan create \
  --name ImpostorePlan \
  --resource-group ImpostoreResourceGroup \
  --sku F1 \
  --is-linux
```

**Nota**: `F1` è il tier completamente gratuito!

### Passo 5: Crea la Web App

```bash
az webapp create \
  --name impostore-game-[tuo-nome-unico] \
  --resource-group ImpostoreResourceGroup \
  --plan ImpostorePlan \
  --runtime "DOTNET:10"
```

⚠️ Sostituisci `[tuo-nome-unico]` con un nome unico (es. `impostore-game-mario123`)

### Passo 6: Deploy da Repository Locale

```bash
# Nella directory del progetto
az webapp up \
  --name impostore-game-[tuo-nome-unico] \
  --resource-group ImpostoreResourceGroup \
  --runtime "DOTNET:10"
```

### Passo 7: Configura le Impostazioni

```bash
# Imposta variabili d'ambiente
az webapp config appsettings set \
  --name impostore-game-[tuo-nome-unico] \
  --resource-group ImpostoreResourceGroup \
  --settings ASPNETCORE_ENVIRONMENT=Production
```

### Accedi alla Tua App

Il tuo sito sarà disponibile su:
```
https://impostore-game-[tuo-nome-unico].azurewebsites.net
```

### Deploy Continuo da GitHub (Opzionale)

Puoi configurare il deploy automatico da GitHub:

```bash
az webapp deployment source config \
  --name impostore-game-[tuo-nome-unico] \
  --resource-group ImpostoreResourceGroup \
  --repo-url https://github.com/[tuo-username]/Impostore \
  --branch main \
  --manual-integration
```

---

## 🔧 Risoluzione Problemi

### Problema: "Application Error" dopo il Deploy

**Soluzione**: Verifica che la porta sia configurata correttamente.

Aggiungi nel `Program.cs` (se non presente):

```csharp
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
```

### Problema: SignalR non Funziona in Produzione

**Soluzione**: Assicurati che WebSockets siano abilitati.

Per Render/Railway, aggiungi in `Program.cs`:

```csharp
app.UseWebSockets();
```

Per Azure:

```bash
az webapp config set \
  --name [tuo-app-name] \
  --resource-group ImpostoreResourceGroup \
  --web-sockets-enabled true
```

### Problema: Database In-Memory si Svuota

**Soluzione**: È normale! Il database InMemory viene resettato ad ogni riavvio dell'applicazione.

Se vuoi persistenza, considera l'upgrade a SQLite (già incluso nel progetto):

```csharp
// In Program.cs, sostituisci UseInMemoryDatabase con:
options.UseSqlite("Data Source=impostore.db");
```

### Problema: "Too Many Requests" su Railway

**Soluzione**: Hai esaurito il credito gratuito di $5/mese. Opzioni:
1. Aspetta il mese successivo
2. Aggiungi credito al tuo account
3. Passa a Render o Azure

---

## 📊 Monitoraggio e Logs

### Railway
- Accedi alla dashboard → Seleziona il tuo progetto → Tab **"Deployments"**
- Vedi i logs in tempo reale

### Render
- Dashboard → Seleziona il tuo servizio → Tab **"Logs"**
- Logs persistiti per 7 giorni

### Azure
```bash
# Stream dei logs in tempo reale
az webapp log tail \
  --name impostore-game-[tuo-nome] \
  --resource-group ImpostoreResourceGroup
```

---

## 🌐 Domini Personalizzati

### Railway
1. Dashboard → Settings → Domains
2. Clicca su **"Custom Domain"**
3. Aggiungi il tuo dominio (es. `impostore.tuosito.com`)
4. Configura il record DNS come indicato

### Render
1. Dashboard → Settings → Custom Domain
2. Aggiungi il dominio
3. Configura il CNAME nel tuo provider DNS

### Azure
```bash
az webapp config hostname add \
  --webapp-name impostore-game-[tuo-nome] \
  --resource-group ImpostoreResourceGroup \
  --hostname www.tuodominio.com
```

---

## ✅ Checklist Pre-Deploy

Prima di fare il deploy, assicurati di:

- [ ] Aver testato l'applicazione localmente con `dotnet run`
- [ ] Aver verificato che SignalR funzioni correttamente
- [ ] Aver configurato `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Aver rimosso eventuali segreti o chiavi dal codice
- [ ] Aver aggiornato il file `appsettings.json` se necessario
- [ ] Aver testato l'applicazione su dispositivi mobili (se applicabile)

---

## 🆘 Hai Bisogno di Aiuto?

- 📖 [Documentazione Railway](https://docs.railway.app)
- 📖 [Documentazione Render](https://render.com/docs)
- 📖 [Documentazione Azure](https://docs.microsoft.com/azure/app-service/)
- 💬 Apri un [Issue su GitHub](https://github.com/Moncymr/Impostore/issues)

---

## 🎉 Congratulazioni!

Hai pubblicato con successo il tuo gioco Impostore online! 

Condividi l'URL con i tuoi amici e divertitevi! 🎭🎮

**Link Utili:**
- 🏠 [Torna al README](README.md)
- 🐛 [Segnala Bug](https://github.com/Moncymr/Impostore/issues)
- ⭐ [Dai una stella al progetto!](https://github.com/Moncymr/Impostore)
