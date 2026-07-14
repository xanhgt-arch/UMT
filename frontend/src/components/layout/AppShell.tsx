import { Outlet } from "react-router-dom"
import { TooltipProvider } from "@/components/ui/tooltip"
import { Toaster } from "@/components/ui/sonner"
import { Sidebar } from "./Sidebar"
import { Topbar } from "./Topbar"
import { ParticleBackground } from "./ParticleBackground"
import { FilterProvider } from "@/lib/filter-context"
import { AdminDataProvider } from "@/lib/admin-data"
import { DashboardSearchProvider } from "@/lib/search-context"

export function AppShell() {
  return (
    <FilterProvider>
      <AdminDataProvider>
        <DashboardSearchProvider>
          <TooltipProvider delayDuration={250}>
            <ParticleBackground />
            {/* Brand hairline - the only place blue + gold meet, echoing
              the swoosh in the Cooper Standard mark. */}
            <div
              aria-hidden
              className="fixed inset-x-0 top-0 z-50 h-[3px] brand-hairline"
            />

            <div className="relative z-10 flex min-h-screen text-foreground">
              <Sidebar />
              <div className="flex min-w-0 flex-1 flex-col">
                <Topbar />
                <main className="mx-auto w-full max-w-[1280px] flex-1 px-4 py-8 md:px-8 md:py-10">
                  <Outlet />
                </main>
                <footer className="relative border-t border-border px-4 py-4 text-xs text-muted-foreground md:px-8">
                  {/* Faint gold spark above the footer - closes the page with
                    the same brand pulse that opens it at the very top. */}
                  <div
                    aria-hidden
                    className="pointer-events-none absolute inset-x-0 -top-px h-[2px] brand-spark opacity-60"
                  />
                  <div className="relative flex flex-col items-center justify-center gap-2 text-center md:gap-1">
                    <div className="flex flex-wrap items-center justify-center gap-x-2">
                  <span className="font-semibold tracking-tight">
                    <span className="text-[#00539f] dark:text-primary">
                      Cooper
                    </span>
                    <span className="text-[#00539f] dark:text-primary">
                      Standard
                    </span>
                  </span>
                  <span className="text-border">·</span>
                  <span>UMT — Usage Monitoring Tool</span>
                  <span className="text-border">·</span>
                  <span>Internal use only</span>
                  </div>
                  <a
                      href="https://www.tatatechnologies.com"
                      target="_blank"
                      rel="noreferrer noopener"
                      className="flex items-center gap-2 rounded-md px-1.5 py-0.5 transition-colors hover:bg-accent/40 md:absolute md:top-1/2 md:right-0 md:-translate-y-1/2"
                      aria-label="Powered by Tata Technologies"
                    >
                      <span className="shrink-0 text-[10px] font-medium uppercase tracking-wider text-muted-foreground">
                        Powered by
                      </span>
                      <img
                        src="/tata-tech-logo.png"
                        alt="Tata Technologies"
                        className="h-2.5 w-auto shrink-0 select-none object-contain dark:brightness-110 mb-0.5"
                        draggable={false}
                      />
                    </a>
                  </div>
                </footer>
              </div>
            </div>
            <Toaster richColors position="top-right" />
          </TooltipProvider>
        </DashboardSearchProvider>
      </AdminDataProvider>
    </FilterProvider>
  )
}
