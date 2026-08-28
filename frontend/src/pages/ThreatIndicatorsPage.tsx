import {
  useEffect,
  useState,
} from 'react'
import {
  useNavigate,
} from 'react-router-dom'
import {
  getThreatIndicators,
} from '../services/threatIndicatorService'
import {
  addToWatchlist,
} from '../services/watchlistService'
import type {
  ThreatIndicator,
} from '../types/threatIndicator'
import {
  hasAnyRole,
} from '../utils/auth'

function ThreatIndicatorsPage() {
  const [indicators, setIndicators] =
    useState<ThreatIndicator[]>([])

  const [pageNumber, setPageNumber] =
    useState(1)

  const [totalPages, setTotalPages] =
    useState(1)

  const [totalCount, setTotalCount] =
    useState(0)

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

  /*
   * چیزی که کاربر داخل فرم تایپ می‌کند
   */
  const [searchTerm, setSearchTerm] =
    useState('')

  const [type, setType] =
    useState('')

  const [severity, setSeverity] =
    useState('')

  const [riskLevel, setRiskLevel] =
    useState('')

  /*
   * فیلترهایی که واقعاً
   * روی API اعمال شده‌اند.
   */
  const [appliedSearchTerm, setAppliedSearchTerm] =
    useState('')

  const [appliedType, setAppliedType] =
    useState('')

  const [appliedSeverity, setAppliedSeverity] =
    useState('')

  const [appliedRiskLevel, setAppliedRiskLevel] =
    useState('')

    const navigate =
  useNavigate()
const canCreateIndicator =
  hasAnyRole([
    'Analyst',
    'Admin',
  ])

const [addingToWatchlist, setAddingToWatchlist] =
  useState(false)


const [watchlistMessage, setWatchlistMessage] =
  useState('')

const [watchlistError, setWatchlistError] =
  useState('')
  useEffect(() => {
    async function loadIndicators() {
      try {
        setLoading(true)
        setError('')

        const result =
          await getThreatIndicators(
            pageNumber,
            20,
            {
              searchTerm:
                appliedSearchTerm,

              type:
                appliedType,

              severity:
                appliedSeverity,

              riskLevel:
                appliedRiskLevel,
            }
          )

        setIndicators(
          result.items
        )

        setTotalPages(
          result.totalPages
        )

        setTotalCount(
          result.totalCount
        )
      } catch (error) {
        console.error(
          'Threat indicators error:',
          error
        )

        setError(
          'Could not load threat indicators.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadIndicators()
  }, [
    pageNumber,
    appliedSearchTerm,
    appliedType,
    appliedSeverity,
    appliedRiskLevel,
  ])

  function handleSearch(
    event:
      React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    /*
     * با اعمال filter
     * همیشه برگردیم صفحه اول.
     */
    setPageNumber(1)

    setAppliedSearchTerm(
      searchTerm
    )

    setAppliedType(
      type
    )

    setAppliedSeverity(
      severity
    )

    setAppliedRiskLevel(
      riskLevel
    )
  }

  function handleClearFilters() {
    setSearchTerm('')
    setType('')
    setSeverity('')
    setRiskLevel('')

    setAppliedSearchTerm('')
    setAppliedType('')
    setAppliedSeverity('')
    setAppliedRiskLevel('')

    setPageNumber(1)
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

  return (
    <div className="indicators-page">

      <div className="page-heading">

        <div>
          <h1>
            Threat Indicators
          </h1>

          <p>
            Browse and analyze collected
            threat intelligence.
          </p>
        </div>

        <div className="page-heading-actions">

  <div className="indicator-count">
    {totalCount}
    {' '}
    indicators
  </div>

  {canCreateIndicator && (
    <button
      type="button"
      className="create-indicator-button"
      onClick={() =>
        navigate('/indicators/create')
      }
    >
      + Create Indicator
    </button>
  )}

</div>

      </div>

      {/* ============================= */}
      {/* FILTERS */}
      {/* ============================= */}

      <form
        className="indicator-filters"
        onSubmit={handleSearch}
      >

        <input
          className="filter-search"
          type="text"
          placeholder="Search CVE, domain, IP..."
          value={searchTerm}
          onChange={(event) =>
            setSearchTerm(
              event.target.value
            )
          }
        />

        <select
          value={type}
          onChange={(event) =>
            setType(
              event.target.value
            )
          }
        >
          <option value="">
            All Types
          </option>

          <option value="Cve">
            CVE
          </option>

          <option value="Domain">
            Domain
          </option>

          <option value="IpAddress">
            IP Address
          </option>

          <option value="Url">
            URL
          </option>

          <option value="FileHash">
            File Hash
          </option>
        </select>

        <select
          value={severity}
          onChange={(event) =>
            setSeverity(
              event.target.value
            )
          }
        >
          <option value="">
            All Severities
          </option>

          <option value="Critical">
            Critical
          </option>

          <option value="High">
            High
          </option>

          <option value="Medium">
            Medium
          </option>

          <option value="Low">
            Low
          </option>

          <option value="Unknown">
            Unknown
          </option>
        </select>

        <select
          value={riskLevel}
          onChange={(event) =>
            setRiskLevel(
              event.target.value
            )
          }
        >
          <option value="">
            All Risk Levels
          </option>

          <option value="Critical">
            Critical
          </option>

          <option value="High">
            High
          </option>

          <option value="Medium">
            Medium
          </option>

          <option value="Low">
            Low
          </option>
        </select>

        <button
          type="submit"
          className="filter-button"
        >
          Search
        </button>

        <button
          type="button"
          className="clear-filter-button"
          onClick={
            handleClearFilters
          }
        >
          Clear
        </button>

      </form>

      {/* ============================= */}
      {/* LOADING / ERROR */}
      {/* ============================= */}

      {loading && (
        <div className="table-message">
          Loading threat indicators...
        </div>
      )}

      {error && (
        <div className="table-message error-message">
          {error}
        </div>
      )}

      {/* ============================= */}
      {/* TABLE */}
      {/* ============================= */}

      {!loading &&
       !error &&
       indicators.length === 0 && (
        <div className="table-message">
          No threat indicators found.
        </div>
      )}

      {!loading &&
       !error &&
       indicators.length > 0 && (
        <>
          <div className="indicators-table-wrapper">

            <table className="indicators-table">

              <thead>
                <tr>
                  <th>Indicator</th>
                  <th>Type</th>
                  <th>Severity</th>
                  <th>Confidence</th>
                  <th>Risk Score</th>
                  <th>Risk Level</th>
                  <th>Source</th>
                  <th>CVSS</th>
                </tr>
              </thead>

              <tbody>

                {indicators.map(
                  (indicator) => (

                    <tr
  key={indicator.id}
  className="clickable-row"
  onClick={() =>
    navigate(
      `/indicators/${indicator.id}`
    )
  }
>

                      <td className="indicator-value">
                        {
                          indicator.value
                        }
                      </td>

                      <td>
                        {
                          indicator.type
                        }
                      </td>

                      <td>
                        <span
                          className={
                            `severity-badge ${getSeverityClass(
                              indicator.severity
                            )}`
                          }
                        >
                          {
                            indicator.severity
                          }
                        </span>
                      </td>

                      <td>
                        {
                          indicator.confidence
                        }%
                      </td>

                      <td>
                        {
                          indicator.riskScore
                        }
                      </td>

                      <td>
                        {
                          indicator.riskLevel
                        }
                      </td>

                      <td>
                        {
                          indicator.sourceName
                        }
                      </td>

                      <td>
                        {
                          indicator.cvssScore
                            ?? '-'
                        }
                      </td>

                    </tr>
                  )
                )}

              </tbody>

            </table>

          </div>

          {/* ============================= */}
          {/* PAGINATION */}
          {/* ============================= */}

          <div className="pagination">

            <button
              disabled={
                pageNumber === 1
              }
              onClick={() =>
                setPageNumber(
                  current =>
                    current - 1
                )
              }
            >
              Previous
            </button>

            <span>
              Page
              {' '}
              {pageNumber}
              {' '}
              of
              {' '}
              {totalPages}
            </span>

            <button
              disabled={
                pageNumber >=
                totalPages
              }
              onClick={() =>
                setPageNumber(
                  current =>
                    current + 1
                )
              }
            >
              Next
            </button>

          </div>
        </>
      )}

    </div>
  )
}

export default ThreatIndicatorsPage