# syntax=docker/dockerfile:1

# ── Build stage ───────────────────────────────────────────────────────────────
FROM golang:1.25-rc-alpine AS builder
WORKDIR /app
COPY go.mod .
COPY main.go .
# Build a fully static binary (no libc dependency needed for scratch base)
RUN CGO_ENABLED=0 GOOS=linux go build -ldflags="-s -w" -o mock-server .

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM scratch
COPY --from=builder /app/mock-server /mock-server
EXPOSE 8080
ENTRYPOINT ["/mock-server"]
