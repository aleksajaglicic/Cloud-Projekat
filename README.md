# Lični Kripto Portfolio

Ovaj projekat simulira lični kripto portfolio koji omogućava korisnicima da prate trenutnu vrednost svojih kriptovaluta. 
Aplikacija omogućava korisnicima da unesu transakcije (kupovine i prodaje), prate profit i gubitak po svakoj kriptovaluti, 
kao i ukupnu vrednost svog portfolija.

## Funkcionalnosti

### Web Role:
- **Registracija korisnika**: Omogućava korisnicima da se registruju unosom ličnih podataka (ime, prezime, email, telefon, lozinka, itd.).
- **Logovanje korisnika**: Korisnici se loguju putem email-a i lozinke.
- **Izmena korisničkog profila**: Mogućnost izmene korisničkih podataka, uključujući ime, adresu i lozinku.
- **Pregled portfolija**: Pregled svih kriptovaluta koje korisnik poseduje.
- **Unos novih transakcija**: Unos novih kupovina ili prodaja kriptovaluta sa podacima o datumu, količini i vrednosti.
- **Brisanje transakcija**: Mogućnost brisanja ranije unetih transakcija.
- **Prikaz profita/gubitka**: Prikaz trenutnog profita ili gubitka za svaku kriptovalutu.
- **Prikaz ukupne vrednosti portfolija**: Prikaz ukupne vrednosti svih kriptovaluta u portfoliju.

### Servisi:
- **PortfolioService**: Glavni servis za upravljanje portfolijom, obrada zahteva za unos, izmenu i brisanje podataka o kriptovalutama i transakcijama.
- **NotificationService**: Servis za slanje email obavestenja korisnicima kada profit na određenoj kriptovaluti pređe zadati iznos.
- **HealthMonitoringService**: Servis za praćenje dostupnosti sistema i obaveštavanje o mogućim greškama ili neispravnostima.
- **HealthStatusService**: Web aplikacija koja prikazuje status servisa i informacije o dostupnosti sistema.

## Tehnologije

- **Frontend**: Web aplikacija razvijena pomoću React.js i TypeScript-a.
- **Backend**: C# (ASP.NET) za implementaciju servisa i API-ja.
- **Baza podataka**: Azure Storage za čuvanje podataka o korisnicima i njihovim transakcijama.
- **Email obavštenja**: Servis za slanje email notifikacija korisnicima.
- **API za kriptovalute**: Integracija sa eksternim API-ima za dohvat trenutnih vrednosti kriptovaluta.

## Instalacija

Pratite ove korake za instalaciju i pokretanje projekta:

### 1. Klonirajte repozitorijum

```bash
git clone https://github.com/aleksajaglicic/crypto-portfolio.git
cd crypto-portfolio
```
### 2. Pokrenite aplikaciju u Visual Studio-u
