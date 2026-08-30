export interface DashboardRiskyIndicator {
  id: number
  type: string
  value: string
  severity: string
  riskScore: number
  riskLevel: string
  sourceName: string
}

export interface DashboardRecentAlert {
  id: number
  threatIndicatorId: number
  indicatorValue: string
  alertType: string
  title: string
  severity: string
  isRead: boolean
  createdAtUtc: string
}

export interface DashboardDistribution {
  name: string
  count: number
}

export interface DashboardFeedSync {
  feedName: string
  startedAtUtc: string
  completedAtUtc: string | null
  totalFetched: number
  failedCount: number
  isSuccessful: boolean
  errorMessage: string | null
}

export interface DashboardOverview {
  totalIndicators: number
  criticalIndicators: number
  highRiskIndicators: number
  watchedIndicators: number
  unreadAlerts: number
  automaticRelations: number
  manualRelations: number

  topRiskyIndicators: DashboardRiskyIndicator[]
  recentAlerts: DashboardRecentAlert[]

  severityDistribution: DashboardDistribution[]
  indicatorTypeDistribution: DashboardDistribution[]

  lastFeedSync: DashboardFeedSync | null
}

export interface IndicatorTypeCount {
  type: string
  count: number
}

export interface SourceCount {
  sourceName: string
  count: number
}

export interface LatestThreat {
  id: number
  type: string
  value: string
  severity: string
  confidence: number
  sourceName: string
  createdAtUtc: string
}

export interface ThreatDashboardFeedSync {
  feedName: string
  startedAtUtc: string
  completedAtUtc: string | null
  totalFetched: number
  createdCount: number
  updatedCount: number
  unchangedCount: number
  failedCount: number
  isSuccessful: boolean
}

export interface ThreatDashboard {
  totalIndicators: number
  activeIndicators: number
  criticalIndicators: number

  indicatorsByType: IndicatorTypeCount[]
  indicatorsBySource: SourceCount[]
  latestThreats: LatestThreat[]

  lastFeedSync: ThreatDashboardFeedSync | null
}