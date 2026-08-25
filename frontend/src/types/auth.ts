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