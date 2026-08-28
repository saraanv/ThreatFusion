export interface ThreatAlert {
  id: number
  threatIndicatorId: number
  indicatorValue: string
  alertType: string
  title: string
  message: string
  severity: string
  isRead: boolean
  createdAtUtc: string
  readAtUtc: string | null
}

export interface UnreadAlertCount {
  count: number
}