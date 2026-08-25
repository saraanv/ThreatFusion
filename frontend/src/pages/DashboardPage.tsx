import { useEffect, useState } from 'react'
import { getDashboardOverview } from '../services/dashboardService'
import type { DashboardOverview } from '../types/dashboard'

function DashboardPage() {
  const [dashboard, setDashboard] =
    useState<DashboardOverview | null>(null)

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

  useEffect(() => {
    async function loadDashboard() {
      try {
        setLoading(true)
        setError('')

        const result =
          await getDashboardOverview()

        setDashboard(result)
      } catch (error) {
        console.error(
          'Dashboard error:',
          error
        )

        setError(
          'Could not load dashboard.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadDashboard()
  }, [])

  if (loading) {
    return <p>Loading dashboard...</p>
  }

  if (error) {
    return <p>{error}</p>
  }

  if (!dashboard) {
    return <p>No dashboard data found.</p>
  }

  return (
    <div className="dashboard-page">
      <h1>ThreatFusion Dashboard</h1>

      <p>Threat Intelligence Overview</p>

      <div className="dashboard-cards">
        <div className="dashboard-card">
          <h3>Total Indicators</h3>
          <strong>
            {dashboard.totalIndicators}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>Critical</h3>
          <strong>
            {dashboard.criticalIndicators}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>High Risk</h3>
          <strong>
            {dashboard.highRiskIndicators}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>Watchlist</h3>
          <strong>
            {dashboard.watchedIndicators}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>Unread Alerts</h3>
          <strong>
            {dashboard.unreadAlerts}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>Automatic Relations</h3>
          <strong>
            {dashboard.automaticRelations}
          </strong>
        </div>

        <div className="dashboard-card">
          <h3>Manual Relations</h3>
          <strong>
            {dashboard.manualRelations}
          </strong>
        </div>
      </div>

      <h2>Top Risky Indicators</h2>

      <div className="dashboard-table">
        <table>
          <thead>
            <tr>
              <th>Value</th>
              <th>Type</th>
              <th>Severity</th>
              <th>Risk Score</th>
              <th>Source</th>
            </tr>
          </thead>

          <tbody>
            {dashboard.topRiskyIndicators.map(
              (indicator) => (
                <tr key={indicator.id}>
                  <td>{indicator.value}</td>
                  <td>{indicator.type}</td>
                  <td>{indicator.severity}</td>
                  <td>{indicator.riskScore}</td>
                  <td>{indicator.sourceName}</td>
                </tr>
              )
            )}
          </tbody>
        </table>
      </div>

      <h2>Recent Alerts</h2>

      {dashboard.recentAlerts.map(
        (alert) => (
          <div
            key={alert.id}
            className="alert-item"
          >
            <strong>
              {alert.title}
            </strong>

            <span>
              {alert.indicatorValue}
            </span>

            <span>
              {alert.severity}
            </span>
          </div>
        )
      )}
    </div>
  )
}

export default DashboardPage