import {
  useEffect,
  useState,
} from 'react'

import {
  useNavigate,
  useParams,
} from 'react-router-dom'

import {
  createThreatRelation,
} from '../services/threatRelationService'

import {
  getThreatIndicators,
} from '../services/threatIndicatorService'

import type {
  ThreatIndicator,
} from '../types/threatIndicator'

import type {
  ThreatRelationTypeValue,
} from '../types/threatRelation'

function CreateThreatRelationPage() {
  const navigate = useNavigate()

  const { sourceIndicatorId } =
    useParams()

  const sourceId =
    Number(sourceIndicatorId)

  const [searchTerm, setSearchTerm] =
    useState('')

  const [
    searchResults,
    setSearchResults,
  ] = useState<ThreatIndicator[]>([])

  const [
    selectedTarget,
    setSelectedTarget,
  ] = useState<ThreatIndicator | null>(null)

  const [
    searching,
    setSearching,
  ] = useState(false)

  const [relationType, setRelationType] =
    useState<ThreatRelationTypeValue>(1)

  const [confidence, setConfidence] =
    useState(80)

  const [description, setDescription] =
    useState('')

  const [loading, setLoading] =
    useState(false)

  const [error, setError] =
    useState('')

  const [success, setSuccess] =
    useState('')

  useEffect(() => {
    if (!searchTerm.trim()) {
      setSearchResults([])
      return
    }

    const timeoutId =
      window.setTimeout(
        async () => {
          try {
            setSearching(true)

            const result =
              await getThreatIndicators(
                1,
                10,
                {
                  searchTerm:
                    searchTerm.trim(),
                }
              )

            const targets =
              result.items.filter(
                item =>
                  item.id !== sourceId
              )

            setSearchResults(
              targets
            )
          } catch (error) {
            console.error(
              'Target search error:',
              error
            )

            setSearchResults([])
          } finally {
            setSearching(false)
          }
        },
        400
      )

    return () => {
      window.clearTimeout(
        timeoutId
      )
    }
  }, [
    searchTerm,
    sourceId,
  ])

  function handleSelectTarget(
    indicator: ThreatIndicator
  ) {
    setSelectedTarget(
      indicator
    )

    setSearchTerm('')
    setSearchResults([])
    setError('')
  }

  function handleRemoveTarget() {
    setSelectedTarget(null)
    setSearchTerm('')
    setSearchResults([])
  }

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    setError('')
    setSuccess('')

    if (
      !Number.isInteger(sourceId) ||
      sourceId <= 0
    ) {
      setError(
        'Source indicator ID is invalid.'
      )

      return
    }

    if (!selectedTarget) {
      setError(
        'Please select a target indicator.'
      )

      return
    }

    if (
      selectedTarget.id === sourceId
    ) {
      setError(
        'Source and target indicators cannot be the same.'
      )

      return
    }

    if (
      confidence < 0 ||
      confidence > 100
    ) {
      setError(
        'Confidence must be between 0 and 100.'
      )

      return
    }

    try {
      setLoading(true)

      const result =
        await createThreatRelation({
          sourceIndicatorId:
            sourceId,

          targetIndicatorId:
            selectedTarget.id,

          relationType,

          description:
            description.trim() || null,

          confidence,
        })

      console.log(
        'Create relation result:',
        result
      )

      setSuccess(
        result.message
      )

      window.setTimeout(
        () => {
          navigate(
            `/graph?indicatorId=${sourceId}`
          )
        },
        700
      )
    } catch (error) {
      console.error(
        'Create relation error:',
        error
      )

      if (error instanceof Error) {
        setError(error.message)
      } else {
        setError(
          'Failed to create threat relation.'
        )
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="create-relation-page">

      <div className="page-heading">

        <div>
          <h1>
            Create Threat Relation
          </h1>

          <p>
            Create a manual relationship
            between two threat indicators.
          </p>
        </div>

        <button
          type="button"
          className="secondary-button"
          onClick={() =>
            navigate(
              `/indicators/${sourceId}`
            )
          }
        >
          Back to Indicator
        </button>

      </div>

      <form
        className="relation-form"
        onSubmit={handleSubmit}
      >

        <div className="form-section">

          <h2>
            Relation Information
          </h2>

          <div className="form-grid">

            {/* SOURCE */}

            <div className="form-group">

              <label>
                Source Indicator ID
              </label>

              <input
                type="number"
                value={
                  Number.isNaN(sourceId)
                    ? ''
                    : sourceId
                }
                disabled
              />

            </div>

            {/* RELATION TYPE */}

            <div className="form-group">

              <label>
                Relation Type *
              </label>

              <select
                value={relationType}
                onChange={(event) =>
                  setRelationType(
                    Number(
                      event.target.value
                    ) as ThreatRelationTypeValue
                  )
                }
              >

                <option value={1}>
                  Related To
                </option>

                <option value={2}>
                  Resolves To
                </option>

                <option value={3}>
                  Hosts
                </option>

                <option value={4}>
                  Exploits
                </option>

                <option value={5}>
                  Downloads
                </option>

                <option value={6}>
                  Communicates With
                </option>

                <option value={7}>
                  Associated With
                </option>

              </select>

            </div>

            {/* TARGET SEARCH */}

            <div
              className="form-group form-grid-full"
            >

              <label>
                Target Indicator *
              </label>

              {!selectedTarget && (
                <div
                  className="target-search-container"
                >

                  <input
                    type="text"
                    value={searchTerm}
                    autoComplete="off"
                    placeholder="Search by indicator value..."
                    onChange={(event) =>
                      setSearchTerm(
                        event.target.value
                      )
                    }
                  />

                  {searching && (
                    <div
                      className="target-search-status"
                    >
                      Searching...
                    </div>
                  )}

                  {!searching &&
                    searchTerm.trim() &&
                    searchResults.length === 0 && (
                      <div
                        className="target-search-status"
                      >
                        No indicators found.
                      </div>
                    )}

                  {searchResults.length > 0 && (
                    <div
                      className="target-search-results"
                    >

                      {searchResults.map(
                        item => (
                          <button
                            key={item.id}
                            type="button"
                            className="target-search-item"
                            onClick={() =>
                              handleSelectTarget(
                                item
                              )
                            }
                          >

                            <div
                              className="target-search-main"
                            >
                              {item.value}
                            </div>

                            <div
                              className="target-search-meta"
                            >
                              ID: {item.id}
                              {' · '}
                              {item.type}
                              {' · '}
                              Risk: {
                                item.riskScore
                              }
                            </div>

                          </button>
                        )
                      )}

                    </div>
                  )}

                </div>
              )}

              {selectedTarget && (
                <div
                  className="selected-target"
                >

                  <div>

                    <span>
                      Selected Target
                    </span>

                    <strong>
                      {
                        selectedTarget.value
                      }
                    </strong>

                    <small>
                      ID: {
                        selectedTarget.id
                      }
                      {' · '}
                      {
                        selectedTarget.type
                      }
                    </small>

                  </div>

                  <button
                    type="button"
                    onClick={
                      handleRemoveTarget
                    }
                  >
                    Change
                  </button>

                </div>
              )}

            </div>

            {/* CONFIDENCE */}

            <div className="form-group">

              <label>
                Confidence *
              </label>

              <input
                type="number"
                min={0}
                max={100}
                required
                value={confidence}
                onChange={(event) =>
                  setConfidence(
                    Number(
                      event.target.value
                    )
                  )
                }
              />

            </div>

            {/* DESCRIPTION */}

            <div
              className="form-group form-grid-full"
            >

              <label>
                Description
              </label>

              <textarea
                rows={5}
                maxLength={1000}
                value={description}
                placeholder="Optional relation description..."
                onChange={(event) =>
                  setDescription(
                    event.target.value
                  )
                }
              />

            </div>

          </div>

        </div>

        {error && (
          <div className="form-error">
            {error}
          </div>
        )}

        {success && (
          <div className="form-success">
            {success}
          </div>
        )}

        <div className="form-actions">

          <button
            type="button"
            className="secondary-button"
            disabled={loading}
            onClick={() =>
              navigate(
                `/indicators/${sourceId}`
              )
            }
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={
              loading ||
              !selectedTarget
            }
          >
            {
              loading
                ? 'Creating...'
                : 'Create Relation'
            }
          </button>

        </div>

      </form>

    </div>
  )
}

export default CreateThreatRelationPage