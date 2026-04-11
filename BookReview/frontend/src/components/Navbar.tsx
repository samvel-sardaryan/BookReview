import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import './Navbar.css'

export default function Navbar() {
  const { isLoggedIn, logout } = useAuth()
  const navigate = useNavigate()
  const { pathname } = useLocation()

  function handleLogout() {
    logout()
    navigate('/books')
  }

  function isActive(path: string) {
    return pathname.startsWith(path) ? 'active' : ''
  }

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link to="/books" className="navbar-brand">
          📚 BookReview
        </Link>

        <div className="navbar-links">
          <Link to="/books" className={isActive('/books')}>Books</Link>
          <Link to="/authors" className={isActive('/authors')}>Authors</Link>

          {isLoggedIn ? (
            <button className="btn-ghost" onClick={handleLogout}>
              Logout
            </button>
          ) : (
            <>
              <Link to="/login" className={isActive('/login')}>Login</Link>
              <Link to="/register" className="nav-register">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  )
}
