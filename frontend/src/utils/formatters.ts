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
