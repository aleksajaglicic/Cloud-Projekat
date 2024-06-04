import LoginData from "../interfaces/ILogin";

/**
 * Kreira sesiju za korisnika i čuva podatke u localStorage.
 * @param {string} email - Email korisnika.
 * @param {string} password - Lozinka korisnika.
 * @returns {boolean} - Vraća true ako je sesija uspešno kreirana, inače false.
 */
export function create_session(user: LoginData): Boolean {
  // Proverava da li su prosleđeni email i lozinka
  if (user.Email !== "" && user.Password !== "") {
      localStorage.setItem('user', JSON.stringify({ Email: user.Email, Password: user.Password }));
      return true; // Sesija je uspešno kreirana
  } else {
      return false; // Sesija nije kreirana zbog nedostajućih podataka
  }
};

/**
* Proverava da li postoje sačuvani korisnički podaci u localStorage-u.
* @returns {LoginData|null} - Vraća parsirane korisničke podatke ako postoje, inače null.
*/
export function check_session (): LoginData | null {
  const savedData = localStorage.getItem('user');

  if (savedData) {
      try {
          const parsedData :LoginData = JSON.parse(savedData);
          return parsedData; // Vraća korisničke podatke ako su prisutni
      } catch (error) {
          console.error('Error parsing stored user data');
          return null; // Vraća null ako dođe do greške pri parsiranju
      }
  } else {
      return null; // Vraća null ako nema podataka u localStorage-u
  }
};

/**
* Završava korisničku sesiju brisanjem podataka iz localStorage-a.
*/
export function end_session (): void {
  localStorage.removeItem('user');
  console.warn('The session has ended. User data has been deleted.');
};