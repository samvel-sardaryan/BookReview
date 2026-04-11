import { createContext, useContext, useState, type ReactNode } from 'react'
import { getStoredToken, storeToken, clearToken } from '../api'

interface AuthContextType {
  isLoggedIn: boolean
  login: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextType | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(() => !!getStoredToken())

  function login(token: string) {
    storeToken(token)
    setIsLoggedIn(true)
  }

  function logout() {
    clearToken()
    setIsLoggedIn(false)
  }

  return (
    <AuthContext.Provider value={{ isLoggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextType {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
