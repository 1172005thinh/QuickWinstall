# Development Tools

This directory contains Python utility scripts for QuickWinstall development.

## Requirements

- **Python 3.x** - All scripts require Python 3
- **ffmpeg** - Required only for `png2ico.py` (must be in system PATH)

## Scripts

### 1. png2ico.py

Convert PNG images to ICO format with automatic size detection.

**Features:**

- Automatically detects icon size from filename (files with '256' → 256x256, others → 16x16)
- Batch conversion of multiple files
- Dry-run mode to preview conversions
- Clean mode to remove generated .ico files
- Size filter to convert only specific sizes

**Usage:**

```bash
# Convert all PNGs in default folder (res/icons/pngs/)
python png2ico.py

# Specify input and output folders
python png2ico.py -i icons/pngs -o icons

# Convert only 16x16 icons
python png2ico.py -s 16

# Convert only 256x256 icons
python png2ico.py -s 256

# Preview conversion without creating files
python png2ico.py --dry-run

# Clean all .ico files in output folder (with confirmation)
python png2ico.py -c

# Show help
python png2ico.py -h
```

**Examples:**

- `app.png` → `app.ico` (16x16)
- `app256.png` → `app256.ico` (256x256)
- `logo_256.png` → `logo_256.ico` (256x256)

---

### 2. findIcons.py

Search for all .ico files in a directory and output their paths with sizes.

**Features:**

- Recursive directory search
- File size information
- Sorted output
- Relative paths from search directory

**Usage:**

```bash
# Search in default directory (res/icons/)
python findIcons.py

# Search in specific directory
python findIcons.py -d res/icons

# Specify output file
python findIcons.py -o icons_list.txt

# Show help
python findIcons.py -h
```

**Output:** `tools/presented_icons.txt`

---

### 3. findLangKeys.py

Extract localization keys from source code files.

**Features:**

- Scans all .cs files for `lang.GetString("key")` calls
- Multiple pattern matching (handles different LangManager access patterns)
- Shows key usage count and locations
- Sorted output

**Usage:**

```bash
# Scan default source directory (src/)
python findLangKeys.py

# Scan specific directory
python findLangKeys.py -d src

# Specify output file
python findLangKeys.py -o keys_list.txt

# Show help
python findLangKeys.py -h
```

**Output:** `tools/presented_langKeys.txt`

**Detected patterns:**

- `lang.GetString("key")`
- `LangManager.Instance.GetString("key")`
- `_langManager.GetString("key")`

---

### 4. sortLangKeys.py

Sort keys in language JSON files alphabetically.

**Features:**

- Recursive sorting (nested objects)
- Check mode to verify if files are sorted
- Dry-run mode to preview changes
- Preserves JSON formatting
- Batch processing

**Usage:**

```bash
# Sort all language files (res/langs/*.json)
python sortLangKeys.py

# Sort specific file
python sortLangKeys.py -f en-US.json

# Sort files in specific directory
python sortLangKeys.py -d res/langs

# Check if files are sorted (without modifying)
python sortLangKeys.py --check

# Preview changes without writing
python sortLangKeys.py --dry-run

# Show help
python sortLangKeys.py -h
```

---

### 5. compareResources.py

Compare resource usage against available resources to identify discrepancies.

**Features:**

- Compares used localization keys vs available keys
- Compares referenced icons vs available icons
- Identifies missing resources (used but not available)
- Identifies unused resources (available but not used)
- Generates comprehensive discrepancy report

**Usage:**

```bash
# Compare both keys and icons
python compareResources.py

# Compare only localization keys
python compareResources.py --keys-only

# Compare only icons
python compareResources.py --icons-only

# Specify output file
python compareResources.py -o report.txt

# Show help
python compareResources.py -h
```

**Output:** `tools/resource_comparison_report.txt`

---

## Typical Workflow

### After adding new icons

1. Place PNG files in `res/icons/pngs/`
2. Convert to ICO: `python tools/png2ico.py`
3. List all icons: `python tools/findIcons.py`
4. Compare usage: `python tools/compareResources.py --icons-only`

### After adding new localization strings

1. Extract keys from code: `python tools/findLangKeys.py`
2. Check `tools/presented_langKeys.txt` for new keys
3. Add missing keys to language files (`res/langs/*.json`)
4. Sort language files: `python tools/sortLangKeys.py`
5. Compare usage: `python tools/compareResources.py --keys-only`

### Before committing language files

```bash
# Ensure all language files are sorted
python tools/sortLangKeys.py --check

# Sort them if needed
python tools/sortLangKeys.py

# Check for resource discrepancies
python tools/compareResources.py
```

## Output Files

All scripts output to the `tools/` directory:

- `presented_icons.txt` - List of all .ico files found
- `presented_langKeys.txt` - List of all localization keys used in code
- `resource_comparison_report.txt` - Comparison report showing missing/unused resources

These files are useful for:

- Identifying missing localization keys
- Finding unused icons
- Identifying unused localization keys
- Documenting available resources
- Code review and validation
- Resource cleanup and optimization

## Notes

- All scripts support `-h` or `--help` for detailed usage information
- All scripts support `-v` or `--version` for version information
- Scripts use relative paths from the project root when possible
- Output files use UTF-8 encoding
- Run `findLangKeys.py` and `findIcons.py` before using `compareResources.py`
