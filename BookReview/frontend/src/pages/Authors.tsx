import { api } from '../api'
import { useApi } from '../hooks/useApi'
import type { AuthorDto } from '../types'

export default function Authors() {
  const { data: authors, loading, error } = useApi<AuthorDto[]>(() => api.getAuthors())

  if (loading) return <div className="loading">Loading authors...</div>
  if (error) return <div className="error-msg">Failed to load authors: {error}</div>

  return (
    <div>
      <h1 className="page-title">Authors</h1>
      {!authors?.length ? (
        <p className="empty-msg">No authors found.</p>
      ) : (
        <div className="grid">
          {authors.map((author) => (
            <div key={author.id} className="card card-hover">
              <div style={{ fontSize: '2.5rem', marginBottom: '0.75rem' }}>✍️</div>
              <h3 style={{ marginBottom: '0.4rem' }}>{author.name}</h3>
              {author.countryName && (
                <span className="badge" style={{ display: 'inline-block', marginBottom: '0.75rem' }}>
                  {author.countryName}
                </span>
              )}
              <p className="text-muted text-sm">{author.bio}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
