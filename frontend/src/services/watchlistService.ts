import type {
  WatchlistItem,
} from '../types/watchlist'

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

export async function addToWatchlist(
  threatIndicatorId: number,
  note: string | null = null
): Promise<void> {
  const token =
    getAccessToken()

  const response =
    await fetch(
      `${API_BASE_URL}/threat/api/watchlists/AddToWatchlist`,
      {
        method: 'POST',

        headers: {
          Authorization:
            `Bearer ${token}`,

          'Content-Type':
            'application/json',
        },

        body: JSON.stringify({
          threatIndicatorId,
          note,
        }),
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to add indicator to watchlist. Status: ${response.status}. ${errorText}`
    )
  }
}

export async function getMyWatchlist():
  Promise<WatchlistItem[]> {
  const token =
    getAccessToken()

  const response =
    await fetch(
      `${API_BASE_URL}/threat/api/watchlists/GetMyWatchlist`,
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
      `Failed to load watchlist. Status: ${response.status}`
    )
  }

  return response.json()
}

export async function removeFromWatchlist(
  threatIndicatorId: number
): Promise<void> {
  const token =
    getAccessToken()

  const response =
    await fetch(
      `${API_BASE_URL}/threat/api/watchlists/RemoveFromWatchlist?threatIndicatorId=${threatIndicatorId}`,
      {
        method: 'DELETE',

        headers: {
          Authorization:
            `Bearer ${token}`,
        },
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to remove indicator from watchlist. Status: ${response.status}. ${errorText}`
    )
  }
}