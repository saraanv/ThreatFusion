import type {
  ThreatAlert,
  UnreadAlertCount,
} from '../types/alert'

import {
  apiFetch,
} from './apiClient'

export async function getMyAlerts():
  Promise<ThreatAlert[]> {

  const response =
    await apiFetch(
      '/threat/api/alerts/GetMyAlerts',
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load alerts. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}

export async function getUnreadAlertCount():
  Promise<number> {

  const response =
    await apiFetch(
      '/threat/api/alerts/GetUnreadAlertCount',
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load unread alert count. Status: ${response.status}. ${errorText}`
    )
  }

  const result:
    UnreadAlertCount =
    await response.json()

  return result.count
}

export async function markAlertAsRead(
  alertId: number
): Promise<void> {

  const params =
    new URLSearchParams()

  params.set(
    'alertId',
    alertId.toString()
  )

  const response =
    await apiFetch(
      `/threat/api/alerts/MarkAlertAsRead?${params.toString()}`,
      {
        method: 'PATCH',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to mark alert as read. Status: ${response.status}. ${errorText}`
    )
  }
}