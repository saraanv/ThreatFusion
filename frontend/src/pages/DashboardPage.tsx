import {
  useEffect,
  useState,
} from 'react'

import {
  useNavigate,
} from 'react-router-dom'

import {
  getDashboardOverview,
  getThreatDashboard,
} from '../services/dashboardService'

import type {
  DashboardOverview,
  ThreatDashboard,
} from '../types/dashboard'

function DashboardPage() {
  const navigate = useNavigate()

  const [
    overview,
    setOverview,
  ] = useState<DashboardOverview | null>(
    null
  )

  const [
    threatDashboard,
    setThreatDashboard,
  ] = useState<ThreatDashboard | null>(
    null
  )

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

  useEffect(() => {
    async function loadDashboard() {
      try {
        setLoading(true)
        setError('')

        const [
          overviewResult,
          dashboardResult,
        ] = await Promise.all([
          getDashboardOverview(),
          getThreatDashboard(),
        ])

        setOverview(overviewResult)
        setThreatDashboard(
          dashboardResult
        )
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

  function formatDate(
    value: string | null | undefined
  ) {
    if (!value) {
      return '-'
    }

    return new Date(
      value
    ).toLocaleString()
  }

  if (loading) {
    return (
      <div className="dashboard-status">
        Loading dashboard...
      </div>
    )
  }

  if (error) {
    return (
      <div className="dashboard-error">
        {error}
      </div>
    )
  }

  if (
    !overview ||
    !threatDashboard
  ) {
    return (
      <div className="dashboard-status">
        No dashboard data found.
      </div>
    )
  }

  return (
    <div className="dashboard-page">


      <div className="dashboard-header">

        <div>
          <h1>
            ThreatFusion Dashboard
          </h1>

          <p>
            Threat Intelligence Overview
          </p>
        </div>

        <button
          type="button"
          className="dashboard-view-button"
          onClick={() =>
            navigate('/indicators')
          }
        >
          View Indicators
        </button>

      </div>



      <section className="dashboard-section">

        <div className="dashboard-cards">

          <div className="dashboard-card">
            <span>
              Total Indicators
            </span>

            <strong>
              {
                threatDashboard
                  .totalIndicators
              }
            </strong>
          </div>

          <div className="dashboard-card">
            <span>
              Active Indicators
            </span>

            <strong>
              {
                threatDashboard
                  .activeIndicators
              }
            </strong>
          </div>

          <div className="dashboard-card">
            <span>
              Critical Risk
            </span>

            <strong>
              {
                overview
                  .criticalIndicators
              }
            </strong>

            <small>
              Risk level = Critical
            </small>
          </div>

          <div className="dashboard-card">
            <span>
              Critical Severity
            </span>

            <strong>
              {
                threatDashboard
                  .criticalIndicators
              }
            </strong>

            <small>
              Severity = Critical
            </small>
          </div>

          <div className="dashboard-card">
            <span>
              High Risk
            </span>

            <strong>
              {
                overview
                  .highRiskIndicators
              }
            </strong>
          </div>

          <div className="dashboard-card">
            <span>
              Watchlist
            </span>

            <strong>
              {
                overview
                  .watchedIndicators
              }
            </strong>
          </div>

          <div className="dashboard-card">
            <span>
              Unread Alerts
            </span>

            <strong>
              {
                overview
                  .unreadAlerts
              }
            </strong>
          </div>

          <div className="dashboard-card">
            <span>
              Relations
            </span>

            <strong>
              {
                overview
                  .automaticRelations +
                overview
                  .manualRelations
              }
            </strong>

            <small>
              {
                overview
                  .automaticRelations
              }{' '}
              automatic ·{' '}
              {
                overview
                  .manualRelations
              }{' '}
              manual
            </small>
          </div>

        </div>

      </section>



      <section className="dashboard-section">

        <div className="dashboard-two-columns">



          <div className="dashboard-panel">

            <div className="dashboard-panel-header">

              <div>
                <h2>
                  Indicators by Type
                </h2>

                <p>
                  Distribution of threat
                  indicators by type.
                </p>
              </div>

            </div>

            <div className="distribution-list">

              {
                threatDashboard
                  .indicatorsByType
                  .map(
                    (item, index) => (
                      <div
                        key={
                          `${item.type}-${index}`
                        }
                        className="distribution-item"
                      >

                        <span>
                          {item.type}
                        </span>

                        <strong>
                          {item.count}
                        </strong>

                      </div>
                    )
                  )
              }

            </div>

          </div>


          <div className="dashboard-panel">

            <div className="dashboard-panel-header">

              <div>
                <h2>
                  Indicators by Source
                </h2>

                <p>
                  Distribution of threat
                  indicators by source.
                </p>
              </div>

            </div>

            <div className="distribution-list">

              {
                threatDashboard
                  .indicatorsBySource
                  .map(
                    (item, index) => (
                      <div
                        key={
                          `${item.sourceName}-${index}`
                        }
                        className="distribution-item"
                      >

                        <span>
                          {
                            item.sourceName ||
                            'Unknown'
                          }
                        </span>

                        <strong>
                          {item.count}
                        </strong>

                      </div>
                    )
                  )
              }

            </div>

          </div>

        </div>

      </section>



      <section className="dashboard-section">

        <div className="dashboard-panel">

          <div className="dashboard-panel-header">

            <div>
              <h2>
                Top Risky Indicators
              </h2>

              <p>
                Indicators with the
                highest calculated risk.
              </p>
            </div>

          </div>

          {
            overview
              .topRiskyIndicators
              .length === 0 ? (

              <div className="dashboard-empty">
                No risky indicators found.
              </div>

            ) : (

              <div className="dashboard-table-wrapper">

                <table className="dashboard-table">

                  <thead>
                    <tr>
                      <th>Value</th>
                      <th>Type</th>
                      <th>Severity</th>
                      <th>Risk Score</th>
                      <th>Risk Level</th>
                      <th>Source</th>
                    </tr>
                  </thead>

                  <tbody>

                    {
                      overview
                        .topRiskyIndicators
                        .map(
                          (
                            indicator,
                            index
                          ) => (
                            <tr
                              key={
                                `${indicator.id}-${index}`
                              }
                              onClick={() =>
                                navigate(
                                  `/indicators/${indicator.id}`
                                )
                              }
                            >

                              <td className="indicator-value-cell">
                                {
                                  indicator
                                    .value
                                }
                              </td>

                              <td>
                                {
                                  indicator
                                    .type
                                }
                              </td>

                              <td>
                                {
                                  indicator
                                    .severity
                                }
                              </td>

                              <td>
                                {
                                  indicator
                                    .riskScore
                                }
                              </td>

                              <td>
                                {
                                  indicator
                                    .riskLevel
                                }
                              </td>

                              <td>
                                {
                                  indicator
                                    .sourceName
                                }
                              </td>

                            </tr>
                          )
                        )
                    }

                  </tbody>

                </table>

              </div>
            )
          }

        </div>

      </section>



      <section className="dashboard-section">

        <div className="dashboard-panel">

          <div className="dashboard-panel-header">

            <div>
              <h2>
                Latest Threats
              </h2>

              <p>
                Most recently added
                threat indicators.
              </p>
            </div>

          </div>

          {
            threatDashboard
              .latestThreats
              .length === 0 ? (

              <div className="dashboard-empty">
                No recent threats found.
              </div>

            ) : (

              <div className="dashboard-table-wrapper">

                <table className="dashboard-table">

                  <thead>
                    <tr>
                      <th>Value</th>
                      <th>Type</th>
                      <th>Severity</th>
                      <th>Confidence</th>
                      <th>Source</th>
                      <th>Created</th>
                    </tr>
                  </thead>

                  <tbody>

                    {
                      threatDashboard
                        .latestThreats
                        .map(
                          (
                            threat,
                            index
                          ) => (
                            <tr
                              key={
                                `${threat.id}-${index}`
                              }
                              onClick={() =>
                                navigate(
                                  `/indicators/${threat.id}`
                                )
                              }
                            >

                              <td className="indicator-value-cell">
                                {
                                  threat.value
                                }
                              </td>

                              <td>
                                {
                                  threat.type
                                }
                              </td>

                              <td>
                                {
                                  threat
                                    .severity
                                }
                              </td>

                              <td>
                                {
                                  threat
                                    .confidence
                                }
                                %
                              </td>

                              <td>
                                {
                                  threat
                                    .sourceName
                                }
                              </td>

                              <td>
                                {
                                  formatDate(
                                    threat
                                      .createdAtUtc
                                  )
                                }
                              </td>

                            </tr>
                          )
                        )
                    }

                  </tbody>

                </table>

              </div>
            )
          }

        </div>

      </section>



      <section className="dashboard-section">

        <div className="dashboard-two-columns">



          <div className="dashboard-panel">

            <div className="dashboard-panel-header">

              <div>
                <h2>
                  Recent Alerts
                </h2>

                <p>
                  Latest watchlist
                  notifications.
                </p>
              </div>

              <button
                type="button"
                onClick={() =>
                  navigate('/alerts')
                }
              >
                View All
              </button>

            </div>

            <div className="dashboard-alert-list">

              {
                overview
                  .recentAlerts
                  .length === 0 ? (

                  <div className="dashboard-empty">
                    No recent alerts.
                  </div>

                ) : (

                  overview
                    .recentAlerts
                    .map(
                      (
                        alert,
                        index
                      ) => (
                        <div
                          key={
                            `${alert.id}-${index}`
                          }
                          className="dashboard-alert-item"
                        >

                          <div className="dashboard-alert-content">

                            <strong>
                              {
                                alert.title
                              }
                            </strong>

                            <span>
                              {
                                alert
                                  .indicatorValue
                              }
                            </span>

                          </div>

                          <div className="dashboard-alert-meta">

                            <span>
                              {
                                alert
                                  .severity
                              }
                            </span>

                            <small>
                              {
                                formatDate(
                                  alert
                                    .createdAtUtc
                                )
                              }
                            </small>

                          </div>

                        </div>
                      )
                    )
                )
              }

            </div>

          </div>



          <div className="dashboard-panel">

            <div className="dashboard-panel-header">

              <div>
                <h2>
                  Last Feed Sync
                </h2>

                <p>
                  Latest threat feed
                  synchronization.
                </p>
              </div>

            </div>

            {
              threatDashboard
                .lastFeedSync ? (

                <div className="feed-sync">

                  <div className="feed-sync-title">

                    <div>
                      <span>
                        Feed
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .feedName
                        }
                      </strong>
                    </div>

                    <span
                      className={
                        threatDashboard
                          .lastFeedSync
                          .isSuccessful
                          ? 'sync-success'
                          : 'sync-failed'
                      }
                    >
                      {
                        threatDashboard
                          .lastFeedSync
                          .isSuccessful
                          ? 'Successful'
                          : 'Failed'
                      }
                    </span>

                  </div>

                  <div className="feed-sync-grid">

                    <div>
                      <span>
                        Fetched
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .totalFetched
                        }
                      </strong>
                    </div>

                    <div>
                      <span>
                        Created
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .createdCount
                        }
                      </strong>
                    </div>

                    <div>
                      <span>
                        Updated
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .updatedCount
                        }
                      </strong>
                    </div>

                    <div>
                      <span>
                        Unchanged
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .unchangedCount
                        }
                      </strong>
                    </div>

                    <div>
                      <span>
                        Failed
                      </span>

                      <strong>
                        {
                          threatDashboard
                            .lastFeedSync
                            .failedCount
                        }
                      </strong>
                    </div>

                  </div>

                  <div className="feed-sync-dates">

                    <div className="feed-sync-date">

                      <span>
                        Started
                      </span>

                      <strong>
                        {
                          formatDate(
                            threatDashboard
                              .lastFeedSync
                              .startedAtUtc
                          )
                        }
                      </strong>

                    </div>

                    <div className="feed-sync-date">

                      <span>
                        Completed
                      </span>

                      <strong>
                        {
                          formatDate(
                            threatDashboard
                              .lastFeedSync
                              .completedAtUtc
                          )
                        }
                      </strong>

                    </div>

                  </div>

                </div>

              ) : (

                <div className="dashboard-empty">
                  No feed synchronization
                  data found.
                </div>

              )
            }

          </div>

        </div>

      </section>

    </div>
  )
}

export default DashboardPage