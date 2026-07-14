import { lazy, Suspense, type ReactNode } from "react"
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom"

import { AppLoadingScreen } from "@/components/AppLoadingScreen"
import { AppShell } from "@/components/layout/AppShell"
import { AuthProvider, useAuth } from "@/lib/auth-context"
import { routePreloaders } from "@/lib/route-preload"

const HomePage = lazy(routePreloaders["/"])
const ReportsPage = lazy(routePreloaders["/reports"])
const SessionsPage = lazy(routePreloaders["/sessions"])
const VdiUsersPage = lazy(routePreloaders["/vdi"])
const AdminsPage = lazy(routePreloaders["/admins"])
const DomainsPage = lazy(routePreloaders["/domains"])
const NotFoundPage = lazy(routePreloaders["*"])

function AdminOnlyRoute({ children }: { children: ReactNode }) {
  const { user, loading } = useAuth()

  if (loading) {
    return <AppLoadingScreen label="Checking user access" />
  }

  if (!user?.isAdmin) {
    return <Navigate to="/" replace />
  }

  return <>{children}</>
}

export function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Suspense fallback={<AppLoadingScreen />}>
          <Routes>
            <Route element={<AppShell />}>
              <Route index element={<HomePage />} />

              <Route
                path="/reports"
                element={
                  <AdminOnlyRoute>
                    <ReportsPage />
                  </AdminOnlyRoute>
                }
              />

              <Route
                path="/sessions"
                element={
                  <AdminOnlyRoute>
                    <SessionsPage />
                  </AdminOnlyRoute>
                }
              />

              <Route
                path="/vdi"
                element={
                  <AdminOnlyRoute>
                    <VdiUsersPage />
                  </AdminOnlyRoute>
                }
              />

              <Route
                path="/admins"
                element={
                  <AdminOnlyRoute>
                    <AdminsPage />
                  </AdminOnlyRoute>
                }
              />

              <Route
                path="/domains"
                element={
                  <AdminOnlyRoute>
                    <DomainsPage />
                  </AdminOnlyRoute>
                }
              />

              <Route path="*" element={<NotFoundPage />} />
            </Route>
          </Routes>
        </Suspense>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App