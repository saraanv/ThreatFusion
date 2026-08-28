import {
  useEffect,
  useState,
} from 'react'

import {
  useNavigate,
} from 'react-router-dom'

import {
  getMyWatchlist,
  removeFromWatchlist,
} from '../services/watchlistService'

import type {
  WatchlistItem,
} from '../types/watchlist'

function WatchlistPage() {
  const navigate =
    useNavigate()

  const [items, setItems] =
    useState<WatchlistItem[]>([])

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

const [
  removingIndicatorId,
  setRemovingIndicatorId,
] = useState<number | null>(null)

  useEffect(() => {
    async function loadWatchlist() {
      try {
        setLoading(true)
        setError('')

        const result =
          await getMyWatchlist()

        setItems(result)
      } catch (error) {
        console.error(
          'Watchlist error:',
          error
        )

        setError(
          'Could not load watchlist.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadWatchlist()
  }, [])

  function getSeverityClass(
    severity: string
  ) {
    switch (
      severity.toLowerCase()
    ) {
      case 'critical':
        return 'severity-critical'

      case 'high':
        return 'severity-high'

      case 'medium':
        return 'severity-medium'

      case 'low':
        return 'severity-low'

      default:
        return 'severity-unknown'
    }
  }

  if (loading) {
    return (
      <div className="watchlist-page">
        <p>
          Loading watchlist...
        </p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="watchlist-page">
        <p className="error-message">
          {error}
        </p>
      </div>
    )
  }

async function handleRemove(
  indicatorId: number
) {
  try {
    setRemovingIndicatorId(
      indicatorId
    )

    await removeFromWatchlist(
      indicatorId
    )

    setItems(currentItems =>
      currentItems.filter(
        item =>
          item.indicatorId !==
          indicatorId
      )
    )
  } catch (error) {
    console.error(
      'Remove from watchlist error:',
      error
    )

    setError(
      'Could not remove indicator from watchlist.'
    )
  } finally {
    setRemovingIndicatorId(null)
  }
}

  return (
    <div className="watchlist-page">

      <div className="page-heading">

        <div>
          <h1>
            My Watchlist
          </h1>

          <p>
            Indicators you are currently monitoring.
          </p>
        </div>

        <div className="indicator-count">
          {items.length}
          {' '}
          watched
        </div>

      </div>

      {items.length === 0 ? (

        <div className="watchlist-empty">

          <h2>
            Your watchlist is empty
          </h2>

          <p>
            Add threat indicators to start monitoring them.
          </p>

          <button
            className="primary-action-button"
            onClick={() =>
              navigate('/indicators')
            }
          >
            Browse Indicators
          </button>

        </div>

      ) : (

        <div className="watchlist-grid">

          {items.map(
            (item) => (

              <div
                key={item.watchlistId}
                className="watchlist-card"
              >

                <div className="watchlist-card-header">

                  <div>
                    <span className="watchlist-type">
                      {item.type}
                    </span>

                    <h3>
                      {item.value}
                    </h3>
                  </div>

                  <span
                    className={
                      `severity-badge ${getSeverityClass(
                        item.severity
                      )}`
                    }
                  >
                    {item.severity}
                  </span>

                </div>

                <div className="watchlist-info">

                  <div>
                    <span>
                      Risk Score
                    </span>

                    <strong>
                      {item.riskScore}
                    </strong>
                  </div>

                  <div>
                    <span>
                      Risk Level
                    </span>

                    <strong>
                      {item.riskLevel}
                    </strong>
                  </div>

                  <div>
                    <span>
                      Source
                    </span>

                    <strong>
                      {item.sourceName}
                    </strong>
                  </div>

                </div>

                {item.note && (
                  <div className="watchlist-note">
                    <span>
                      Note
                    </span>

                    <p>
                      {item.note}
                    </p>
                  </div>
                )}

                <div className="watchlist-added">
                  Added:
                  {' '}
                  {new Date(
                    item.addedAtUtc
                  ).toLocaleString()}
                </div>

                <div className="watchlist-actions">

  <button
    className="remove-watchlist-button"
    disabled={
      removingIndicatorId ===
      item.indicatorId
    }
    onClick={() =>
      handleRemove(
        item.indicatorId
      )
    }
  >
    {
      removingIndicatorId ===
      item.indicatorId
        ? 'Removing...'
        : 'Remove'
    }
  </button>

  <button
    className="secondary-action-button"
    onClick={() =>
      navigate(
        `/indicators/${item.indicatorId}`
      )
    }
  >
    View Details
  </button>

</div>

              </div>
            )
          )}

        </div>
      )}

    </div>
  )
}

export default WatchlistPage