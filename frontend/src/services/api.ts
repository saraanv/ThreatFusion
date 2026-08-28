import type {
  LoginResponse,
  RegisterUserRequest,
  RegisterUserResponse,
} from '../types/auth'

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

export async function registerUser(
  request: RegisterUserRequest
): Promise<RegisterUserResponse> {
  const response = await fetch(
    `${API_BASE_URL}/identity/api/auth/RegisterUser`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    }
  )

  if (!response.ok) {
    const errorData = await response.json()

    const message =
      errorData.errors?.join(', ') ||
      'Registration failed.'

    throw new Error(message)
  }

  return response.json()
}