// Tiny CSV helper for "download this chart's data" buttons. The rows handed in
// are already the exact values a chart is showing (filters applied), so the
// export always matches what the user sees on screen.

export type CsvValue = string | number | boolean | null | undefined;
export type CsvRow = Record<string, CsvValue>;

/** RFC-4180 style cell quoting — wrap in quotes when the value contains a
 *  comma, quote or newline, and double up any embedded quotes. */
function escapeCell(value: CsvValue): string {
  if (value === null || value === undefined) return "";
  const s = String(value);
  return /[",\n\r]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s;
}

/** Build CSV text from a list of row objects. Headers are the union of every
 *  row's keys, in first-seen order, so rows with dynamic columns (e.g. one
 *  column per year/app/region) still line up. */
export function rowsToCsv(rows: ReadonlyArray<CsvRow>): string {
  if (rows.length === 0) return "";

  const headers: string[] = [];
  const seen = new Set<string>();
  for (const row of rows) {
    for (const key of Object.keys(row)) {
      if (!seen.has(key)) {
        seen.add(key);
        headers.push(key);
      }
    }
  }

  const lines = [headers.map(escapeCell).join(",")];
  for (const row of rows) {
    lines.push(headers.map((h) => escapeCell(row[h])).join(","));
  }
  return lines.join("\r\n");
}

/** Slugify a chart title/name into a safe file stem. */
function slugify(name: string): string {
  const slug = name
    .replace(/[^a-z0-9]+/gi, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();
  return slug || "chart-data";
}

/** Turn rows into a CSV file and trigger a browser download. A UTF-8 BOM is
 *  prepended so Excel opens the file with the right encoding. */
export function downloadCsv(filename: string, rows: ReadonlyArray<CsvRow>): void {
  const csv = rowsToCsv(rows);
  const blob = new Blob(["﻿" + csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = `${slugify(filename)}.csv`;
  document.body.appendChild(anchor);
  anchor.click();
  document.body.removeChild(anchor);
  URL.revokeObjectURL(url);
}
