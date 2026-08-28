import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

interface GraphSearchIndicator {
  id: number
  type: string
  value: string
  severity: string
  riskScore: number
  riskLevel: string
  sourceName: string
}

interface SearchResponse {
  items: GraphSearchIndicator[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}

const API_BASE_URL = 'http://localhost:5152'

function ThreatGraphSelector() {
  const navigate = useNavigate()

  const [searchTerm, setSearchTerm] =
    useState('')

  const [results, setResults] =
    useState<GraphSearchIndicator[]>([])

  const [loading, setLoading] =
    useState(false)

  const [error, setError] =
    useState('')

  const [hasSearched, setHasSearched] =
    useState(false)

  async function handleSearch() {
    if (!searchTerm.trim()) {
      setError(
        'Enter an indicator value to search.'
      )

      return
    }

    const token =
      localStorage.getItem(
        'accessToken'
      )

    if (!token) {
      setError(
        'Access token not found.'
      )

      return
    }

    try {
      setLoading(true)
      setError('')
      setHasSearched(true)

      const params =
        new URLSearchParams()

      params.append(
        'SearchTerm',
        searchTerm.trim()
      )

      params.append(
        'PageNumber',
        '1'
      )

      params.append(
        'PageSize',
        '10'
      )

      const response =
        await fetch(
          `${API_BASE_URL}/threat/api/threat-indicators/GetThreatIndicators?${params.toString()}`,
          {
            method: 'GET',

            headers: {
              Authorization:
                `Bearer ${token}`,
            },
          }
        )

      if (!response.ok) {
        const errorText =
          await response.text()

        throw new Error(
          `Search failed. Status: ${response.status}. ${errorText}`
        )
      }

      const data: SearchResponse =
        await response.json()

      setResults(
        data.items ?? []
      )
    } catch (error) {
      console.error(
        'Graph indicator search error:',
        error
      )

      setError(
        'Could not search threat indicators.'
      )

      setResults([])
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="graph-selector">

      <div className="graph-selector-header">
        <h1>
          Threat Graph
        </h1>

        <p>
          Search for a threat
          indicator to visualize its
          relationships.
        </p>
      </div>

      <div className="graph-search-box">

        <input
          type="text"
          placeholder="Search IP, domain, URL, CVE..."
          value={searchTerm}
          onChange={e =>
            setSearchTerm(
              e.target.value
            )
          }
          onKeyDown={e => {
            if (
              e.key === 'Enter'
            ) {
              handleSearch()
            }
          }}
        />

        <button
          type="button"
          onClick={handleSearch}
          disabled={loading}
        >
          {loading
            ? 'Searching...'
            : 'Search'}
        </button>

      </div>

      {error && (
        <div className="graph-search-error">
          {error}
        </div>
      )}

      {!loading &&
        hasSearched &&
        results.length === 0 &&
        !error && (
          <div className="graph-search-empty">

            <h3>
              No indicators found
            </h3>

            <p>
              Try another value,
              domain, IP address or
              CVE identifier.
            </p>

          </div>
        )}

      {results.length > 0 && (
        <div className="graph-search-results">

          {results.map(
            indicator => (
              <button
                type="button"
                key={indicator.id}
                className="graph-search-result"
                onClick={() =>
                  navigate(
                    `/graph?indicatorId=${indicator.id}`
                  )
                }
              >

                <div className="graph-search-result-main">

                  <span className="graph-search-type">
                    {
                      indicator.type
                    }
                  </span>

                  <strong>
                    {
                      indicator.value
                    }
                  </strong>

                  <small>
                    {
                      indicator.sourceName
                    }
                  </small>

                </div>

                <div className="graph-search-result-info">

                  <span>
                    Severity:{' '}
                    <strong>
                      {
                        indicator.severity
                      }
                    </strong>
                  </span>

                  <span>
                    Risk:{' '}
                    <strong>
                      {
                        indicator.riskScore
                      }
                    </strong>
                  </span>

                  <span>
                    {
                      indicator.riskLevel
                    }
                  </span>

                </div>

              </button>
            )
          )}

        </div>
      )}

    </div>
  )
}

export default ThreatGraphSelector