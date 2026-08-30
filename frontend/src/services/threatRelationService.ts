import {
  apiFetch,
} from './apiClient'

import type {
  CreateThreatRelationRequest,
  CreateThreatRelationResponse,
} from '../types/threatRelation'

export async function createThreatRelation(
  request: CreateThreatRelationRequest
): Promise<CreateThreatRelationResponse> {
  const response = await apiFetch(
    '/threat/api/threat-relations/CreateRelation',
    {
      method: 'POST',
      body: JSON.stringify(request),
    }
  )

  if (!response.ok) {
    let message =
      'Failed to create threat relation.'

    try {
      const errorData =
        await response.json()

      if (
        Array.isArray(errorData.errors) &&
        errorData.errors.length > 0
      ) {
        message =
          errorData.errors.join(', ')
      } else if (errorData.message) {
        message =
          errorData.message
      }
    } catch {
      // Response was not JSON.
    }

    throw new Error(message)
  }

  return response.json()
}