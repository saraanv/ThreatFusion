import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

import {
  createThreatIndicator,
} from '../services/threatIndicatorService'

import type {
  IndicatorTypeValue,
  ThreatSeverityValue,
} from '../types/threatIndicator'

function CreateThreatIndicatorPage() {
  const navigate = useNavigate()

  const [type, setType] =
    useState<IndicatorTypeValue>(1)

  const [value, setValue] =
    useState('')

  const [severity, setSeverity] =
    useState<ThreatSeverityValue>(0)

  const [confidence, setConfidence] =
    useState(80)

  const [sourceName, setSourceName] =
    useState('Manual')

  const [description, setDescription] =
    useState('')

  const [firstSeenUtc, setFirstSeenUtc] =
    useState('')

  const [lastSeenUtc, setLastSeenUtc] =
    useState('')

  const [cvssScore, setCvssScore] =
    useState('')

  const [cvssVersion, setCvssVersion] =
    useState('')

  const [cvssVector, setCvssVector] =
    useState('')

  const [cweId, setCweId] =
    useState('')

  const [referenceUrl, setReferenceUrl] =
    useState('')

  const [error, setError] =
    useState('')

  const [loading, setLoading] =
    useState(false)

  function toUtcOrNull(
    value: string
  ): string | null {
    if (!value) {
      return null
    }

    return new Date(value).toISOString()
  }

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    setError('')

    if (
      confidence < 0 ||
      confidence > 100
    ) {
      setError(
        'Confidence must be between 0 and 100.'
      )

      return
    }

    const parsedCvssScore =
      cvssScore === ''
        ? null
        : Number(cvssScore)

    if (
      parsedCvssScore !== null &&
      Number.isNaN(parsedCvssScore)
    ) {
      setError(
        'CVSS Score must be a valid number.'
      )

      return
    }

    try {
      setLoading(true)

      const result =
        await createThreatIndicator({
          type,
          value: value.trim(),
          severity,
          confidence,

          sourceName:
            sourceName.trim(),

          description:
            description.trim() || null,

          firstSeenUtc:
            toUtcOrNull(firstSeenUtc),

          lastSeenUtc:
            toUtcOrNull(lastSeenUtc),

          cvssScore:
            parsedCvssScore,

          cvssVersion:
            cvssVersion.trim() || null,

          cvssVector:
            cvssVector.trim() || null,

          cweId:
            cweId.trim() || null,

          referenceUrl:
            referenceUrl.trim() || null,
        })

      console.log(
        'Create indicator result:',
        result
      )

      navigate(
        `/indicators/${result.indicatorId}`,
        {
          state: {
            createStatus:
              result.status,
          },
        }
      )
    } catch (error) {
      console.error(
        'Create indicator error:',
        error
      )

      if (error instanceof Error) {
        setError(error.message)
      } else {
        setError(
          'Failed to create threat indicator.'
        )
      }
    } finally {
      setLoading(false)
    }
  }

  function getValuePlaceholder() {
    switch (type) {
      case 1:
        return 'Example: 192.168.1.10'

      case 2:
        return 'MD5, SHA-1 or SHA-256 hash'

      case 3:
        return 'Example: example.com'

      case 4:
        return 'Example: https://example.com/malware'

      case 5:
        return 'Example: attacker@example.com'

      case 8:
        return 'Example: CVE-2026-12345'

      default:
        return 'Enter indicator value'
    }
  }

  return (
    <div className="create-indicator-page">

      <div className="page-heading">

        <div>
          <h1>
            Create Threat Indicator
          </h1>

          <p>
            Manually create or update a threat
            intelligence indicator.
          </p>
        </div>

        <button
          type="button"
          className="secondary-button"
          onClick={() =>
            navigate('/indicators')
          }
        >
          Back to Indicators
        </button>

      </div>

      <form
        className="indicator-form"
        onSubmit={handleSubmit}
      >

        {/* ========================= */}
        {/* BASIC INFORMATION */}
        {/* ========================= */}

        <div className="form-section">

          <h2>
            Indicator Information
          </h2>

          <div className="form-grid">

            <div className="form-group">

              <label>
                Indicator Type *
              </label>

              <select
                value={type}
                onChange={(event) =>
                  setType(
                    Number(
                      event.target.value
                    ) as IndicatorTypeValue
                  )
                }
              >

                <option value={1}>
                  IP Address
                </option>

                <option value={2}>
                  File Hash
                </option>

                <option value={3}>
                  Domain
                </option>

                <option value={4}>
                  URL
                </option>

                <option value={5}>
                  Email
                </option>

                <option value={8}>
                  CVE
                </option>

              </select>

            </div>

            <div className="form-group">

              <label>
                Severity *
              </label>

              <select
                value={severity}
                onChange={(event) =>
                  setSeverity(
                    Number(
                      event.target.value
                    ) as ThreatSeverityValue
                  )
                }
              >

                <option value={0}>
                  Unknown
                </option>

                <option value={1}>
                  Low
                </option>

                <option value={2}>
                  Medium
                </option>

                <option value={3}>
                  High
                </option>

                <option value={4}>
                  Critical
                </option>

              </select>

            </div>

            <div
              className="form-group form-grid-full"
            >

              <label>
                Indicator Value *
              </label>

              <input
                type="text"
                required
                value={value}
                placeholder={
                  getValuePlaceholder()
                }
                onChange={(event) =>
                  setValue(
                    event.target.value
                  )
                }
              />

            </div>

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

            <div className="form-group">

              <label>
                Source Name *
              </label>

              <input
                type="text"
                required
                maxLength={200}
                value={sourceName}
                placeholder="Example: Manual"
                onChange={(event) =>
                  setSourceName(
                    event.target.value
                  )
                }
              />

            </div>

          </div>

        </div>

        {/* ========================= */}
        {/* OBSERVATION */}
        {/* ========================= */}

        <div className="form-section">

          <h2>
            Observation
          </h2>

          <div className="form-grid">

            <div className="form-group">

              <label>
                First Seen
              </label>

              <input
                type="datetime-local"
                value={firstSeenUtc}
                onChange={(event) =>
                  setFirstSeenUtc(
                    event.target.value
                  )
                }
              />

            </div>

            <div className="form-group">

              <label>
                Last Seen
              </label>

              <input
                type="datetime-local"
                value={lastSeenUtc}
                onChange={(event) =>
                  setLastSeenUtc(
                    event.target.value
                  )
                }
              />

            </div>

            <div
              className="form-group form-grid-full"
            >

              <label>
                Description
              </label>

              <textarea
                value={description}
                maxLength={2000}
                rows={5}
                placeholder="Optional description..."
                onChange={(event) =>
                  setDescription(
                    event.target.value
                  )
                }
              />

            </div>

          </div>

        </div>

        {/* ========================= */}
        {/* CVE INFORMATION */}
        {/* ========================= */}

        {type === 8 && (
          <div className="form-section">

            <h2>
              CVE Information
            </h2>

            <div className="form-grid">

              <div className="form-group">

                <label>
                  CVSS Score
                </label>

                <input
                  type="number"
                  step="0.1"
                  value={cvssScore}
                  placeholder="Example: 9.8"
                  onChange={(event) =>
                    setCvssScore(
                      event.target.value
                    )
                  }
                />

              </div>

              <div className="form-group">

                <label>
                  CVSS Version
                </label>

                <input
                  type="text"
                  value={cvssVersion}
                  placeholder="Example: 3.1"
                  onChange={(event) =>
                    setCvssVersion(
                      event.target.value
                    )
                  }
                />

              </div>

              <div
                className="form-group form-grid-full"
              >

                <label>
                  CVSS Vector
                </label>

                <input
                  type="text"
                  value={cvssVector}
                  placeholder="Example: CVSS:3.1/AV:N/AC:L/..."
                  onChange={(event) =>
                    setCvssVector(
                      event.target.value
                    )
                  }
                />

              </div>

              <div className="form-group">

                <label>
                  CWE ID
                </label>

                <input
                  type="text"
                  value={cweId}
                  placeholder="Example: CWE-287"
                  onChange={(event) =>
                    setCweId(
                      event.target.value
                    )
                  }
                />

              </div>

              <div className="form-group">

                <label>
                  Reference URL
                </label>

                <input
                  type="url"
                  value={referenceUrl}
                  placeholder="https://..."
                  onChange={(event) =>
                    setReferenceUrl(
                      event.target.value
                    )
                  }
                />

              </div>

            </div>

          </div>
        )}

        {/* ========================= */}
        {/* ERROR */}
        {/* ========================= */}

        {error && (
          <div className="form-error">
            {error}
          </div>
        )}

        {/* ========================= */}
        {/* ACTIONS */}
        {/* ========================= */}

        <div className="form-actions">

          <button
            type="button"
            className="secondary-button"
            disabled={loading}
            onClick={() =>
              navigate('/indicators')
            }
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={loading}
          >
            {loading
              ? 'Saving...'
              : 'Save Indicator'}
          </button>

        </div>

      </form>

    </div>
  )
}

export default CreateThreatIndicatorPage