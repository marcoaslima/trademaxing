/**
 * Formats a date into Brazilian Portuguese standard format (DD/MM/YYYY)
 */
export function formatDateBR(dateInput?: string | Date | null): string {
  if (!dateInput) return '-';

  if (typeof dateInput === 'string') {
    // Standard ISO string YYYY-MM-DD
    const isoMatch = dateInput.match(/^(\d{4})-(\d{2})-(\d{2})/);
    if (isoMatch) {
      const [, year, month, day] = isoMatch;
      return `${day}/${month}/${year}`;
    }
    // Brazilian string DD/MM/YYYY
    const brMatch = dateInput.match(/^(\d{2})\/(\d{2})\/(\d{4})/);
    if (brMatch) {
      return dateInput;
    }
  }

  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput;
  if (isNaN(date.getTime())) return '-';

  const day = String(date.getUTCDate()).padStart(2, '0');
  const month = String(date.getUTCMonth() + 1).padStart(2, '0');
  const year = date.getUTCFullYear();
  return `${day}/${month}/${year}`;
}

/**
 * Auto-masks text input into Brazilian format DD/MM/YYYY as the user types
 */
export function maskDateBR(value: string): string {
  if (!value) return '';
  const digits = value.replace(/\D/g, '').slice(0, 8);
  if (digits.length <= 2) return digits;
  if (digits.length <= 4) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
  return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4, 8)}`;
}

/**
 * Parses DD/MM/YYYY or YYYY-MM-DD to ISO date string YYYY-MM-DD
 */
export function parseDateBRToISO(inputStr: string): string {
  if (!inputStr) {
    const today = new Date();
    const y = today.getFullYear();
    const m = String(today.getMonth() + 1).padStart(2, '0');
    const d = String(today.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  const brMatch = inputStr.trim().match(/^(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})/);
  if (brMatch) {
    const [, day, month, year] = brMatch;
    const padDay = day.padStart(2, '0');
    const padMonth = month.padStart(2, '0');
    return `${year}-${padMonth}-${padDay}`;
  }

  const isoMatch = inputStr.trim().match(/^(\d{4})-(\d{2})-(\d{2})/);
  if (isoMatch) {
    return inputStr.trim().substring(0, 10);
  }

  return inputStr;
}
