// These interfaces mirror the C# DTO classes in BookReview/Dto/

export interface BookDto {
  id: number
  title: string
  releaseDate: string
}

export interface AuthorDto {
  id: number
  name: string
  bio: string
  countryName: string
}

export interface ReviewDto {
  id: number
  title: string
  text: string
  rating: number
  bookId: number
  reviewerId: number
}

export interface ReviewerDto {
  id: number
  firstName: string
  lastName: string
}

export interface CategoryDto {
  id: number
  name: string
}

export interface UserDto {
  username: string
  password: string
}

// API state wrapper — generic loading/error pattern
export interface ApiState<T> {
  data: T | null
  loading: boolean
  error: string | null
}
