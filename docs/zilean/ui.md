# Zilean UI Guide

The Zilean UI provides a graphical dashboard to manage your ingested data, specifically torrent details.

## Enabling the Dashboard

The dashboard is disabled by default. To enable it, you need to set the `EnableDashboard` configuration property to `true`.

You can do this in your `/app/data/settings.json`:
```json
{
  "Zilean": {
    "EnableDashboard": true
  }
}
```

Or via an environment variable in your docker setup:
```bash
Zilean__EnableDashboard=true
```

After enabling it and starting Zilean, navigate to `http://localhost:8181/` (or the respective port you exposed) to access the UI.

## Dashboard Features

The main interface of the Zilean dashboard is the Torrents Grid, which displays all ingested torrent details.

### Searching and Filtering

The grid comes with built-in searching functionality.

**Regex Searching:**
The search box supports regular text search, but it also powerful regex matching. To use regex, simply wrap your search pattern with forward slashes.
- For example, `/(movie|show)/i` will do a case-insensitive search for "movie" or "show".

### Managing Torrents (CRUD)

The dashboard allows you to modify the ingested metadata:

- **Edit:** Double-click on any row, or select a row and click the "Edit" button in the toolbar. A dialog will open where you can update:
  - Info Hash
  - Raw Title
  - Size
  - **Override Category**: Force a category (`movie`, `tvSeries`, `xxx`).
  - **Override Year**: Force a specific year.
  - **Override Imdb Id**: Manually link the torrent to a specific IMDB ID.
  - **Override Adult Flag**: Mark the torrent as Adult.
  - **Override Trash Flag**: Mark the torrent as Trash.
- **Add:** Click the "Add" button to manually insert a new torrent entry into the database.
- **Delete:** Select one or more rows and click "Delete" to remove them from the database. A confirmation dialog will appear.

### Copying Magnet Links

If you want to manually grab the magnet links for one or more torrents:
1. Select the rows you want using the checkboxes on the left.
2. Click the **"Magnets"** button in the top toolbar (it has a magnet icon).
3. The properly formatted magnet URIs (including exact sizes, metadata, and trackers) will be copied to your clipboard.
