import { jwtDecode } from 'jwt-decode'

export type UserRole =
  | 'Admin'
  | 'Analyst'
  | 'Viewer'

interface JwtPayload {
  sub: string
  email: string
  given_name: string
  family_name: string
  jti: string
  role?: string | string[]
  exp: number
  iss: string
  aud: string
}

function getToken(): string | null {
  return localStorage.getItem('accessToken')
}

export function getCurrentUserRoles(): UserRole[] {
  const token = getToken()

  if (!token) {
    return []
  }

  try {
    const payload =
      jwtDecode<JwtPayload>(token)

    if (!payload.role) {
      return []
    }

    const roles = Array.isArray(payload.role)
      ? payload.role
      : [payload.role]

    return roles.filter(
      (role): role is UserRole =>
        role === 'Admin' ||
        role === 'Analyst' ||
        role === 'Viewer'
    )
  } catch (error) {
    console.error(
      'Could not decode access token:',
      error
    )

    return []
  }
}

export function hasRole(
  role: UserRole
): boolean {
  return getCurrentUserRoles().includes(role)
}

export function hasAnyRole(
  roles: UserRole[]
): boolean {
  const currentRoles =
    getCurrentUserRoles()

  return roles.some(role =>
    currentRoles.includes(role)
  )
}

export function isAdmin(): boolean {
  return hasRole('Admin')
}

export function isAnalyst(): boolean {
  return hasRole('Analyst')
}

export function isViewer(): boolean {
  return hasRole('Viewer')
}