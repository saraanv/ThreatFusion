import type {
  DashboardOverview,
} from '../types/dashboard'

import {
  apiFetch,
} from './apiClient'

export async function getDashboardOverview():
  Promise<DashboardOverview> {

  const response =
    await apiFetch(
      '/threat/api/dashboard/GetOverview',
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load dashboard. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}