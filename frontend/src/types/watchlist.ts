export interface WatchlistItem {
  watchlistId: number
  indicatorId: number
  type: string
  value: string
  severity: string
  riskScore: number
  riskLevel: string
  sourceName: string
  note: string | null
  addedAtUtc: string
}