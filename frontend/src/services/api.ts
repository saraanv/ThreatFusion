import type { LoginResponse } from '../types/auth'

const API_BASE_URL = 'http://localhost:5152'

export async function login(
  email: string,
  password: string
): Promise<LoginResponse> {
  const response = await fetch(
    `${API_BASE_URL}/identity/api/auth/LoginUser`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email,
        password,
      }),
    }
  )

  if (!response.ok) {
    throw new Error('Login failed')
  }

  return response.json()
}