import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { registerUser } from '../services/api'

function RegisterPage() {
  const navigate = useNavigate()

  const [firstName, setFirstName] =
    useState('')

  const [lastName, setLastName] =
    useState('')

  const [email, setEmail] =
    useState('')

  const [password, setPassword] =
    useState('')

  const [confirmPassword, setConfirmPassword] =
    useState('')

  const [error, setError] =
    useState('')

  const [loading, setLoading] =
    useState(false)

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    setError('')

    if (password !== confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    if (password.length < 8) {
      setError(
        'Password must be at least 8 characters.'
      )
      return
    }

    try {
      setLoading(true)

      await registerUser({
        firstName,
        lastName,
        email,
        password,
      })

      navigate('/login')
    } catch (error) {
      console.error(
        'Registration error:',
        error
      )

      if (error instanceof Error) {
        setError(error.message)
      } else {
        setError(
          'Registration failed.'
        )
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>ThreatFusion</h1>

        <p>
          Create your ThreatFusion account
        </p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>First Name</label>

            <input
              type="text"
              placeholder="Enter your first name"
              value={firstName}
              maxLength={100}
              required
              onChange={(event) =>
                setFirstName(
                  event.target.value
                )
              }
            />
          </div>

          <div className="form-group">
            <label>Last Name</label>

            <input
              type="text"
              placeholder="Enter your last name"
              value={lastName}
              maxLength={100}
              required
              onChange={(event) =>
                setLastName(
                  event.target.value
                )
              }
            />
          </div>

          <div className="form-group">
            <label>Email</label>

            <input
              type="email"
              placeholder="Enter your email"
              value={email}
              maxLength={256}
              required
              onChange={(event) =>
                setEmail(
                  event.target.value
                )
              }
            />
          </div>

          <div className="form-group">
            <label>Password</label>

            <input
              type="password"
              placeholder="At least 8 characters"
              value={password}
              minLength={8}
              required
              onChange={(event) =>
                setPassword(
                  event.target.value
                )
              }
            />
          </div>

          <div className="form-group">
            <label>
              Confirm Password
            </label>

            <input
              type="password"
              placeholder="Enter password again"
              value={confirmPassword}
              minLength={8}
              required
              onChange={(event) =>
                setConfirmPassword(
                  event.target.value
                )
              }
            />
          </div>

          {error && (
            <p className="auth-error">
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={loading}
          >
            {loading
              ? 'Creating account...'
              : 'Create Account'}
          </button>
        </form>

        <div className="auth-switch">
          Already have an account?{' '}

          <button
            type="button"
            onClick={() =>
              navigate('/login')
            }
          >
            Sign in
          </button>
        </div>
      </div>
    </div>
  )
}

export default RegisterPage