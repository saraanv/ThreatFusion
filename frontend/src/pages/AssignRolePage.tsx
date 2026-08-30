import {
  useState,
} from 'react'

import {
  useNavigate,
} from 'react-router-dom'

import {
  assignRole,
} from '../services/adminService'

import type {
  AssignableRole,
} from '../types/auth'

function AssignRolePage() {
  const navigate =
    useNavigate()

  const [userId, setUserId] =
    useState('')

  const [role, setRole] =
    useState<AssignableRole>('Viewer')

  const [loading, setLoading] =
    useState(false)

  const [error, setError] =
    useState('')

  const [success, setSuccess] =
    useState('')

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    setError('')
    setSuccess('')

    const parsedUserId =
      Number(userId)

    if (
      !Number.isInteger(parsedUserId) ||
      parsedUserId <= 0
    ) {
      setError(
        'Please enter a valid User ID.'
      )

      return
    }

    try {
      setLoading(true)

      const result =
        await assignRole({
          userId: parsedUserId,
          role,
        })

      setSuccess(
        result.message
      )

      console.log(
        'Assign role result:',
        result
      )
    } catch (error) {
      console.error(
        'Assign role error:',
        error
      )

      if (error instanceof Error) {
        setError(
          error.message
        )
      } else {
        setError(
          'Failed to assign role.'
        )
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="assign-role-page">

      <div className="page-heading">

        <div>
          <h1>
            Assign User Role
          </h1>

          <p>
            Assign a ThreatFusion role
            to an existing user.
          </p>
        </div>

        <button
          type="button"
          className="secondary-button"
          onClick={() =>
            navigate('/dashboard')
          }
        >
          Back to Dashboard
        </button>

      </div>

      <form
        className="assign-role-form"
        onSubmit={handleSubmit}
      >

        <div className="form-section">

          <h2>
            Role Assignment
          </h2>

          <div className="form-grid">

            <div className="form-group">

              <label>
                User ID *
              </label>

              <input
                type="number"
                min={1}
                required
                value={userId}
                placeholder="Example: 2"
                onChange={(event) =>
                  setUserId(
                    event.target.value
                  )
                }
              />

            </div>

            <div className="form-group">

              <label>
                Role *
              </label>

              <select
                value={role}
                onChange={(event) =>
                  setRole(
                    event.target
                      .value as AssignableRole
                  )
                }
              >

                <option value="Viewer">
                  Viewer
                </option>

                <option value="Analyst">
                  Analyst
                </option>

                <option value="Admin">
                  Admin
                </option>

              </select>

            </div>

          </div>

          <div className="role-help">

            <strong>
              Role permissions
            </strong>

            <p>
              Viewer — View threat intelligence,
              watchlist, alerts and graph.
            </p>

            <p>
              Analyst — Viewer permissions plus
              creating indicators and relations.
            </p>

            <p>
              Admin — Full access including
              role assignment.
            </p>

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
              navigate('/dashboard')
            }
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={loading}
          >
            {
              loading
                ? 'Assigning...'
                : 'Assign Role'
            }
          </button>

        </div>

      </form>

    </div>
  )
}

export default AssignRolePage