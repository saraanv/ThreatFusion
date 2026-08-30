export type ThreatRelationTypeValue =
  | 1 // RelatedTo
  | 2 // ResolvesTo
  | 3 // Hosts
  | 4 // Exploits
  | 5 // Downloads
  | 6 // CommunicatesWith
  | 7 // AssociatedWith

export interface CreateThreatRelationRequest {
  sourceIndicatorId: number
  targetIndicatorId: number
  relationType: ThreatRelationTypeValue
  description: string | null
  confidence: number
}

export interface CreateThreatRelationResponse {
  relationId: number
  message: string
}