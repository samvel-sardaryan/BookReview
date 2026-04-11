import { useParams, Link } from 'react-router-dom'
import { api } from '../api'
import { useApi } from '../hooks/useApi'
import StarRating from '../components/StarRating'
import type { AuthorDto, ReviewDto } from '../types'

export default function BookDetail() {
  const { id } = useParams<{ id: string }>()
  const bookId = Number(id)

  const { data: book, loading: loadingBook, error: bookError } = useApi(
    () => api.getBook(bookId), [bookId]
  )
  const { data: authors } = useApi<AuthorDto[]>(
    () => api.getAuthorsOfBook(bookId), [bookId]
  )
  const { data: reviews } = useApi<ReviewDto[]>(
    () => api.getReviewsOfBook(bookId), [bookId]
  )
  const { data: rating } = useApi<number>(
    () => api.getBookRating(bookId), [bookId]
  )

  if (loadingBook) return <div className="loading">Loading...</div>
  if (bookError || !book) return <div className="error-msg">Book not found.</div>

  return (
    <div style={{ maxWidth: '720px' }}>
      <Link to="/books" className="text-muted text-sm" style={{ display: 'inline-block', marginBottom: '1.25rem' }}>
        ← Back to books
      </Link>

      <div className="card" style={{ marginBottom: '2rem' }}>
        <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📖</div>
        <h1 style={{ marginBottom: '0.4rem' }}>{book.title}</h1>
        <p className="text-muted text-sm" style={{ marginBottom: '1.25rem' }}>
          Released {new Date(book.releaseDate).toLocaleDateString()}
        </p>

        {authors && authors.length > 0 && (
          <div style={{ marginBottom: '1.25rem' }}>
            <span style={{ fontSize: '0.7rem', fontWeight: 600, letterSpacing: '0.07em', textTransform: 'uppercase', color: 'var(--muted)' }}>
              Authors
            </span>
            <div style={{ marginTop: '0.4rem', display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
              {authors.map((a) => (
                <span key={a.id} className="badge">{a.name}</span>
              ))}
            </div>
          </div>
        )}

        <StarRating value={rating ?? 0} />
      </div>

      <h2 style={{ marginBottom: '1rem' }}>
        Reviews{' '}
        <span className="text-muted" style={{ fontSize: '1rem', fontWeight: 400 }}>
          ({reviews?.length ?? 0})
        </span>
      </h2>

      {!reviews?.length ? (
        <p className="empty-msg">No reviews yet.</p>
      ) : (
        reviews.map((review) => (
          <div key={review.id} className="card" style={{ marginBottom: '1rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '0.5rem' }}>
              <h4>{review.title}</h4>
              <span className="stars" style={{ fontSize: '0.875rem', flexShrink: 0, marginLeft: '1rem' }}>
                {'★'.repeat(review.rating)}{'☆'.repeat(5 - review.rating)}
              </span>
            </div>
            <p className="text-muted text-sm">{review.text}</p>
          </div>
        ))
      )}
    </div>
  )
}
