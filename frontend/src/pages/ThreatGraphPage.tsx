import {
  useEffect,
  useMemo,
  useState,
} from 'react'

import {
  useNavigate,
  useSearchParams,
} from 'react-router-dom'

import {
  Background,
  Controls,
  MarkerType,
  MiniMap,
  ReactFlow,
  type Edge,
  type Node,
} from '@xyflow/react'

import '@xyflow/react/dist/style.css'

import {
  getThreatGraph,
} from '../services/threatGraphService'

import type {
  ThreatGraphResponse,
} from '../types/threatGraph'

import ThreatGraphSelector from '../components/ThreatGraphSelector'

function ThreatGraphPage() {
  const navigate = useNavigate()

  const [searchParams] =
    useSearchParams()

  const indicatorIdParam =
  searchParams.get('indicatorId')

const indicatorId =
  indicatorIdParam
    ? Number(indicatorIdParam)
    : null

  const [graph, setGraph] =
    useState<ThreatGraphResponse | null>(
      null
    )

  const [loading, setLoading] =
    useState(true)

  const [error, setError] =
    useState('')

  // Graph filters
  const [depth, setDepth] =
    useState(1)

  const [
    relationType,
    setRelationType,
  ] = useState('')

  const [
    automaticFilter,
    setAutomaticFilter,
  ] = useState('')

  const [
    minRiskScore,
    setMinRiskScore,
  ] = useState('')

  useEffect(() => {
    async function loadGraph() {
      if (indicatorId === null) {
  setLoading(false)
  return
}

if (Number.isNaN(indicatorId)) {
  setError(
    'Invalid indicator id.'
  )

  setLoading(false)

  return
}

      try {
        setLoading(true)
        setError('')

        const result =
          await getThreatGraph(
            indicatorId,
            {
              depth,

              relationType:
                relationType === ''
                  ? null
                  : Number(
                      relationType
                    ),

              isAutomatic:
                automaticFilter === ''
                  ? null
                  : automaticFilter ===
                    'true',

              minRiskScore:
                minRiskScore === ''
                  ? null
                  : Number(
                      minRiskScore
                    ),
            }
          )

        setGraph(result)
      } catch (error) {
        console.error(
          'Threat graph error:',
          error
        )

        setError(
          'Could not load threat graph.'
        )
      } finally {
        setLoading(false)
      }
    }

    loadGraph()
  }, [
    indicatorId,
    depth,
    relationType,
    automaticFilter,
    minRiskScore,
  ])

  const nodes =
    useMemo<Node[]>(() => {
      if (!graph) {
        return []
      }

      const centerX = 400
      const centerY = 250
      const radius = 220

      const otherNodes =
        graph.nodes.filter(
          node =>
            node.id !== indicatorId
        )

      return graph.nodes.map(
        node => {
          const isMainNode =
            node.id === indicatorId

          let x = centerX
          let y = centerY

          if (!isMainNode) {
            const otherIndex =
              otherNodes.findIndex(
                item =>
                  item.id === node.id
              )

            const angle =
              (
                2 *
                Math.PI *
                otherIndex
              ) /
              Math.max(
                otherNodes.length,
                1
              )

            x =
              centerX +
              radius *
                Math.cos(angle)

            y =
              centerY +
              radius *
                Math.sin(angle)
          }

          return {
            id:
              node.id.toString(),

            position: {
              x,
              y,
            },

            data: {
              label: (
                <div className="graph-node-content">
                  <strong>
                    {node.type}
                  </strong>

                  <span>
                    {node.value}
                  </span>

                  <small>
                    Risk: {node.riskScore}
                  </small>
                </div>
              ),
            },

            style: {
              width: 190,

              padding: '10px',

              borderRadius: '10px',

              border:
                isMainNode
                  ? '2px solid #818cf8'
                  : '1px solid #475569',

              background:
                isMainNode
                  ? '#1e1b4b'
                  : '#111827',

              color: '#f8fafc',
            },
          }
        }
      )
    }, [
      graph,
      indicatorId,
    ])

  const edges =
    useMemo<Edge[]>(() => {
      if (!graph) {
        return []
      }

      return graph.edges.map(
        edge => ({
          id:
            edge.relationId.toString(),

          source:
            edge.sourceId.toString(),

          target:
            edge.targetId.toString(),

          label:
            edge.relationType,

          markerEnd: {
            type:
              MarkerType.ArrowClosed,
          },

          animated:
            edge.isAutomatic,

          style: {
            strokeWidth: 2,
          },

          labelStyle: {
            fill: '#cbd5e1',
            fontSize: 11,
          },
        })
      )
    }, [graph])

if (indicatorId === null) {
  return (
    <ThreatGraphSelector />
  )
}

  if (loading) {
    return (
      <div className="threat-graph-page">
        <p>
          Loading threat graph...
        </p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="threat-graph-page">
        <p>
          {error}
        </p>
      </div>
    )
  }

  if (!graph) {
    return null
  }

  return (
    <div className="threat-graph-page">

      {/* Page header */}
      <div className="page-heading">
        <div>
          <h1>
            Threat Graph
          </h1>

          <p>
            Visualize relationships
            between threat indicators.
          </p>
        </div>
      </div>

      {/* Graph filters */}
      <div className="graph-filters">

        {/* Depth */}
        <div className="graph-filter-field">
          <label>
            Depth
          </label>

          <select
            value={depth}
            onChange={e =>
              setDepth(
                Number(
                  e.target.value
                )
              )
            }
          >
            <option value={1}>
              1
            </option>

            <option value={2}>
              2
            </option>

            <option value={3}>
              3
            </option>
          </select>
        </div>

        {/* Relation Type */}
        <div className="graph-filter-field">
          <label>
            Relation Type
          </label>

          <select
            value={relationType}
            onChange={e =>
              setRelationType(
                e.target.value
              )
            }
          >
            <option value="">
              All Relations
            </option>

            <option value="1">
              Related To
            </option>

            <option value="2">
              Resolves To
            </option>

            <option value="3">
              Hosts
            </option>

            <option value="4">
              Exploits
            </option>

            <option value="5">
              Downloads
            </option>

            <option value="6">
              Communicates With
            </option>

            <option value="7">
              Associated With
            </option>
          </select>
        </div>

        {/* Automatic / Manual */}
        <div className="graph-filter-field">
          <label>
            Relation Source
          </label>

          <select
            value={
              automaticFilter
            }
            onChange={e =>
              setAutomaticFilter(
                e.target.value
              )
            }
          >
            <option value="">
              All
            </option>

            <option value="true">
              Automatic
            </option>

            <option value="false">
              Manual
            </option>
          </select>
        </div>

        {/* Minimum Risk */}
        <div className="graph-filter-field">
          <label>
            Minimum Risk
          </label>

          <input
            type="number"
            min="0"
            max="100"
            placeholder="0 - 100"
            value={minRiskScore}
            onChange={e =>
              setMinRiskScore(
                e.target.value
              )
            }
          />
        </div>

      </div>

      {/* Graph summary */}
      <div className="graph-summary">

        <div className="graph-summary-card">
          <span>
            Nodes
          </span>

          <strong>
            {
              graph.summary
                .nodeCount
            }
          </strong>
        </div>

        <div className="graph-summary-card">
          <span>
            Relations
          </span>

          <strong>
            {
              graph.summary
                .edgeCount
            }
          </strong>
        </div>

        <div className="graph-summary-card">
          <span>
            Automatic
          </span>

          <strong>
            {
              graph.summary
                .automaticRelationCount
            }
          </strong>
        </div>

        <div className="graph-summary-card">
          <span>
            Manual
          </span>

          <strong>
            {
              graph.summary
                .manualRelationCount
            }
          </strong>
        </div>

        <div className="graph-summary-card">
          <span>
            Average Risk
          </span>

          <strong>
            {
              graph.summary
                .averageRiskScore
                .toFixed(1)
            }
          </strong>
        </div>

      </div>

      {/* Empty graph */}
      {graph.nodes.length === 0 ? (
        <div className="graph-empty">

          <h2>
            No relationships found
          </h2>

          <p>
            No threat relationships
            match the selected filters.
          </p>

        </div>
      ) : (
        /* React Flow graph */
        <div className="graph-container">

          <ReactFlow
            nodes={nodes}
            edges={edges}

            fitView

            nodesDraggable

            onNodeClick={(
              _event,
              node
            ) => {
              navigate(
                `/indicators/${node.id}`
              )
            }}
          >

            <Background />

            <Controls />

            <MiniMap />

          </ReactFlow>

        </div>
      )}

    </div>
  )
}

export default ThreatGraphPage