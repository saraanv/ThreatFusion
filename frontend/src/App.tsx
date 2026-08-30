import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from 'react-router-dom'
import WatchlistPage
  from './pages/WatchlistPage'
import ThreatIndicatorDetailsPage
  from './pages/ThreatIndicatorDetailsPage'
import ThreatGraphPage
  from './pages/ThreatGraphPage'
import LoginPage
  from './pages/LoginPage'

import DashboardPage
  from './pages/DashboardPage'

import ThreatIndicatorsPage
  from './pages/ThreatIndicatorsPage'

import ProtectedRoute
  from './components/ProtectedRoute'

import AppLayout
  from './components/AppLayout'

import AlertsPage
  from './pages/AlertsPage'
import RegisterPage
  from './pages/RegisterPage'

import CreateThreatIndicatorPage
  from './pages/CreateThreatIndicatorPage'

import RoleProtectedRoute
  from './components/RoleProtectedRoute'

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

        </Route>

<Route
  path="/watchlist"
  element={
    <WatchlistPage />
  }
/>

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
  path="/alerts"
  element={
    <AlertsPage />
  }
/>

<Route
  path="/graph"
  element={<ThreatGraphPage />}
/>

      </Routes>

    </BrowserRouter>
  )
}

export default App