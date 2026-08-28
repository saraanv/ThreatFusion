import { Outlet, useNavigate } from 'react-router-dom'

function AppLayout() {
  const navigate = useNavigate()

  const userJson =
    localStorage.getItem('user')

  const user = userJson
    ? JSON.parse(userJson)
    : null

  function handleLogout() {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
    localStorage.removeItem('expiresAtUtc')

    navigate('/login')
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
            onClick={() =>
              navigate('/dashboard')
            }
          >
            Dashboard
          </button>

          <button
  onClick={() =>
    navigate('/indicators')
  }
>
  Threat Indicators
</button>

          <button
  onClick={() =>
    navigate('/graph')
  }
>
  Threat Graph
</button>

          <button
  onClick={() =>
    navigate('/watchlist')
  }
>
  Watchlist
</button>

          <button
  onClick={() =>
    navigate('/alerts')
  }
>
  Alerts
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
              <span>
                {user.firstName}{' '}
                {user.lastName}
              </span>
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