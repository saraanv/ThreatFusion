import type {
  ThreatIndicator,
  ThreatIndicatorListResponse,
} from '../types/threatIndicator'

const API_BASE_URL =
  'http://localhost:5152'

export interface ThreatIndicatorFilters {
  searchTerm?: string
  type?: string
  severity?: string
  riskLevel?: string
  source?: string
  minRiskScore?: number
  maxRiskScore?: number
  isActive?: boolean
}

export async function getThreatIndicators(
  pageNumber: number = 1,
  pageSize: number = 20,
  filters: ThreatIndicatorFilters = {}
): Promise<ThreatIndicatorListResponse> {
  const token =
    localStorage.getItem('accessToken')

  if (!token) {
    throw new Error(
      'Access token not found.'
    )
  }

  const params =
    new URLSearchParams()

  params.set(
    'pageNumber',
    pageNumber.toString()
  )

  params.set(
    'pageSize',
    pageSize.toString()
  )

  if (filters.searchTerm?.trim()) {
    params.set(
      'searchTerm',
      filters.searchTerm.trim()
    )
  }

  if (filters.type) {
    params.set(
      'type',
      filters.type
    )
  }

  if (filters.severity) {
    params.set(
      'severity',
      filters.severity
    )
  }

  if (filters.riskLevel) {
    params.set(
      'riskLevel',
      filters.riskLevel
    )
  }

  if (filters.source?.trim()) {
    params.set(
      'source',
      filters.source.trim()
    )
  }

  if (
    filters.minRiskScore !== undefined
  ) {
    params.set(
      'minRiskScore',
      filters.minRiskScore.toString()
    )
  }

  if (
    filters.maxRiskScore !== undefined
  ) {
    params.set(
      'maxRiskScore',
      filters.maxRiskScore.toString()
    )
  }

  if (
    filters.isActive !== undefined
  ) {
    params.set(
      'isActive',
      filters.isActive.toString()
    )
  }

  const response =
    await fetch(
      `${API_BASE_URL}/threat/api/threat-indicators/GetThreatIndicators?${params.toString()}`,
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
      `Failed to load threat indicators. Status: ${response.status}`
    )
  }

  return response.json()
}

export async function getThreatIndicatorById(
  id: number
): Promise<ThreatIndicator> {
  const token =
    localStorage.getItem('accessToken')

  if (!token) {
    throw new Error(
      'Access token not found.'
    )
  }

  const response =
    await fetch(
      `${API_BASE_URL}/threat/api/threat-indicators/GetThreatIndicatorById?id=${id}`,
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
      `Failed to load threat indicator. Status: ${response.status}`
    )
  }

  return response.json()
}