import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api'
import { useApi } from '../hooks/useApi'
import StarRating from '../components/StarRating'
import type { BookDto } from '../types'

export default function Books() {
  const { data: books, loading, error } = useApi<BookDto[]>(() => api.getBooks())
  const [ratings, setRatings] = useState<Record<number, number>>({})

  useEffect(() => {
    if (!books) return
    Promise.all(
      books.map((book) =>
        api.getBookRating(book.id)
          .then((rating) => ({ id: book.id, rating }))
          .catch(() => ({ id: book.id, rating: 0 }))
      )
    ).then((results) => {
      const map: Record<number, number> = {}
      results.forEach(({ id, rating }) => { map[id] = rating })
      setRatings(map)
    })
  }, [books])

  if (loading) return <div className="loading">Loading books...</div>
  if (error) return <div className="error-msg">Failed to load books: {error}</div>

  return (
    <div>
      <h1 className="page-title">Books</h1>
      {!books?.length ? (
        <p className="empty-msg">No books found.</p>
      ) : (
        <div className="grid">
          {books.map((book) => (
            <Link to={`/books/${book.id}`} key={book.id} style={{ textDecoration: 'none' }}>
              <div className="card card-hover" style={{ height: '100%' }}>
                <div style={{ fontSize: '2.5rem', marginBottom: '0.75rem' }}>📖</div>
                <h3 style={{ marginBottom: '0.35rem', color: 'var(--text)' }}>{book.title}</h3>
                <p className="text-muted text-sm" style={{ marginBottom: '0.75rem' }}>
                  {new Date(book.releaseDate).getFullYear()}
                </p>
                <StarRating value={ratings[book.id] ?? 0} />
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
