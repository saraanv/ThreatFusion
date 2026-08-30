import type {
  ThreatIndicator,
  ThreatIndicatorListResponse,
  CreateThreatIndicatorRequest,
  CreateThreatIndicatorResponse,
} from '../types/threatIndicator'

import {
  apiFetch,
} from './apiClient'

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
    await apiFetch(
      `/threat/api/threat-indicators/GetThreatIndicators?${params.toString()}`,
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load threat indicators. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}

export async function getThreatIndicatorById(
  id: number
): Promise<ThreatIndicator> {

  const params =
    new URLSearchParams()

  params.set(
    'id',
    id.toString()
  )

  const response =
    await apiFetch(
      `/threat/api/threat-indicators/GetThreatIndicatorById?${params.toString()}`,
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load threat indicator. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}

export async function createThreatIndicator(
  request: CreateThreatIndicatorRequest
): Promise<CreateThreatIndicatorResponse> {
  const response = await apiFetch(
    '/threat/api/threat-indicators/CreateThreatIndicator',
    {
      method: 'POST',
      body: JSON.stringify(request),
    }
  )

  if (!response.ok) {
    let message =
      'Failed to create threat indicator.'

    try {
      const errorData = await response.json()

      if (
        Array.isArray(errorData.errors) &&
        errorData.errors.length > 0
      ) {
        message =
          errorData.errors.join(', ')
      } else if (errorData.message) {
        message =
          errorData.message
      }
    } catch {
      
    }

    throw new Error(message)
  }

  return response.json()
}

