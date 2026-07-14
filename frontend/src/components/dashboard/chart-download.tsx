import { createContext, useContext, useEffect } from "react";
import { Download } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { downloadCsv, type CsvRow } from "@/lib/csv";

export type ChartDownload = {
  /** Base file name (no extension) — slugified when the file is saved. */
  filename: string;
  /** Rows to export, already shaped with the column headers you want. */
  rows: ReadonlyArray<CsvRow>;
};

/**
 * ChartCard provides this so any chart rendered inside it can register the data
 * it's currently showing. Because charts hand over the same rows they plot, the
 * export always reflects the active filters.
 */
export const ChartDownloadContext = createContext<
  ((download: ChartDownload | null) => void) | null
>(null);

/**
 * Charts call this to make their current (filtered) data downloadable from the
 * enclosing ChartCard. Pass an empty `rows` array (e.g. when a filter wipes the
 * data) and the download button hides itself.
 *
 * `rows` should be a stable reference (wrap it in `useMemo`) so the chart
 * doesn't re-register on every render.
 *
 * Safe to call even when the chart isn't wrapped in a ChartCard — it no-ops.
 */
export function useChartDownload(
  filename: string,
  rows: ReadonlyArray<CsvRow>,
): void {
  const register = useContext(ChartDownloadContext);
  useEffect(() => {
    if (!register) return;
    register(rows.length > 0 ? { filename, rows } : null);
    return () => register(null);
  }, [register, filename, rows]);
}

export function ChartDownloadButton({ download }: { download: ChartDownload }) {
  const handleClick = () => {
    downloadCsv(download.filename, download.rows);
    const count = download.rows.length;
    toast.success(`Downloaded ${count} row${count === 1 ? "" : "s"} as CSV`);
  };

  return (
    <Button
      type="button"
      variant="outline"
      size="sm"
      className="h-8 gap-2 rounded-full px-3 text-xs"
      onClick={handleClick}
      aria-label="Download chart data as CSV"
      title="Download data (CSV)"
    >
      <Download className="size-3.5" />
      Download
    </Button>
  );
}
