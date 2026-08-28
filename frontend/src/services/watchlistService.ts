import type {
  WatchlistItem,
} from '../types/watchlist'

import {
  apiFetch,
} from './apiClient'

export async function addToWatchlist(
  threatIndicatorId: number,
  note: string | null = null
): Promise<void> {

  const response =
    await apiFetch(
      '/threat/api/watchlists/AddToWatchlist',
      {
        method: 'POST',

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

  const response =
    await apiFetch(
      '/threat/api/watchlists/GetMyWatchlist',
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load watchlist. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}

export async function removeFromWatchlist(
  threatIndicatorId: number
): Promise<void> {

  const params =
    new URLSearchParams()

  params.set(
    'threatIndicatorId',
    threatIndicatorId.toString()
  )

  const response =
    await apiFetch(
      `/threat/api/watchlists/RemoveFromWatchlist?${params.toString()}`,
      {
        method: 'DELETE',
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