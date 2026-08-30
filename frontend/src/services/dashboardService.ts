import type {
  DashboardOverview,
  ThreatDashboard,
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
      `Failed to load dashboard overview. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}

export async function getThreatDashboard():
  Promise<ThreatDashboard> {

  const response =
    await apiFetch(
      '/threat/api/dashboard/GetDashboard',
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load threat dashboard. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}