import {
  useEffect,
  useState,
} from 'react'
import {
  addToWatchlist,
} from '../services/watchlistService'
import {
  useNavigate,
  useParams,
} from 'react-router-dom'

import {
  getThreatIndicatorById,
} from '../services/threatIndicatorService'

import type {
  ThreatIndicator,
} from '../types/threatIndicator'

function ThreatIndicatorDetailsPage() {
  const navigate =
    useNavigate()

  const { id } =
    useParams()

  const [indicator, setIndicator] =
    useState<ThreatIndicator | null>(null)

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')
const [addingToWatchlist, setAddingToWatchlist] =
  useState(false)

const [watchlistMessage, setWatchlistMessage] =
  useState('')

const [watchlistError, setWatchlistError] =
  useState('')
  useEffect(() => {
    async function loadIndicator() {
      try {
        setLoading(true)
        setError('')

        if (!id) {
          throw new Error(
            'Indicator id was not provided.'
          )
        }

        const indicatorId =
          Number(id)

        if (
          Number.isNaN(
            indicatorId
          )
        ) {
          throw new Error(
            'Indicator id is invalid.'
          )
        }

        const result =
          await getThreatIndicatorById(
            indicatorId
          )

        setIndicator(result)
      } catch (error) {
        console.error(
          'Indicator details error:',
          error
        )

        setError(
          'Could not load indicator details.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadIndicator()
  }, [id])

  if (loading) {
    return (
      <div className="indicator-details-page">
        Loading indicator details...
      </div>
    )
  }

  if (error) {
    return (
      <div className="indicator-details-page">
        {error}
      </div>
    )
  }

  if (!indicator) {
    return (
      <div className="indicator-details-page">
        Indicator not found.
      </div>
    )
  }async function handleAddToWatchlist() {
  if (!indicator) {
    return
  }

  try {
    setAddingToWatchlist(true)

    setWatchlistMessage('')
    setWatchlistError('')

    await addToWatchlist(
      indicator.id
    )

    setWatchlistMessage(
      'Indicator added to watchlist.'
    )
  } catch (error) {
    console.error(
      'Add to watchlist error:',
      error
    )

    setWatchlistError(
      'Could not add indicator to watchlist.'
    )
  } finally {
    setAddingToWatchlist(false)
  }
}

  return (
    <div className="indicator-details-page">

      <button
        className="back-button"
        onClick={() =>
          navigate('/indicators')
        }
      >
        ← Back to Indicators
      </button>

      <div className="details-heading">

        <div>
          <div className="details-type">
            {indicator.type}
          </div>

          <h1>
            {indicator.value}
          </h1>

          <p>
            Threat Indicator Details
          </p>
        </div>

        <div className="details-actions">

          <button
            className="secondary-action-button"
            onClick={() =>
              navigate(
                `/graph?indicatorId=${indicator.id}`
              )
            }
          >
            View Threat Graph
          </button>

          <button
  className="primary-action-button"
  onClick={handleAddToWatchlist}
  disabled={addingToWatchlist}
>{watchlistMessage && (
  <p className="watchlist-success">
    {watchlistMessage}
  </p>
)}

{watchlistError && (
  <p className="watchlist-error">
    {watchlistError}
  </p>
)}
  {
    addingToWatchlist
      ? 'Adding...'
      : 'Add to Watchlist'
  }
</button>

        </div>

      </div>

      <div className="details-stats">

        <div className="details-stat-card">
          <span>
            Severity
          </span>

          <strong>
            {
              indicator.severity
            }
          </strong>
        </div>

        <div className="details-stat-card">
          <span>
            Risk Score
          </span>

          <strong>
            {
              indicator.riskScore
            }
          </strong>
        </div>

        <div className="details-stat-card">
          <span>
            Risk Level
          </span>

          <strong>
            {
              indicator.riskLevel
            }
          </strong>
        </div>

        <div className="details-stat-card">
          <span>
            Confidence
          </span>

          <strong>
            {
              indicator.confidence
            }%
          </strong>
        </div>

        <div className="details-stat-card">
          <span>
            CVSS
          </span>

          <strong>
            {
              indicator.cvssScore
                ?? '-'
            }
          </strong>
        </div>

      </div>

      <div className="details-grid">

        <section className="details-panel">

          <h2>
            Threat Information
          </h2>

          <div className="details-row">
            <span>
              Source
            </span>

            <strong>
              {
                indicator.sourceName
              }
            </strong>
          </div>

          <div className="details-row">
            <span>
              CWE
            </span>

            <strong>
              {
                indicator.cweId
                ?? '-'
              }
            </strong>
          </div>

          <div className="details-row">
            <span>
              CVSS Version
            </span>

            <strong>
              {
                indicator.cvssVersion
                ?? '-'
              }
            </strong>
          </div>

          <div className="details-row">
            <span>
              Active
            </span>

            <strong>
              {
                indicator.isActive
                  ? 'Yes'
                  : 'No'
              }
            </strong>
          </div>

        </section>

        <section className="details-panel">

          <h2>
            Observation
          </h2>

          <div className="details-row">
            <span>
              First Seen
            </span>

            <strong>
              {
                indicator.firstSeenUtc
                  ? new Date(
                      indicator.firstSeenUtc
                    ).toLocaleString()
                  : '-'
              }
            </strong>
          </div>

          <div className="details-row">
            <span>
              Last Seen
            </span>

            <strong>
              {
                indicator.lastSeenUtc
                  ? new Date(
                      indicator.lastSeenUtc
                    ).toLocaleString()
                  : '-'
              }
            </strong>
          </div>

        </section>

      </div>

      <section className="details-panel details-description">

        <h2>
          Description
        </h2>

        <p>
          {
            indicator.description
              ?? 'No description available.'
          }
        </p>

      </section>

      {
        indicator.cvssVector && (
          <section className="details-panel">

            <h2>
              CVSS Vector
            </h2>

            <code className="cvss-vector">
              {
                indicator.cvssVector
              }
            </code>

          </section>
        )
      }

      {
        indicator.referenceUrl && (
          <section className="details-panel">

            <h2>
              Reference
            </h2>

            <a
              href={
                indicator.referenceUrl
              }
              target="_blank"
              rel="noreferrer"
              className="reference-link"
            >
              {
                indicator.referenceUrl
              }
            </a>

          </section>
        )
      }

    </div>
  )
}

export default ThreatIndicatorDetailsPage