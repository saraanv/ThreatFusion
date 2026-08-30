import {
  apiFetch,
} from './apiClient'

import type {
  AssignRoleRequest,
  AssignRoleResponse,
} from '../types/auth'

export async function assignRole(
  request: AssignRoleRequest
): Promise<AssignRoleResponse> {
  const response = await apiFetch(
    '/identity/api/auth/AssignRole',
    {
      method: 'POST',
      body: JSON.stringify(request),
    }
  )

  if (!response.ok) {
    let message =
      'Failed to assign role.'

    try {
      const errorData =
        await response.json()

      if (
        Array.isArray(errorData.errors) &&
        errorData.errors.length > 0
      ) {
        message =
          errorData.errors.join(', ')
      } else if (
        Array.isArray(errorData.Errors) &&
        errorData.Errors.length > 0
      ) {
        message =
          errorData.Errors.join(', ')
      } else if (errorData.message) {
        message =
          errorData.message
      }
    } catch {
        
    }

    throw new Error(message)
  }

  return response.json()
}