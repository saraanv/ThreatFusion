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



        <Route
          element={
            <ProtectedRoute>
              <AppLayout />
            </ProtectedRoute>
          }
        >



          <Route
            path="/dashboard"
            element={
              <DashboardPage />
            }
          />



          <Route
            path="/indicators"
            element={
              <ThreatIndicatorsPage />
            }
          />



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


          <Route
            path="/graph"
            element={
              <ThreatGraphPage />
            }
          />



          <Route
            path="/watchlist"
            element={
              <WatchlistPage />
            }
          />



          <Route
            path="/alerts"
            element={
              <AlertsPage />
            }
          />



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



        <Route
          path="/"
          element={
            <Navigate
              to="/dashboard"
              replace
            />
          }
        />



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