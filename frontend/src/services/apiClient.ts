const API_BASE_URL = 'http://localhost:5152'

function clearAuthentication() {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('user')
  localStorage.removeItem('expiresAtUtc')
}

function redirectToLogin() {
  clearAuthentication()

  if (window.location.pathname !== '/login') {
    window.location.href = '/login'
  }
}

export async function apiFetch(
  path: string,
  options: RequestInit = {}
): Promise<Response> {
  const token =
    localStorage.getItem('accessToken')

  const headers =
    new Headers(options.headers)

  if (token) {
    headers.set(
      'Authorization',
      `Bearer ${token}`
    )
  }

  if (
    options.body &&
    !(options.body instanceof FormData) &&
    !headers.has('Content-Type')
  ) {
    headers.set(
      'Content-Type',
      'application/json'
    )
  }

  const response = await fetch(
    `${API_BASE_URL}${path}`,
    {
      ...options,
      headers,
    }
  )

  if (response.status === 401) {
    redirectToLogin()

    throw new Error(
      'Your session has expired. Please log in again.'
    )
  }

  return response
}