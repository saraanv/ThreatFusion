export interface ThreatGraphNode {
  id: number
  type: string
  value: string
  severity: string
  riskScore: number
  riskLevel: string
  sourceName: string
}

export interface ThreatGraphEdge {
  relationId: number
  sourceId: number
  targetId: number
  relationType: string
  confidence: number
  description: string | null
  sourceName: string
  isAutomatic: boolean
  discoveredAtUtc: string
}

export interface ThreatGraphSummary {
  nodeCount: number
  edgeCount: number
  criticalNodeCount: number
  highRiskNodeCount: number
  automaticRelationCount: number
  manualRelationCount: number
  averageRiskScore: number
  highestRiskIndicatorId: number | null
  highestRiskIndicatorValue: string | null
  highestRiskScore: number | null
}

export interface ThreatGraphResponse {
  nodes: ThreatGraphNode[]
  edges: ThreatGraphEdge[]
  summary: ThreatGraphSummary
}