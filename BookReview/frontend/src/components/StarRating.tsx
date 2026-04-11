interface Props {
  value: number
  max?: number
}

export default function StarRating({ value, max = 5 }: Props) {
  if (!value || value === 0) {
    return <span className="text-muted text-sm">No ratings yet</span>
  }
  const filled = Math.round(value)
  return (
    <span className="stars">
      {'★'.repeat(filled)}
      {'☆'.repeat(max - filled)}
      <span className="text-muted text-sm" style={{ marginLeft: '0.35rem' }}>
        ({value.toFixed(1)})
      </span>
    </span>
  )
}
