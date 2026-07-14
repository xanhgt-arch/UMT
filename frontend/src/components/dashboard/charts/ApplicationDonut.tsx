import { useMemo } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { useChartFilters } from "@/lib/filter-context";
import { filterApplicationUsage, filterRawSessions } from "@/lib/filtering";
import { normalizeAppName } from "@/lib/mock-data";
import type { FilterDim } from "@/lib/types";
import { num, pct } from "@/lib/format";
import { useChartDownload } from "@/components/dashboard/chart-download";
import { isLightFill, pickTextOnFill, usePaletteVersion } from "./segment-label";

/** Per-application breakdown by CAD platform and product line. */
type AppBreakdown = {
  cad: Map<string, number>;
  productLine: Map<string, number>;
  total: number;
};

/** "FLUIDS" → "Fluids"; "SEALING" → "Sealings" (the UI uses the plural
 *  consistently even though the MySQL data ships the singular); short
 *  tokens (FBD, FTS) stay uppercase. */
function prettyProductLine(value: string): string {
  const k = value.toLowerCase();
  if (k.startsWith("sealing")) return "Sealings";
  if (k.startsWith("fluid")) return "Fluids";
  if (value.length <= 3) return value;
  return value.charAt(0).toUpperCase() + value.slice(1).toLowerCase();
}

/**
 * Custom tooltip — for the hovered slice we show:
 *   1. application name + run total + % of grand total
 *   2. CAD split (CATIA vs NX)
 *   3. Product-line split (Fluids, Sealings, …)
 *
 * Each row shows count + share-of-this-slice. Driven by the same
 * filtered raw sessions that produced the slices, so the numbers
 * always reconcile with the chart even under active filters.
 */
function DonutTooltip({
  active,
  payload,
  breakdowns,
  grand,
  swatchOf,
}: {
  active?: boolean;
  payload?: ReadonlyArray<{ name?: string | number; value?: number | string; payload?: { name?: string } }>;
  breakdowns: Map<string, AppBreakdown>;
  grand: number;
  swatchOf: (name: string) => string;
}) {
  if (!active || !payload || payload.length === 0) return null;
  const entry = payload[0];
  const name = String(entry?.name ?? entry?.payload?.name ?? "");
  if (!name) return null;
  const total = Number(entry?.value ?? 0);
  const b = breakdowns.get(name);
  const sharePct = grand > 0 ? Math.round((total / grand) * 100) : 0;

  const cadEntries = b
    ? [...b.cad.entries()].sort((a, c) => c[1] - a[1])
    : [];
  // Product-line section is intentionally limited to the two flagship lines
  // (Fluids and Sealings). Other lines (FBD, FTS, …) are aggregated under
  // these in the rest of the dashboard, so showing them here would add
  // noise rather than insight. The MySQL data uses "SEALING" (singular)
  // while the UI label is "Sealings" (plural) — match both, plus startsWith
  // so any future variants like "SEALING_FOAM" still get picked up.
  const plEntries = b
    ? [...b.productLine.entries()]
        .filter(([pl]) => {
          const k = pl.toLowerCase();
          return k.startsWith("fluid") || k.startsWith("sealing");
        })
        .sort((a, c) => c[1] - a[1])
    : [];

  return (
    <div
      className="relative rounded-xl border border-border px-3 py-2.5 text-[13px] text-foreground shadow-xl"
      style={{
        // Inline `backgroundColor` (rather than the `bg-card` utility) so the
        // tooltip is guaranteed fully opaque — Tailwind utilities can lose to
        // sibling rules in some chart contexts, leaving the popover see-through.
        backgroundColor: "var(--card)",
        // New stacking context — the recharts <svg> slice labels are siblings
        // of this tooltip's wrapper div; without an explicit stacking context
        // the SVG numbers can paint on top of the popover.
        isolation: "isolate",
        minWidth: 220,
      }}
    >
      <div className="flex items-center gap-2">
        <span
          className="block size-2.5 shrink-0 rounded-full"
          style={{ background: swatchOf(name) }}
        />
        <span className="font-semibold text-foreground">{name}</span>
      </div>
      <div className="mt-1 flex items-baseline gap-2">
        <span className="num text-base font-semibold tabular-nums">{num(total)}</span>
        <span className="text-xs text-muted-foreground">runs · {sharePct}% of total</span>
      </div>

      {cadEntries.length > 0 ? (
        <>
          <div className="mt-2 text-[10px] font-medium uppercase tracking-wider text-muted-foreground">
            CAD platform
          </div>
          <ul className="mt-1 space-y-0.5">
            {cadEntries.map(([cad, n]) => (
              <li key={cad} className="flex items-center justify-between gap-3">
                <span className="text-muted-foreground">{cad}</span>
                <span className="num tabular-nums">
                  {num(n)}
                  <span className="ml-1 text-[10px] text-muted-foreground">
                    {total > 0 ? pct(n / total) : ""}
                  </span>
                </span>
              </li>
            ))}
          </ul>
        </>
      ) : null}

      {plEntries.length > 0 ? (
        <>
          <div className="mt-2 text-[10px] font-medium uppercase tracking-wider text-muted-foreground">
            Product line
          </div>
          <ul className="mt-1 space-y-0.5">
            {plEntries.map(([pl, n]) => (
              <li key={pl} className="flex items-center justify-between gap-3">
                <span className="text-muted-foreground">{prettyProductLine(pl)}</span>
                <span className="num tabular-nums">
                  {num(n)}
                  <span className="ml-1 text-[10px] text-muted-foreground">
                    {total > 0 ? pct(n / total) : ""}
                  </span>
                </span>
              </li>
            ))}
          </ul>
        </>
      ) : null}
    </div>
  );
}

