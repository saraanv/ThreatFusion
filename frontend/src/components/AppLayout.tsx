import {
  useEffect,
  useState,
} from 'react'

import {
  Outlet,
  useLocation,
  useNavigate,
} from 'react-router-dom'

import {
  getCurrentUserRoles,
} from '../utils/auth'

import {
  getUnreadAlertCount,
} from '../services/alertService'

function AppLayout() {
  const navigate = useNavigate()
  const location = useLocation()

  const [unreadAlertCount, setUnreadAlertCount] =
    useState(0)

  const userJson =
    localStorage.getItem('user')

  const user = userJson
    ? JSON.parse(userJson)
    : null

  const roles =
    getCurrentUserRoles()

  useEffect(() => {
    async function loadUnreadAlertCount() {
      try {
        const count =
          await getUnreadAlertCount()

        setUnreadAlertCount(count)
      } catch (error) {
        console.error(
          'Failed to load unread alert count:',
          error
        )
      }
    }

    loadUnreadAlertCount()
  }, [location.pathname])

  function handleLogout() {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
    localStorage.removeItem('expiresAtUtc')

    navigate('/login')
  }

  function isActive(path: string) {
    return location.pathname.startsWith(path)
  }

  return (
    <div className="app-layout">

      <aside className="sidebar">

        <div className="sidebar-logo">
          <h2>ThreatFusion</h2>
          <span>Threat Intelligence</span>
        </div>

        <nav className="sidebar-nav">

          <button
            className={
              isActive('/dashboard')
                ? 'active'
                : ''
            }
            onClick={() =>
              navigate('/dashboard')
            }
          >
            Dashboard
          </button>

          <button
            className={
              isActive('/indicators')
                ? 'active'
                : ''
            }
            onClick={() =>
              navigate('/indicators')
            }
          >
            Threat Indicators
          </button>

          <button
            className={
              isActive('/graph')
                ? 'active'
                : ''
            }
            onClick={() =>
              navigate('/graph')
            }
          >
            Threat Graph
          </button>

          <button
            className={
              isActive('/watchlist')
                ? 'active'
                : ''
            }
            onClick={() =>
              navigate('/watchlist')
            }
          >
            Watchlist
          </button>

          <button
            className={
              isActive('/alerts')
                ? 'active'
                : ''
            }
            onClick={() =>
              navigate('/alerts')
            }
          >
            <span>Alerts</span>

            {unreadAlertCount > 0 && (
              <span className="sidebar-alert-badge">
                {unreadAlertCount > 99
                  ? '99+'
                  : unreadAlertCount}
              </span>
            )}
          </button>

        </nav>

      </aside>

      <div className="main-area">

        <header className="top-header">

          <div>
            <strong>
              Threat Intelligence Platform
            </strong>
          </div>

          <div className="header-user">

            {user && (
              <div className="header-user-info">

                <span>
                  {user.firstName}{' '}
                  {user.lastName}
                </span>

                {roles.map(role => (
                  <span
                    key={role}
                    className="role-badge"
                  >
                    {role}
                  </span>
                ))}

              </div>
            )}

            <button
              onClick={handleLogout}
            >
              Logout
            </button>

          </div>

        </header>

        <main className="page-content">
          <Outlet />
        </main>

      </div>

    </div>
  )
}

export default AppLayout