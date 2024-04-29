# ClinicaCare360 - Sistema di Gestione Clinica

## Descrizione
ClinicaCare360 è un'applicazione web progettata per facilitare la gestione efficiente di una clinica medica. L'applicazione permette una gestione completa degli appuntamenti, degli utenti registrati e del personale medico.

## Caratteristiche

### Lato Utente
- **Accesso Pubblico:**
  - Visualizzazione di post con consigli e promozioni sulla home page.
  - Accesso alle informazioni dettagliate sulla clinica e sul personale medico.
  - Consultazione dell'elenco delle prestazioni offerte e dei relativi costi.

- **Utenti Registrati:**
  - Prenotazione di appuntamenti.
  - Accesso a un profilo personale con visualizzazione di appuntamenti futuri e storico delle visite passate.
  - Visualizzazione delle prescrizioni e dei farmaci prescritti.

### Lato Amministratore
- Gestione dei post pubblicati sulla piattaforma.
- Amministrazione dell'elenco dei medici e delle prestazioni offerte.
- Gestione della disponibilità degli appuntamenti.
- Accesso completo alla lista dei pazienti con storico dettagliato di visite e prescrizioni.

## Tecnologie Utilizzate
- **Frontend:** HTML, CSS, Bootstrap
- **Backend:** C#, ASP.NET Framework MVC, Entity Framework
- **Database:** MySQL, gestito tramite SQL Server Management Studio 2019

## Installazione
Per eseguire questo progetto localmente, seguire i seguenti passaggi:

1. Clonare il repository:
   git clone https://github.com/ramonasecci/CliniCare360.git
2. Aprire il progetto con Visual Studio.
3. Assicurarsi che le stringhe di connessione nel file `Web.config` siano corrette e che punti al nuovo server di database locale.

## Importazione del Database
Per configurare il database utilizzando il file `.bacpac` incluso, seguire le istruzioni per la piattaforma desiderata:

### SQL Server Management Studio (SSMS)
1. Apri SSMS e connettiti al tuo server SQL.
2. Clicca destro su "Databases" e seleziona "Import Data-tier Application...".
3. Segui la procedura guidata e seleziona il file `.bacpac` scaricato dal repository GitHub.
4. Completa l'importazione seguendo le istruzioni a schermo.
