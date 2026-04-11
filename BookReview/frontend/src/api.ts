import type { BookDto, AuthorDto, ReviewDto, UserDto } from './types'

const BASE = '/api'

function getToken(): string | null {
  return localStorage.getItem('token')
}

function authHeaders(): Record<string, string> {
  const token = getToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...authHeaders(),
      ...(options.headers as Record<string, string>),
    },
    ...options,
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `Request failed with status ${res.status}`)
  }

  const contentType = res.headers.get('content-type')
  if (contentType?.includes('application/json')) {
    return res.json() as Promise<T>
  }
  return res.text() as Promise<T>
}

export const api = {
  // Books
  getBooks: () => request<BookDto[]>('/books'),
  getBook: (id: number) => request<BookDto>(`/books/${id}`),
  getBookRating: (id: number) => request<number>(`/books/${id}/rating`),

  // Authors
  getAuthors: () => request<AuthorDto[]>('/authors'),
  getAuthorsOfBook: (bookId: number) => request<AuthorDto[]>(`/authors/book/${bookId}`),

  // Reviews
  getReviewsOfBook: (bookId: number) => request<ReviewDto[]>(`/reviews/book/${bookId}`),

  // Auth
  login: (data: UserDto) =>
    request<string>('/auth/login', { method: 'POST', body: JSON.stringify(data) }),
  register: (data: UserDto) =>
    request<unknown>('/auth/register', { method: 'POST', body: JSON.stringify(data) }),
}

export function getStoredToken(): string | null {
  return localStorage.getItem('token')
}

export function storeToken(token: string): void {
  localStorage.setItem('token', token)
}

export function clearToken(): void {
  localStorage.removeItem('token')
}
