import { NavLink } from "react-router-dom"
import type { ComponentType } from "react"
import { Home, Network, ShieldCheck, Users } from "lucide-react"

import { Logo } from "./Logo"
import { ParticleBackground } from "./ParticleBackground"
import { Separator } from "@/components/ui/separator"
import { preloadRoute } from "@/lib/route-preload"
import { useAuth } from "@/lib/auth-context"

type Item = {
  to: string
  icon: ComponentType<{ className?: string }>
  label: string
  description: string
}

const NAV_PRIMARY: Item[] = [
  {
    to: "/",
    icon: Home,
    label: "Home",
    description: "Headline overview",
  },
]

const NAV_ADMIN: Item[] = [
  {
    to: "/vdi",
    icon: Users,
    label: "VDI users",
    description: "Manage virtual desktop users",
  },
  {
    to: "/admins",
    icon: ShieldCheck,
    label: "Admins",
    description: "Manage UMT administrators",
  },
  {
    to: "/domains",
    icon: Network,
    label: "Domains",
    description: "Map domains to corporate groups",
  },
]

function linkClasses(isActive: boolean) {
  return [
    "group relative flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition-colors",
    isActive
      ? "bg-accent text-accent-foreground font-medium"
      : "text-sidebar-foreground hover:bg-sidebar-accent/60 hover:text-accent-foreground",
  ].join(" ")
}

function SidebarItem({ item }: { item: Item }) {
  const Icon = item.icon

  return (
    <NavLink
      to={item.to}
      end={item.to === "/"}
      className={({ isActive }) => linkClasses(isActive)}
      onFocus={() => preloadRoute(item.to)}
      onMouseEnter={() => preloadRoute(item.to)}
    >
      {({ isActive }) => (
        <>
          {isActive ? (
            <span
              aria-hidden
              className="pointer-events-none absolute top-1.5 bottom-1.5 left-0 w-[3px] rounded-full bg-[oklch(0.83_0.16_88)]"
            />
          ) : null}

          <Icon
            className={[
              "size-[18px] shrink-0 transition-colors",
              isActive
                ? "text-primary"
                : "text-muted-foreground group-hover:text-[#00539f] dark:group-hover:text-primary",
            ].join(" ")}
          />

          <span
            className={[
              "truncate transition-colors",
              isActive
                ? "text-primary"
                : "group-hover:text-[#00539f] dark:group-hover:text-primary",
            ].join(" ")}
          >
            {item.label}
          </span>
        </>
      )}
    </NavLink>
  )
}

export function Sidebar() {
  const { user, loading } = useAuth()

  const canSeeAdminMenu = !loading && user?.isAdmin === true

  return (
    <aside
      className="sticky top-0 hidden h-screen w-64 shrink-0 flex-col overflow-hidden border-r border-border bg-sidebar/82 backdrop-blur-md md:flex"
      aria-label="Primary"
    >
      <span
        aria-hidden
        className="brand-hairline-v pointer-events-none absolute top-0 right-0 bottom-0 z-20 w-[3px]"
      />

      <ParticleBackground variant="sidebar" />

      <div className="relative z-10 flex h-full flex-col overflow-y-auto">
        <div className="px-5 pt-6 pb-4">
          <Logo />
        </div>

        <nav className="flex flex-col gap-1 px-3" aria-label="Main pages">
          <div className="px-2 pb-1 text-[11px] font-medium uppercase tracking-wider text-muted-foreground">
            Workspace
          </div>

          {NAV_PRIMARY.map((item) => (
            <SidebarItem key={item.to} item={item} />
          ))}
        </nav>

        {canSeeAdminMenu ? (
          <>
            <Separator className="my-4" />

            <nav
              className="flex flex-col gap-1 px-3"
              aria-label="Administration"
            >
              <div className="px-2 pb-1 text-[11px] font-medium uppercase tracking-wider text-muted-foreground">
                Administration
              </div>

              {NAV_ADMIN.map((item) => (
                <SidebarItem key={item.to} item={item} />
              ))}
            </nav>
          </>
        ) : null}

        <div className="mt-auto p-4">
          <a
            href="https://www.tatatechnologies.com"
            target="_blank"
            rel="noreferrer noopener"
            className="mt-3 flex flex-col items-center justify-center gap-1.5 rounded-lg border border-border bg-card/60 px-3 py-3 transition-colors hover:border-[oklch(0.83_0.16_88_/_0.55)] hover:bg-card"
            aria-label="Powered by Tata Technologies"
          >
            <span className="text-[9px] font-medium uppercase tracking-[0.14em] text-muted-foreground">
              Powered by
            </span>

            <img
              src="/tata-tech-logo.png"
              alt="Tata Technologies"
              className="h-2.5 w-auto max-w-full select-none object-contain dark:brightness-110"
              draggable={false}
            />
          </a>
        </div>
      </div>
    </aside>
  )
}