export const APP_DONUT_FILTER: { id: string; applicable: readonly FilterDim[] } = {
  id: "appDonut",
  applicable: ["range", "cad", "productLine", "region", "hardware"],
};

const PALETTE = [
  "var(--chart-1)",
  "var(--chart-2)",
  "var(--chart-3)",
  "var(--chart-4)",
  "var(--chart-5)",
  "var(--chart-6)",
];

type PieLabelProps = {
  cx?: number;
  cy?: number;
  midAngle?: number;
  innerRadius?: number;
  outerRadius?: number;
  value?: number;
  percent?: number;
  index?: number;
};

// Beyond the brand palette, stay on-brand by alternating between
// Cooper Standard blue (~250°) and gold (~80°) hues and stepping
// lightness so every slice past index 5 remains visually distinct
// without leaving the company colour story.
function colorAt(i: number, total: number): string {
  if (i < PALETTE.length) return PALETTE[i];
  const k = i - PALETTE.length;
  const extras = Math.max(total - PALETTE.length, 1);
  const hue = k % 2 === 0 ? 250 : 80;
  const steps = Math.max(Math.ceil(extras / 2), 1);
  const t = steps === 1 ? 0 : Math.floor(k / 2) / (steps - 1);
  const L = 0.50 + t * 0.32;
  const C = k % 2 === 0 ? 0.15 : 0.16;
  return `oklch(${L.toFixed(2)} ${C} ${hue})`;
}

