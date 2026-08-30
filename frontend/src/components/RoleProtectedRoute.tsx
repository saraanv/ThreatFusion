import {
  Navigate,
} from 'react-router-dom'

import {
  hasAnyRole,
} from '../utils/auth'

import type {
  UserRole,
} from '../utils/auth'

interface RoleProtectedRouteProps {
  children: React.ReactNode
  allowedRoles: UserRole[]
}

function RoleProtectedRoute({
  children,
  allowedRoles,
}: RoleProtectedRouteProps) {
  const hasAccess =
    hasAnyRole(allowedRoles)

  if (!hasAccess) {
    return (
      <Navigate
        to="/dashboard"
        replace
      />
    )
  }

  return children
}

export default RoleProtectedRoute