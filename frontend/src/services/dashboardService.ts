import type { DashboardOverview } from '../types/dashboard'

const API_BASE_URL = 'http://localhost:5152'

export async function getDashboardOverview(): Promise<DashboardOverview> {
  const token = localStorage.getItem('accessToken')

  if (!token) {
    throw new Error('Access token not found.')
  }

  const response = await fetch(
    `${API_BASE_URL}/threat/api/dashboard/GetOverview`,
    {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  )

  if (!response.ok) {
    throw new Error(
      `Failed to load dashboard. Status: ${response.status}`
    )
  }

  return response.json()
}