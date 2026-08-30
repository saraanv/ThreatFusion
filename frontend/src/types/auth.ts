export interface User {
  userId: number
  firstName: string
  lastName: string
  email: string
}

export interface LoginResponse {
  accessToken: string
  expiresAtUtc: string
  user: User
}

export interface RegisterUserRequest {
  firstName: string
  lastName: string
  email: string
  password: string
}

export interface RegisterUserResponse {
  userId: number
  message: string
}

export type AssignableRole =
  | 'Admin'
  | 'Analyst'
  | 'Viewer'

export interface AssignRoleRequest {
  userId: number
  role: AssignableRole
}

export interface AssignRoleResponse {
  message: string
}