# Zilean Generic Agent Guide

This is a comprehensive guide tailored for AI Agents to understand how to install, configure, test, and use the Zilean service, including interfacing with "arr" applications like Prowlarr.

## 1. Understanding Zilean

Zilean is a service that searches for [DebridMediaManager (DMM)](https://github.com/debridmediamanager/debrid-media-manager) sourced content. It provides a Torznab-compliant API so media applications (Radarr, Sonarr, Prowlarr) can use it as an indexer. It also supports scraping from running Zurg or other Zilean instances.

## 2. Installation & Setup

Zilean is distributed via Docker.

### Pre-requisites
1. Docker or Podman.
2. An `.env` file containing a secure Postgres password:
   ```bash
   echo "POSTGRES_PASSWORD=$(openssl rand -base64 42 | tr -dc A-Za-z0-9 | cut -c -32 | tr -d '\n')" > .env
   ```

### Docker Compose
Create a `docker-compose.yml` file with Zilean and PostgreSQL (version 17+ recommended):

```yaml
volumes:
  zilean_data:
  zilean_tmp:
  postgres_data:
services:
  zilean:
    image: ipromknight/zilean:latest
    restart: unless-stopped
    container_name: zilean
    tty: true
    environment:
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "8181:8181"
    volumes:
      - ./data:/app/data
      - zilean_tmp:/tmp
    depends_on:
      postgres:
        condition: service_healthy
  postgres:
    image: postgres:17.2-alpine
    container_name: postgres
    restart: unless-stopped
    environment:
      PGDATA: /var/lib/postgresql/data/pgdata
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: zilean
    volumes:
      - postgres_data:/var/lib/postgresql/data/pgdata
    healthcheck:
      test: [ "CMD-SHELL", "pg_isready -U postgres" ]
      interval: 10s
      timeout: 5s
      retries: 5
```

Deploy with `docker compose up -d`.

## 3. Configuration

Configuration lives in `/app/data/settings.json`. Environment variables override JSON properties.

**Load Order**: `settings.json` -> Environment Variables.
**Env Format**: `Zilean__{Section}__{Key}` (e.g., `Zilean__Dmm__EnableScraping=true`)

### Key Settings
- `ApiKey`: Generated automatically on first run. Required for authenticated endpoints and instance-to-instance scraping.
- `EnableDashboard`: Set to `true` to enable the Blazor UI (disabled by default).
- `Dmm.EnableScraping`: Enable/Disable syncing DMM hash lists.
- `Database.ConnectionString`: Points to the Postgres instance.

> **Important Rate Limit Tip**: Pass a `GITHUB_TOKEN` environment variable to Zilean to authenticate git clones and avoid GitHub public rate limits when scraping DMM hash lists.

### Ingestion Setup
To scrape from Zurg or other Zilean nodes, edit `settings.json`:
1. Set `Ingestion.EnableScraping = true`
2. Populate `Ingestion.ZurgInstances`, `Ingestion.ZileanInstances`, or `Ingestion.GenericInstances` with `{ "Url": "http://instance", "EndpointType": 0/1/2 }`.

## 4. Database Migrations & Scraper

**Migrations**: Zilean handles DB migrations and index generation automatically on startup. The agent does not need to run manual migration scripts.

**Running the Scraper**:
The scraper runs on a schedule but can be invoked manually from within the container.
> **Warning**: Stop the main API container (`docker compose stop zilean`) before running the standalone scraper to avoid data corruption.

Run it manually via `docker run`:
```bash
docker run -it --rm \
    -v ./data/settings.json:/app/data/settings.json \
    --name zileanscraper \
    ipromknight/zilean:latest \
    /app/scraper <args>
```

**Common Scraper Args**:
- `dmm-sync`: Import DMM hash lists.
- `generic-sync`: Sync from configured Zilean/Zurg instances.
- `resync-imdb -s`: Force re-ingest IMDB metadata.
- `resync-imdb -a`: Rematch / update database entries with new IMDB IDs.

## 5. Interfacing with "Arrs" (Prowlarr)

Zilean exposes a Torznab API which is standard across media management apps.

### Add to Prowlarr
1. Go to **Indexers -> Add**.
2. Search for `generic` and select **Generic Torznab**.
3. Name: `Zilean`
4. URL: `http://zilean:8181/torznab`
5. API Path: `/api`
6. API Key: None needed for search (unless blocked via proxy, standard torznab routes on zilean are unauthenticated).
7. Sync to apps (Radarr/Sonarr).

### Radarr Tweaks
In Radarr settings (`/settings/indexers`), edit Zilean and check **"Remove year from search string"**.

## 6. API and Testing

Zilean's API can be explored (if running) via Scalar at `http://localhost:8181/scalar/v2`.

**Authentication**:
Endpoints requiring auth need the header: `X-Api-Key: <your-api-key>`.

**Testing Health**:
```bash
curl -X GET http://localhost:8181/healthchecks/ping
```
*(Should return a 200 OK).*

**Torznab Test**:
```bash
curl -X GET "http://localhost:8181/torznab/api?t=caps"
```
*(Should return XML capability list).*

## 7. Using the UI

If `EnableDashboard` is true, the UI is at `http://localhost:8181/`.
- Features a Syncfusion DataGrid showing all Torrents.
- **Search**: Supports regex (e.g., `/batman/i`).
- **Editing**: Double-click rows to override Category, Year, ImdbID, Adult/Trash flags.
- **Magnets**: Select rows and click the Magnet button to copy fully-formed `magnet:?xt=urn:btih:...` links to the clipboard.
