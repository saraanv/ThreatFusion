import {
  useEffect,
  useState,
} from 'react'

import {
  useNavigate,
} from 'react-router-dom'

import {
  getMyAlerts,
  getUnreadAlertCount,
  markAlertAsRead,
} from '../services/alertService'

import type {
  ThreatAlert,
} from '../types/alert'

function AlertsPage() {
  const navigate = useNavigate()

  const [alerts, setAlerts] =
    useState<ThreatAlert[]>([])

  const [unreadCount, setUnreadCount] =
    useState(0)

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

  const [
    markingAlertId,
    setMarkingAlertId,
  ] = useState<number | null>(null)

  useEffect(() => {
    async function loadAlerts() {
      try {
        setLoading(true)
        setError('')

        const [
          alertsResult,
          unreadResult,
        ] = await Promise.all([
          getMyAlerts(),
          getUnreadAlertCount(),
        ])

        setAlerts(alertsResult)
        setUnreadCount(unreadResult)
      } catch (error) {
        console.error(
          'Alerts error:',
          error
        )

        setError(
          'Could not load alerts.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadAlerts()
  }, [])

  async function handleMarkAsRead(
    alertId: number
  ) {
    try {
      setMarkingAlertId(alertId)

      await markAlertAsRead(alertId)

      setAlerts(currentAlerts =>
        currentAlerts.map(alert =>
          alert.id === alertId
            ? {
                ...alert,
                isRead: true,
                readAtUtc:
                  new Date().toISOString(),
              }
            : alert
        )
      )

      setUnreadCount(current =>
        Math.max(0, current - 1)
      )
    } catch (error) {
      console.error(
        'Mark alert as read error:',
        error
      )

      setError(
        'Could not mark alert as read.'
      )
    } finally {
      setMarkingAlertId(null)
    }
  }

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
      <div className="alerts-page">
        <p>Loading alerts...</p>
      </div>
    )
  }

  if (error && alerts.length === 0) {
    return (
      <div className="alerts-page">
        <p>{error}</p>
      </div>
    )
  }

  return (
    <div className="alerts-page">

      <div className="page-heading">

        <div>
          <h1>
            Security Alerts
          </h1>

          <p>
            Review important threat
            intelligence events.
          </p>
        </div>

        <div className="alert-count-badge">
          {unreadCount} unread
        </div>

      </div>

      {error && (
        <p className="watchlist-error">
          {error}
        </p>
      )}

      {alerts.length === 0 ? (

        <div className="alerts-empty">
          <h2>No alerts</h2>

          <p>
            You currently have no
            security alerts.
          </p>
        </div>

      ) : (

        <div className="alerts-list">

          {alerts.map(alert => (

            <div
              key={alert.id}
              className={
                `alert-card ${
                  !alert.isRead
                    ? 'alert-unread'
                    : ''
                }`
              }
            >

              <div className="alert-card-header">

                <div>
                  <div className="alert-title-row">

                    {!alert.isRead && (
                      <span className="unread-dot" />
                    )}

                    <h3>
                      {alert.title}
                    </h3>

                  </div>

                  <span className="alert-type">
                    {alert.alertType}
                  </span>
                </div>

                <span
                  className={
                    `severity-badge ${getSeverityClass(
                      alert.severity
                    )}`
                  }
                >
                  {alert.severity}
                </span>

              </div>

              <p className="alert-message">
                {alert.message}
              </p>

              <div className="alert-indicator">
                Indicator:
                {' '}
                <strong>
                  {alert.indicatorValue}
                </strong>
              </div>

              <div className="alert-footer">

                <span className="alert-date">
                  {new Date(
                    alert.createdAtUtc
                  ).toLocaleString()}
                </span>

                <div className="alert-actions">

                  <button
                    className="secondary-action-button"
                    onClick={() =>
                      navigate(
                        `/indicators/${alert.threatIndicatorId}`
                      )
                    }
                  >
                    View Indicator
                  </button>

                  {!alert.isRead && (
                    <button
                      className="primary-action-button"
                      disabled={
                        markingAlertId ===
                        alert.id
                      }
                      onClick={() =>
                        handleMarkAsRead(
                          alert.id
                        )
                      }
                    >
                      {
                        markingAlertId ===
                        alert.id
                          ? 'Updating...'
                          : 'Mark as Read'
                      }
                    </button>
                  )}

                  {alert.isRead && (
                    <span className="read-label">
                      Read
                    </span>
                  )}

                </div>

              </div>

            </div>

          ))}

        </div>
      )}

    </div>
  )
}

export default AlertsPage