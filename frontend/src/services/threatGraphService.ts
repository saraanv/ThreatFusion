import type {
  ThreatGraphResponse,
} from '../types/threatGraph'

import {
  apiFetch,
} from './apiClient'

export interface ThreatGraphFilters {
  depth?: number
  relationType?: number | null
  isAutomatic?: boolean | null
  minRiskScore?: number | null
}

export async function getThreatGraph(
  indicatorId: number,
  filters: ThreatGraphFilters = {}
): Promise<ThreatGraphResponse> {
  const params =
    new URLSearchParams()

  params.append(
    'indicatorId',
    indicatorId.toString()
  )

  params.append(
    'depth',
    (
      filters.depth ?? 1
    ).toString()
  )

  if (
    filters.relationType !== null &&
    filters.relationType !== undefined
  ) {
    params.append(
      'relationType',
      filters.relationType.toString()
    )
  }

  if (
    filters.isAutomatic !== null &&
    filters.isAutomatic !== undefined
  ) {
    params.append(
      'isAutomatic',
      filters.isAutomatic.toString()
    )
  }

  if (
    filters.minRiskScore !== null &&
    filters.minRiskScore !== undefined
  ) {
    params.append(
      'minRiskScore',
      filters.minRiskScore.toString()
    )
  }

  const response =
    await apiFetch(
      `/threat/api/threat-relations/GetThreatGraph?${params.toString()}`,
      {
        method: 'GET',
      }
    )

  if (!response.ok) {
    const errorText =
      await response.text()

    throw new Error(
      `Failed to load threat graph. Status: ${response.status}. ${errorText}`
    )
  }

  return response.json()
}