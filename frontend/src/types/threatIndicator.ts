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

export type IndicatorTypeValue =
  | 1 // IpAddress
  | 2 // FileHash
  | 3 // Domain
  | 4 // Url
  | 5 // Email
  | 8 // Cve

export type ThreatSeverityValue =
  | 0 // Unknown
  | 1 // Low
  | 2 // Medium
  | 3 // High
  | 4 // Critical

export interface CreateThreatIndicatorRequest {
  type: IndicatorTypeValue
  value: string
  severity: ThreatSeverityValue
  confidence: number
  sourceName: string
  description: string | null
  firstSeenUtc: string | null
  lastSeenUtc: string | null
  cvssScore: number | null
  cvssVersion: string | null
  cvssVector: string | null
  cweId: string | null
  referenceUrl: string | null
}

export interface CreateThreatIndicatorResponse {
  indicatorId: number
  status: string | null
}