export function ApplicationDonut() {
  usePaletteVersion();
  const { effective } = useChartFilters(APP_DONUT_FILTER.id, APP_DONUT_FILTER.applicable);
  const apps = useMemo(
    () => filterApplicationUsage(effective),
    [effective],
  );

  // Per-application CAD + product-line breakdowns for the rich tooltip.
  // Keyed by `normalizeAppName(s.application)` to line up exactly with
  // the keys used by `filterApplicationUsage` (and therefore the slices).
  const breakdowns = useMemo(() => {
    const map = new Map<string, AppBreakdown>();
    for (const s of filterRawSessions(effective)) {
      const key = normalizeAppName(s.application);
      let b = map.get(key);
      if (!b) {
        b = { cad: new Map(), productLine: new Map(), total: 0 };
        map.set(key, b);
      }
      b.cad.set(s.cad, (b.cad.get(s.cad) ?? 0) + 1);
      b.productLine.set(s.productLine, (b.productLine.get(s.productLine) ?? 0) + 1);
      b.total += 1;
    }
    return map;
  }, [effective]);

  const grand = apps.reduce((s, a) => s + a.total, 0) || 0;
  const data = apps.map((a) => ({ name: a.application, value: a.total }));
  // Stable swatch lookup so the tooltip dot matches its slice colour
  // even with the dynamic palette beyond index 5.
  const swatchOf = (name: string): string => {
    const i = data.findIndex((d) => d.name === name);
    return colorAt(i < 0 ? 0 : i, data.length);
  };

  const exportRows = useMemo(
    () =>
      apps.map((a) => ({
        "KBE Tool": a.application,
        Runs: a.total,
        Share: grand > 0 ? pct(a.total / grand) : "0%",
      })),
    [apps, grand],
  );
  useChartDownload("tool-runs", exportRows);

  if (apps.length === 0) {
    return (
      <div className="grid h-[240px] place-items-center text-sm text-muted-foreground">
        No KBE tools match the current filter.
      </div>
    );
  }

  // Split the legend into two side-by-side columns so we don't need a
  // scrollbar — first column holds the busier half, second column the rest.
  const half = Math.ceil(apps.length / 2);
  const legendColumns = [apps.slice(0, half), apps.slice(half)];

  return (
    <div className="flex flex-col items-center justify-evenly gap-6 md:flex-row md:gap-0">
      <div className="relative aspect-square w-full max-w-[440px] shrink-0">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Tooltip
              wrapperStyle={{ outline: "none", zIndex: 50, pointerEvents: "none" }}
              content={(props) => (
                <DonutTooltip
                  active={props.active}
                  payload={props.payload as ReadonlyArray<{ name?: string | number; value?: number | string; payload?: { name?: string } }>}
                  breakdowns={breakdowns}
                  grand={grand}
                  swatchOf={swatchOf}
                />
              )}
            />
            <Pie
              data={data}
              dataKey="value"
              nameKey="name"
              innerRadius="50%"
              outerRadius="92%"
              paddingAngle={2}
              stroke="var(--card)"
              strokeWidth={3}
              labelLine={false}
              label={(props: unknown) => {
                const RADIAN = Math.PI / 180;
                const { cx = 0, cy = 0, midAngle = 0, innerRadius = 0, outerRadius = 0, value = 0, percent = 0, index = 0 } =
                  props as PieLabelProps;
                if (percent < 0.04) return null;
                const r = (innerRadius + outerRadius) / 2;
                const x = cx + r * Math.cos(-midAngle * RADIAN);
                const y = cy + r * Math.sin(-midAngle * RADIAN);
                const sliceColor = colorAt(index ?? 0, data.length);
                const fill = pickTextOnFill(sliceColor);
                // SVG attribute `fill=` does NOT resolve CSS var() — only the
                // `style` property does. Setting fill through style ensures
                // var(--foreground) actually paints black in light mode.
                const isLight = isLightFill(sliceColor);
                const textStyle = isLight
                  ? { fill }
                  : {
                      fill,
                      paintOrder: "stroke" as const,
                      stroke: "rgba(0,0,0,0.25)",
                      strokeWidth: 2,
                    };
                return (
                  <g>
                    <text
                      x={x}
                      y={y - 8}
                      textAnchor="middle"
                      dominantBaseline="central"
                      fontSize={13}
                      fontWeight={700}
                      style={textStyle}
                    >
                      {num(value)}
                    </text>
                    <text
                      x={x}
                      y={y + 8}
                      textAnchor="middle"
                      dominantBaseline="central"
                      fontSize={11}
                      fontWeight={600}
                      style={textStyle}
                    >
                      {`${Math.round(percent * 100)}%`}
                    </text>
                  </g>
                );
              }}
              isAnimationActive={false}
            >
              {data.map((_, i) => (
                <Cell key={i} fill={colorAt(i, data.length)} />
              ))}
            </Pie>
          </PieChart>
        </ResponsiveContainer>
        <div className="pointer-events-none absolute inset-0 flex flex-col items-center justify-center text-center">
          <div className="text-xs uppercase tracking-wide text-muted-foreground">Total runs</div>
          <div className="num mt-0.5 text-3xl font-semibold">{num(grand)}</div>
        </div>
      </div>

      {/* Two side-by-side legend columns instead of one long scrollable
          list. Each inner <ul> is its own 3-column grid (name | count |
          percent) where `display:contents` on the <li> lets the row's
          three cells participate in the parent grid so columns stay
          aligned across every row in that column. */}
      <div className="flex shrink-0 gap-6">
        {legendColumns.map((col, colIdx) => {
          const offset = colIdx === 0 ? 0 : half;
          return (
            <ul
              key={colIdx}
              className="grid w-fit grid-cols-[auto_auto_auto] items-center gap-x-3 gap-y-1 self-start"
            >
              {col.map((a, j) => {
                const i = offset + j; // absolute index → matches pie slice colour
                return (
                  <li
                    key={a.application}
                    className="contents [&>*]:rounded-md [&>*]:py-1 hover:[&>*]:bg-muted/40"
                  >
                    <div className="flex min-w-0 items-center gap-2 pl-1.5">
                      <span
                        className="block size-2.5 shrink-0 rounded-full"
                        style={{ background: colorAt(i, apps.length) }}
                      />
                      <div className="min-w-0">
                        <div
                          className="truncate text-[13px] font-medium leading-tight uppercase"
                          title={a.application}
                        >
                          {a.application}
                        </div>
                        {/* <div className="truncate text-[10px] leading-tight text-muted-foreground">
                          {a.cad} · {a.productLine}
                        </div> */}
                      </div>
                    </div>
                    <span className="num text-right text-[13px] font-semibold tabular-nums">
                      {num(a.total)}
                    </span>
                    <span className="num pr-1.5 text-right text-[10px] tabular-nums text-muted-foreground">
                      {pct(a.total / grand)}
                    </span>
                  </li>
                );
              })}
            </ul>
          );
        })}
      </div>
    </div>
  );
}
