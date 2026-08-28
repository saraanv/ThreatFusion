import type {
  ThreatAlert,
  UnreadAlertCount,
} from '../types/alert'

const API_BASE_URL =
  'http://localhost:5152'

function getAccessToken(): string {
  const token =
    localStorage.getItem('accessToken')

  if (!token) {
    throw new Error(
      'Access token not found.'
    )
  }

  return token
}

export async function getMyAlerts():
  Promise<ThreatAlert[]> {
  const token = getAccessToken()

  const response = await fetch(
    `${API_BASE_URL}/threat/api/alerts/GetMyAlerts`,
    {
      method: 'GET',

      headers: {
        Authorization:
          `Bearer ${token}`,
      },
    }
  )

  if (!response.ok) {
    throw new Error(
      `Failed to load alerts. Status: ${response.status}`
    )
  }

  return response.json()
}

export async function getUnreadAlertCount():
  Promise<number> {
  const token = getAccessToken()

  const response = await fetch(
    `${API_BASE_URL}/threat/api/alerts/GetUnreadAlertCount`,
    {
      method: 'GET',

      headers: {
        Authorization:
          `Bearer ${token}`,
      },
    }
  )

  if (!response.ok) {
    throw new Error(
      `Failed to load unread alert count. Status: ${response.status}`
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
  const token = getAccessToken()

  const response = await fetch(
    `${API_BASE_URL}/threat/api/alerts/MarkAlertAsRead?alertId=${alertId}`,
    {
      method: 'PATCH',

      headers: {
        Authorization:
          `Bearer ${token}`,
      },
    }
  )

  if (!response.ok) {
    throw new Error(
      `Failed to mark alert as read. Status: ${response.status}`
    )
  }
}