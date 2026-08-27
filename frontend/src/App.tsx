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

      </Routes>

    </BrowserRouter>
  )
}

export default App