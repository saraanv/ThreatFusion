import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from 'react-router-dom'

import LoginPage
  from './pages/LoginPage'

import RegisterPage
  from './pages/RegisterPage'

import DashboardPage
  from './pages/DashboardPage'

import ThreatIndicatorsPage
  from './pages/ThreatIndicatorsPage'

import ThreatIndicatorDetailsPage
  from './pages/ThreatIndicatorDetailsPage'

import CreateThreatIndicatorPage
  from './pages/CreateThreatIndicatorPage'

import CreateThreatRelationPage
  from './pages/CreateThreatRelationPage'

import ThreatGraphPage
  from './pages/ThreatGraphPage'

import WatchlistPage
  from './pages/WatchlistPage'

import AlertsPage
  from './pages/AlertsPage'

import AssignRolePage
  from './pages/AssignRolePage'

import ProtectedRoute
  from './components/ProtectedRoute'

import RoleProtectedRoute
  from './components/RoleProtectedRoute'

import AppLayout
  from './components/AppLayout'

function App() {
  return (
    <BrowserRouter>

      <Routes>

        {/* =========================
            PUBLIC ROUTES
            ========================= */}

        <Route
          path="/login"
          element={
            <LoginPage />
          }
        />

        <Route
          path="/register"
          element={
            <RegisterPage />
          }
        />

        {/* =========================
            AUTHENTICATED ROUTES
            All routes here use AppLayout
            and therefore show Sidebar.
            ========================= */}

        <Route
          element={
            <ProtectedRoute>
              <AppLayout />
            </ProtectedRoute>
          }
        >

          {/* Dashboard */}

          <Route
            path="/dashboard"
            element={
              <DashboardPage />
            }
          />

          {/* Threat Indicators */}

          <Route
            path="/indicators"
            element={
              <ThreatIndicatorsPage />
            }
          />

          {/* Create Threat Indicator */}

          <Route
            path="/indicators/create"
            element={
              <RoleProtectedRoute
                allowedRoles={[
                  'Analyst',
                  'Admin',
                ]}
              >
                <CreateThreatIndicatorPage />
              </RoleProtectedRoute>
            }
          />

          {/* Threat Indicator Details */}

          <Route
            path="/indicators/:id"
            element={
              <ThreatIndicatorDetailsPage />
            }
          />

          {/* Create Threat Relation */}

          <Route
            path="/indicators/:sourceIndicatorId/create-relation"
            element={
              <RoleProtectedRoute
                allowedRoles={[
                  'Analyst',
                  'Admin',
                ]}
              >
                <CreateThreatRelationPage />
              </RoleProtectedRoute>
            }
          />

          {/* Threat Graph */}

          <Route
            path="/graph"
            element={
              <ThreatGraphPage />
            }
          />

          {/* Watchlist */}

          <Route
            path="/watchlist"
            element={
              <WatchlistPage />
            }
          />

          {/* Alerts */}

          <Route
            path="/alerts"
            element={
              <AlertsPage />
            }
          />

          {/* Admin - Assign Role */}

          <Route
            path="/admin/assign-role"
            element={
              <RoleProtectedRoute
                allowedRoles={[
                  'Admin',
                ]}
              >
                <AssignRolePage />
              </RoleProtectedRoute>
            }
          />

        </Route>

        {/* =========================
            DEFAULT ROUTE
            ========================= */}

        <Route
          path="/"
          element={
            <Navigate
              to="/dashboard"
              replace
            />
          }
        />

        {/* =========================
            UNKNOWN ROUTES
            ========================= */}

        <Route
          path="*"
          element={
            <Navigate
              to="/dashboard"
              replace
            />
          }
        />

      </Routes>

    </BrowserRouter>
  )
}

export default App