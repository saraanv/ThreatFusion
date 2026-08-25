import { useState } from 'react'
import { login } from '../services/api'
import { useNavigate } from 'react-router-dom'

function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
const navigate = useNavigate()

  async function handleSubmit(
  event: React.FormEvent<HTMLFormElement>
) {
  event.preventDefault()

  try {
    setError('')

    const result = await login(
      email,
      password
    )

    localStorage.setItem(
      'accessToken',
      result.accessToken
    )

    localStorage.setItem(
      'user',
      JSON.stringify(result.user)
    )

    localStorage.setItem(
      'expiresAtUtc',
      result.expiresAtUtc
    )

    console.log(
      'Login successful:',
      result
    )

    navigate('/dashboard')
  } catch (error) {
    console.error(
      'Login error:',
      error
    )

    setError('Login failed.')
  }
}

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>ThreatFusion</h1>
        <p>Threat Intelligence Platform</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Email</label>

            <input
              type="email"
              placeholder="Enter your email"
              value={email}
              onChange={(event) =>
                setEmail(event.target.value)
              }
            />
          </div>

          <div className="form-group">
            <label>Password</label>

            <input
              type="password"
              placeholder="Enter your password"
              value={password}
              onChange={(event) =>
                setPassword(event.target.value)
              }
            />
          </div>

          {error && (
            <p style={{ color: '#f87171' }}>
              {error}
            </p>
          )}

          <button type="submit">
            Sign in
          </button>
        </form>
      </div>
    </div>
  )
}

export default LoginPage