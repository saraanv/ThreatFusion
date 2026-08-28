export interface ThreatIndicator {
  id: number
  type: string
  value: string
  severity: string
  confidence: number
  riskScore: number
  riskLevel: string
  sourceName: string
  description: string | null
  firstSeenUtc: string | null
  lastSeenUtc: string | null
  cvssScore: number | null
  cvssVersion: string | null
  cvssVector: string | null
  cweId: string | null
  referenceUrl: string | null
  isActive: boolean
}

export interface ThreatIndicatorListResponse {
  items: ThreatIndicator[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}